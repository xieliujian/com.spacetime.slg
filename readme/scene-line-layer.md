# 信息层线段同屏 N 条线段设计

[返回技术总览](../README.md)

本文说明 `SLGSceneLineLayer` 如何在同一屏幕内稳定支持大量线段（N 条）同时显示，并解释它与区域格子信息层的职责边界。

---

## 问题背景

SLG 战斗场景里，攻击指示线、路径线、引导线可能在同一帧并发出现。如果按“一条线段一个 GameObject + 一个 Renderer”处理，会带来以下问题：

- DrawCall 数量线性上涨
- 频繁创建/销毁对象导致 GC 抖动
- 线段更新（起终点变化）时 CPU 开销高

该模块采用“ID 映射 + 固定槽位 + 分块 Instancing”的方案，把 N 条线段的渲染成本压到少量批次调用。

---

## 1) 分层职责：为什么线段层独立于 Area 信息层

场景中存在两类信息叠加：

- `SLGAreaGridInfoLayer`：以“格子”为中心，数据结构天然是 25 个 Area × 每 Area 100 格
- `SLGSceneLineLayer`：以“跨格子/跨区域的线段”为中心，线段起终点不受 Area 边界约束

因此线段层不挂在 Area 内部，而是挂在 `SLGScene` 根级（统一渲染入口），避免跨 Area 拆线或重复写入多层格子数据。

---

## 2) 核心数据结构：同屏 N 条线段怎么被索引

### 2.1 UniqueID 到全局槽位

`SLGSceneLineLayer` 使用：

```csharp
Dictionary<uint, int> m_UniqueID2IndexDict;
```

- Key：业务唯一 ID（如单位 ID、路径 ID）
- Value：全局槽位索引 `globalIndex`

这让“重复 Add 同一个 ID”变成更新，而不是新增。也就是同一条业务线段在屏幕上始终占一个稳定槽位。

### 2.2 全局槽位到 Block 槽位

全局索引拆分规则：

```text
blockIndex  = globalIndex / 300
matrixIndex = globalIndex % 300
```

其中 300 来自 `SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM`。

这样 N 条线段会被均匀铺到多个 Block 中，每个 Block 固定容纳 300 条。

---

## 3) Block 机制：为什么能扩展到任意 N

### 3.1 固定容量块

每个 `SLGSceneLineBlock` 维护固定长度数组：

- `List<Matrix4x4> m_MatrixList`（长度 300）
- `List<float> m_EnemyPropList`（长度 300）
- `List<Vector4> m_UVScaleOffsetPropList`（长度 300）

并配套：

- `Stack<int> m_EmptyIndexStack`：空闲槽位池
- `Dictionary<int, bool> m_DataExistDict`：占用标记

### 3.2 自动扩容

`SLGSceneLineLayer.AddBlock()` 逻辑：

1. 先扫描已有 Block，找第一个未满块
2. 若都满，创建新 Block 并追加到 `m_BlockList`
3. 从块的空闲栈弹出一个 `matrixIndex`

所以理论容量是：

```text
最大线段数 = blockCount × 300
```

`blockCount` 可随需求增长，因此同屏支持 N 条线段不受固定上限（除设备资源约束）限制。

---

## 4) 渲染批次：N 条线段如何压成少量 DrawCall

每个 Block 一次 `Graphics.DrawMeshInstanced`：

```csharp
Graphics.DrawMeshInstanced(m_Mesh, 0, m_Mat, m_MatrixList, m_MatPropBlock, ...)
```

因此：

```text
DrawCall ≈ 有效 Block 数
      ≈ ceil(N / 300)
```

示例：

- N=120：1 个 Block，约 1 次调用
- N=650：3 个 Block，约 3 次调用
- N=1800：6 个 Block，约 6 次调用

这就是同屏大量线段可控的关键：渲染复杂度随 Block 数增长，而不是随每条线段单独增长。

---

## 5) 运行时更新路径：增、改、删都不分配

### 新增

`AddSceneLineInfo(id, start, end, enemy)`：

- 若 ID 不存在：分配空槽位，写入矩阵/样式，登记映射

### 更新

- 若 ID 已存在：通过映射直接定位旧槽位，覆写矩阵/样式

### 删除

`RemoveSceneLineInfo(id)`：

