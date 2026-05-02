# 信息格子层设计

[返回技术总览](../README.md)

本文聚焦信息格子层的数据存储模型与渲染路径，说明其与地图层在“是否预计算入库”的差异，以及 prefab 旋转在运行时的保留方式。

---

## 设计目标

信息格子层需要同时满足两类需求：

- 动态标记：阵营、火力、路径点等频繁变化的格子覆盖
- 静态属性：资源等级等可预计算、可直接批渲染的格子状态

因此系统采用“双路径”设计，而不是单一数据形态。

---

## 1) 两类信息格子层

### 1.1 运行时写入型：SLGAreaGridInfoLayer

特点：

- 初始化只分配固定 100 格容器（每 Area）：
  - `m_MatrixList` 先填 `s_UnVisMatrix`
  - `m_ColorList` 先填 `Color.clear`
- 运行时通过 `AddGridInfo/RemoveGridInfo` 按逻辑格写入或移除
- 仅在有数据时提交并绘制（`m_DataExistDict.Count > 0`）

适用场景：高频动态变化的格子信息。

### 1.2 预计算入库型：SLGAreaPropertyInfoLayer

特点：

- 编辑器阶段把每个 Area 的数据预计算到 DB：
  - `matrixList`
  - `uvScaleOffsetList`
- 运行时直接取 `blockList[areaIndex]` 绘制
- 初始化时一次性把 UV 参数写入 `MaterialPropertyBlock`

适用场景：资源等级等偏静态、可离线生成的信息层。

---

## 2) 与地图层的关系

地图层（`SLGAreaMapBlockDB`）天然采用预计算入库模式，保存每实例 `matrixList + uvScaleOffsetList`。

信息格子层不是完全一致：

- `SLGAreaGridInfoLayer`：不是地图层式入库，走运行时写入
- `SLGAreaPropertyInfoLayer`：与地图层同型，走预计算入库

结论：是否“像地图层一样存 UV+Matrix 到 DB”取决于信息层子类型，而非“信息格子层”整体。

---

## 3) Prefab 旋转支持

信息层支持 prefab 自带旋转。

资源初始化阶段会记录：

- `meshScale`
- `meshRotation`（来自 MeshRenderer 节点 `localRotation`）

信息层基类在计算初始矩阵时使用：

`m_InitScaleMatrix = Matrix4x4.TRS(Vector3.zero, meshRotation, scale)`

这保证格子叠加渲染会保留 prefab 朝向，而不是强制 identity 旋转。

---

## 4) 设计收益

- 动态层与静态层各走最合适的数据路径，避免“一刀切”
- 动态层避免不必要的 DB 膨胀，响应快
- 静态层减少运行时计算，批渲染成本稳定
- 统一支持 prefab 朝向，降低美术资源适配成本

---

[返回技术总览](../README.md)
