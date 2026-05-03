# 层级的混合处理

[返回技术总览](../README.md)

## 背景与问题

SLG 场景由多层叠加的平面 Quad 组成：地形底层、信息标记层、动态地图层、半透明叠加层等。这些层全部绘制在同一个相机视角下，因此必须有明确的前后遮挡关系。

**核心问题：GPU Instancing 的 `DrawMeshInstanced` 无法通过调用顺序保证前后遮挡。**

测试发现：如果多个层共享相同的 `RenderQueue` 值，GPU 驱动可以任意重排渲染指令，依赖 API 调用顺序的层叠关系将失效，产生穿插闪烁或随机遮挡错误。

**唯一可靠方案：为每一层分配唯一的 `RenderQueue` 值。**

---

## 三个决定层级行为的字段

每一层在 `SLGLayerConfig` 中有三个关键字段，全部来自 `SLGLayer_WPS.xlsx`：

| 字段 | 含义 |
|------|------|
| `isOpacityLayer` | 该层是否使用透明/Alpha 混合渲染（true = 透明混合，false = 标准不透明）|
| `renderQueueOffset` | 在该层所在 RenderQueue 基准段上的偏移量 |
| `isZWriteOn` | 是否开启深度写入（ZWrite） |

---

## RenderQueue 计算规则

```csharp
// SLGEditUtils.CalcSLGRenderQueue
public static int CalcSLGRenderQueue(bool isOpacity, int renderQueueOffset)
{
    if (isOpacity)
        return 2000 + renderQueueOffset;   // Opacity 段：地形 / 信息标记层
    else
        return 3000 + renderQueueOffset;   // Transparent 段：半透明叠加层
}
```

- **`isOpacity = true`**：基准 2000，落在 Unity 不透明几何段前后（实际偏移为负值，使这些层在标准场景物体之前渲染）
- **`isOpacity = false`**：基准 3000，落在 Unity 透明段（半透明叠加层，始终渲染在所有地形之上）

---

## 各层配置一览（来自 SLGLayer_WPS.xlsx）

### 渲染层（IsInfoLayer = 0）

| 层名 | LayerID | IsOpacity | RenderQueueOffset | 最终 RenderQueue | IsZWrite | 说明 |
|------|---------|-----------|-------------------|-----------------|----------|------|
| BottomMap1 | 0 | ✓ | -12 | **1988** | ✓ | 最底层地形，最先渲染，写深度 |
| BottomMap2 | 1 | ✓ | -11 | **1989** | ✓ | 底层地形第二层，写深度 |
| TopMap1 | 2 | ✓ | -3 | **1997** | ✓ | 顶层地形第一层，写深度 |
| TopMap2 | 3 | ✓ | -2 | **1998** | ✓ | 顶层地形第二层，写深度 |
| DynamicMap1~5 | 4 | ✓ | -1 | **1999** | ✓ | 动态切换地图层，最晚渲染，写深度 |

### 信息层（IsInfoLayer = 1）

| 层名 | LayerID | IsOpacity | RenderQueueOffset | 最终 RenderQueue | IsZWrite | 说明 |
|------|---------|-----------|-------------------|-----------------|----------|------|
| CampInfo | 5 | ✓ | -6 | **1994** | ✗ | 阵营着色，叠在底层地形上，不写深度 |
| FireInfo | 6 | ✓ | -5 | **1995** | ✗ | 火力着色，叠在 CampInfo 上，不写深度 |
| ResLvProperty | 10 | ✗ | 96 | **3096** | ✗ | 资源等级属性，透明段，始终最上层 |
| AreaTargetPos | 7 | ✗ | 97 | **3097** | ✗ | 目标点标记，透明段 |
| AreaWayPoint | 8 | ✗ | 98 | **3098** | ✗ | 路径点标记，透明段 |
| SceneLine | 9 | ✗ | 99 | **3099** | ✗ | 场景连线，透明段，最前层 |

---

## 完整层级顺序（由远至近）

```
1988  BottomMap1       ← 地形最底层（ZWrite ON，建立深度基准）
1989  BottomMap2       ← 地形底层补充
1994  CampInfo         ← 阵营信息叠加（ZWrite OFF，Alpha 混合）
1995  FireInfo         ← 火力信息叠加（ZWrite OFF，Alpha 混合）
1997  TopMap1          ← 地形顶层（ZWrite ON，覆盖信息层）
1998  TopMap2          ← 地形顶层补充
1999  DynamicMap       ← 动态地图，最后写深度
─────────────────────────────────────────────── 分界（2000 = Unity Geometry 默认基准）
3096  ResLvProperty    ← 资源等级（透明叠加，不参与深度竞争）
3097  AreaTargetPos    ← 目标点
3098  AreaWayPoint     ← 路径点
3099  SceneLine        ← 场景连线（最顶层显示）
```

---

## 深度写入的作用

`IsZWriteOn` 控制该层是否向深度缓冲区写入当前像素深度：

- **ZWrite ON**（地形层）：渲染后在深度缓冲留下"屏障"，后续层若开启 ZTest，位于其后方的像素会被剔除。地形层彼此之间因 RenderQueue 不同，不会互相遮挡测试。
- **ZWrite OFF**（信息层、透明叠加层）：不修改深度缓冲，始终绘制。可以正确叠加在地形上而不影响后续层的深度判断。

---

## 为什么相同 RenderQueue + DrawInstance 顺序无效

`Graphics.DrawMeshInstanced` 将渲染指令提交至 GPU 命令缓冲区。当两个 Draw Call 的 `RenderQueue` 相同时，GPU 驱动及渲染管线**不保证执行顺序**（允许重排以优化 State Change 或硬件吞吐）。

实测结果：两层共用相同 `RenderQueue` 时，不论 C# 端的调用先后，GPU 侧的像素覆盖关系随帧变化，无法形成稳定的前后遮挡。

**结论：每一层必须使用唯一的 `RenderQueue` 值，不可共用。**

`renderQueueOffset` 字段正是为此而设计：通过差异化偏移，确保每层拥有独立的 `RenderQueue`，从而由 GPU 渲染管线原生保证层叠顺序，彻底规避 Draw Call 乱序问题。

---

[返回技术总览](../README.md)