- 该槽位写回 `s_UnVisMatrix`
- 清除样式参数
- 槽位索引压回空闲栈可复用
- 从 ID 映射表删除

这一流程避免频繁 new/destroy，减少 GC 与碎片化。

---

## 6) Shader 参数设计：一套材质区分多条线

每实例除矩阵外，还带两类属性：

- `_Enemy`：0/1，表示友军或敌军样式
- `_UVScaleOffset`：按线段长度动态拉伸 UV，保证贴图沿线段方向连续

`SLGSceneLineBlock.SubmitGPU()` 仅在脏标记 `m_Dirty=true` 时更新 `MaterialPropertyBlock`，降低重复提交成本。

---

## 7) 线段 Shader 工作原理

线段的几何由 CPU 侧 `Matrix4x4.TRS` 决定（位置/朝向/长度），Shader 主要负责“每实例贴图采样与样式选择”。

### 7.1 Instancing 属性如何进 Shader

`SLGSceneLineBlock.SubmitGPU()` 把每实例数组写到 `MaterialPropertyBlock`：

- `_Enemy`（float，0/1）
- `_UVScaleOffset`（Vector4，`xy=缩放`，`zw=偏移`）

Shader 端通过 `UNITY_ACCESS_INSTANCED_PROP(Props, ...)` 逐实例读取，不同线段共用一套材质与同一次 DrawCall。

### 7.2 顶点阶段：UV 变换与流动动画

顶点函数 `SLGSceneLineVert` 调用 `CalcUV(srcUV)`（`SLGSceneLineForwardPass.hlsl`）后，按固定顺序做三步：

1. **镜像**：`newUV = CalcUVMirror(srcUV, _UVMirror)`
   - `_UVMirror = 0`：`newUV = srcUV`
   - `_UVMirror = 1`：`newUV = 1 - srcUV`

2. **子区域映射**：`newUV = newUV * uvScaleOffset.xy + uvScaleOffset.zw`
   - `uvScaleOffset.xy`：UV 缩放（子区域宽高）
   - `uvScaleOffset.zw`：UV 偏移（子区域起点）
   - 该值来自实例属性 `_UVScaleOffset`，不同线段可映射到不同纹理区域

3. **流动偏移**：`newUV += float2(flowRevert * _FlowSpeed * _Time.y, 0)`
   - 仅沿 U 方向滚动，V 不变
   - `flowRevert = lerp(1, -1, _UVMirror)`
   - 镜像时自动反转流向，避免视觉上“翻转后流向反了”

可合并为：

`UV_final = (mirror(UV0) * scale + offset) + (dir * speed * time, 0)`

其中 `scale=uvScaleOffset.xy`、`offset=uvScaleOffset.zw`、`dir=+1/-1`（由 `_UVMirror` 决定）。

### 7.3 片元阶段：友敌贴图分支 + 基色调制

片元函数 `SLGSceneLineFrag` 先做纹理选择：

- 采样 `_MainTex`
- 采样 `_EnemyTex`
- 用 `_Enemy` 线性插值：`lerp(mainTexColor, enemyTexColor, isEnemy)`

再与 `_BaseColor` 相乘得到最终 RGBA，最后走 `UniversalFragment_SLGScene` 完成管线侧统一处理。

### 7.4 深度与混合控制

该 Shader 暴露并使用：

- `Blend [_SrcBlend][_DstBlend]`
- `ZTest [_ZTest]`
- `ZWrite [_ZWrite]`
- `ZClip [_ZClip]`

因此同一套线段逻辑可通过材质参数切换为不透明或半透明风格，而不改代码路径。

### 7.5 旋转中心与 UV 长度计算

线段几何矩阵由 `CalcLineMatrix(startPos, endPos)` 生成，核心是先计算中点 `pos = (startPos + endPos) / 2`，再根据方向计算旋转，最后用 `matrix = TRS(pos, rot, scale)` 组合位置、旋转与长度缩放。由于平移目标是线段中点，最终视觉效果等价为线段围绕中点对齐方向，而不是围绕起点或世界原点旋转。

