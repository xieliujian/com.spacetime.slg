# 数据层（DB）

[返回技术总览](../README.md)

所有 DB 类均为 `ScriptableObject`，纯数据，不含运行时逻辑。通过 Editor 工具生成并序列化到磁盘，运行时只读。

---

## 数据结构关系

```
SLGSceneDB
├── SLGSceneResDB          ← 资源引用（Mesh、Material、Prefab）
├── SLGScenePropDB         ← 全图属性网格（50×50 格）
└── SLGAreaSetDB
    └── SLGAreaDB × 25
        ├── SLGAreaGridSetDB
        │   └── SLGAreaGridDB × 100    ← 每格的位置与变换矩阵
        ├── SLGAreaMapLayerSetDB
        │   └── SLGAreaMapLayerDB × N
        │       └── SLGAreaMapBlockDB × N   ← 渲染批次块
        ├── SLGAreaDynamicMapLayerDB × N    ← 动态地图层
        └── SLGAreaInfoLayerSetDB
            ├── SLGAreaInfoLayerDB × N
            └── SLGAreaPropertyInfoLayerDB
                └── SLGAreaPropertyInfoBlockDB × N
```

---

## 关键 DB 类说明

### SLGSceneDB
顶层容器，传入 `SLGSceneMgr.Init()` 完成整个场景初始化。

### SLGSceneResDB
存储场景所需的所有资源引用：地形 Mesh、Material、动态对象 Prefab 等。  
`SLGResMgr` 在初始化时从此处加载并缓存。

### SLGScenePropDB
全图 50×50 格的属性数组，每格对应一个 `SLGPropertyGridDB`，记录：
- 选中属性类型（`SelectPropertyType`）
- 资源等级类型（`ResLvType`）

### SLGAreaGridDB
单格数据，包含：
- `logicPos`：逻辑坐标
- `matrix`：世界变换矩阵（预计算，供 GPU Instancing 直接使用）

### SLGAreaMapBlockDB
渲染批次块，存储一批格的 `Matrix4x4[]`，直接传入 `Graphics.DrawMeshInstanced`。

### SLGAreaDynamicMapLayerDB
动态地图层数据，支持多状态切换（通过 `SetDynamicMapIndex` 控制显隐）。

### SLGAreaPropertyInfoLayerDB / SLGAreaPropertyInfoBlockDB
属性信息图层数据，按资源等级类型分块存储颜色与矩阵，用于叠加显示资源等级状态。

### SLGResDB
单条资源记录，包含资源路径或直接引用，由 `SLGResMgr` 管理生命周期。

---

## 数据流

```
Editor 工具（属性/渲染网格编辑）
        ↓  生成/修改
ScriptableObject（DB 文件）
        ↓  Init() 时传入
SLGSceneMgr → SLGScene → SLGArea → 各 Layer
        ↓  运行时只读
GPU Instancing / Mesh 渲染
```

---

[返回技术总览](../README.md)