`_UVScaleOffset` 的 X/Z 由长度比例直接推导：`uvScaleX = lineLength / meshLength`，随后写入 `_UVScaleOffset.x = uvScaleX` 与 `_UVScaleOffset.z = -uvScaleX`。在忽略镜像和流动动画时，U 坐标变换为 `U' = U * uvScaleX + (-uvScaleX) = uvScaleX * (U - 1)`。这意味着原始 `U ∈ [0,1]` 会映射到 `[-uvScaleX, 0]`，本质是把右端锚定在 0，再向左展开与线段长度成比例的区间，并非“左移一半”。

线段 UV 长度缩放由世界长度与参考 Mesh 长度的比值决定：`uvScaleX = worldLineLength / meshReferenceLength`，其中 `worldLineLength = Distance(startPos, endPos)`，`meshReferenceLength` 来自线段资源初始化记录的 `meshScale.x`。例如 `worldLineLength = 20`、`meshReferenceLength = 10` 时，`uvScaleX = 2`，对应区间为 `[-2, 0]`。在此基础上，Shader 再叠加 `_FlowSpeed * _Time.y` 形成沿 U 方向滚动的流动效果。该设计把“长度决定采样跨度”和“时间决定滚动位移”拆分为两层独立控制，调参与稳定性更好。

### 7.6 与“同屏 N 条线段”能力的关系

线段层能同屏支持 N 条，不只依赖 C# 分块，也依赖 Shader 的 Instancing 读参模型：

- CPU：按块提交 `matrix[] + instancedProps[]`
- GPU：同一次 pass 内逐实例读取 `_Enemy/_UVScaleOffset`

结果是“每条线段样式可不同，但仍可批量绘制”，这正是 `ceil(N/300)` 批次数模型成立的前提之一。

---

## 8) 信息格子层数据存储与旋转支持（与地图层对比）

信息格子层并不是单一模式，存在“运行时写入型”和“预计算入库型”两条路径。

- 运行时写入型：`SLGAreaGridInfoLayer`
  - 初始化时仅创建固定 100 格的 `m_MatrixList/m_ColorList`，矩阵先填 `s_UnVisMatrix`
  - 运行时由 `AddGridInfo/RemoveGridInfo` 按逻辑格实时改写矩阵与颜色
  - 特点是响应动态标记（阵营、火力、路径点）快，但不依赖预烘焙 block DB

- 预计算入库型：`SLGAreaPropertyInfoLayer`
  - 编辑器阶段将每个 Area 的 `matrixList + uvScaleOffsetList` 写入 `SLGAreaPropertyInfoBlockDB`
  - 运行时按 `blockList[areaIndex]` 直接取块绘制
  - 特点与地图层更接近，偏静态展示（如资源等级）

因此，“信息格子层是否像地图层一样直接存 UV+Matrix 到 DB”取决于具体子类型：

- `SLGAreaGridInfoLayer`：不是（运行时写入）
- `SLGAreaPropertyInfoLayer`：是（预计算入库）

### Prefab 资源旋转支持

信息层支持 prefab 自带旋转。资源初始化时会记录 `meshRotation`（来自 `MeshRenderer` 所在节点的 `localRotation`），随后在信息层基类计算初始矩阵时参与 TRS：

`m_InitScaleMatrix = Matrix4x4.TRS(Vector3.zero, meshRotation, scale)`

这意味着格子叠加渲染会保留 prefab 的朝向，不会被强制重置为 `Quaternion.identity`。

---

## 9) 性能边界与约束

### 边界 1：每块固定 300

这里选 300 而不是 1023（Unity Instanced 上限）是工程折中：

- 预留属性数组更新与业务波动空间
- 单块数据体量更小，更新局部性更好
- 块数稍多但更稳定

### 边界 2：Render 空块跳过

当 `m_DataExistDict.Count == 0` 时，Block 直接 return，不发起 DrawCall。

### 边界 3：同 ID 不重复占槽

业务若多次调用 Add 同一 ID，不会扩增线段数，能避免“逻辑重复上报导致渲染爆炸”。

---

## 10) 与“信息层”关系的落地结论

- 格子信息层负责“每格状态表达”（颜色/属性）
- 线段层负责“跨格子关系表达”（起点-终点连线）
- 两者共享统一场景渲染时序，但数据结构解耦

这种设计让系统既能保持 Area 粒度的高效格子渲染，又能在全局维度支持同屏 N 条线段。

---

[返回技术总览](../README.md)
