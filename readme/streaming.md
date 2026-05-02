# 场景流化数据

[返回技术总览](../README.md)

场景流化（Streaming）是指将 Unity 场景中摆放的美术资产，通过编辑器工具**一键采集**并序列化为 `SLGSceneDB` ScriptableObject，供运行时零 GameObject 开销地驱动 GPU Instancing 渲染。

---

## 1) 整体流程

```
Unity 编辑场景（美术摆放 Prefab）
        │
        ▼
SLGSceneStreamer.ExportSceneDB()          ← 导出总入口
        ├── SLGRenderGridEdit.StreamerExport()
        │       ├── InitSceneDB()              → 初始化 25 个 Area + 注册共享格
        │       ├── FillRenderLayerSceneDB()   → 采集静态/动态渲染层
        │       ├── FillInfoLayerSceneDB()     → 采集信息层 Prefab 路径
        │       └── CalcAllAreaBounds()        → 计算各 Area 渲染包围盒
        └── SLGPropertyGridEdit.StreamerExport()
                ├── FillPropertyLayerSceneDB() → 采集属性格（选中属性 / 资源等级）
                ├── CalcAllPropertyGrid()      → 遍历全图 50×50 格写入属性 DB
                └── FillAllAreaPropertyLayer() → 将属性数据写入各 Area 属性层
        │
        ▼
SLGSceneDB（.asset）写入磁盘
        │
        ▼
运行时 SLGSceneMgr.Init(sceneDB) 直接消费
```

---

## 2) 导出总入口：SLGSceneStreamer

**文件**：`Editor/Scripts/SLGSceneStreamer.cs`

### ExportSceneDB()

```
返回值：SLGSceneDB
输出参数：
  outScenePath   — 当前活动场景的 Assets 路径
  outResPathList — 本次导出涉及的所有资源完整路径（prefab、材质、共享格）
```

执行步骤：

1. 通过 `SLGEditUtils.GetSLGSceneDBPath(scenePath)` 计算输出路径（同场景目录，文件名加 `_SLG.asset` 后缀）
2. 删除旧 `.asset`（`SafeRemoveAsset`）
3. 若场景中找不到 `Grid` 组件则提前退出（无法采集网格数据）
4. 创建新 `SLGSceneDB` ScriptableObject 并通过 `AssetDatabase.CreateAsset` 落盘占位
5. 依次调用 `SLGRenderGridEdit.StreamerExport` → `SLGPropertyGridEdit.StreamerExport`
6. `SLGPropertyGridEdit.ExportExcel`（`ART_SCENE_PROJECT` 宏下写属性 Excel）
7. `EditorUtility.SetDirty` + `AssetDatabase.SaveAssets` 保存

### CreateSLGAllRes()

接收 `resPathList`，在场景中创建名为 `"SLG"` 的根节点，将所有涉及的 Prefab 实例化为子对象并**默认失活**，用于 Addressables / AB 打包时收集依赖。

---

## 3) 渲染层采集：SLGRenderGridEdit.StreamerExport

**文件**：`Editor/Scripts/RenderGridEdit/SLGRenderGridEdit_Streamer.cs`

### InitSceneDB

- 调用 `SLGSceneDB.Init()` → `SLGAreaSetDB.Init()`，按 5×5 生成 25 个 `SLGAreaDB`，每个 Area 计算逻辑边界并初始化格子集合
- 查找共享格 Prefab（路径由 `SLGEditDefine.s_SLGInfoPrefab_PathPrefix + s_SLGShareGrid_PrefabName + ".prefab"` 拼接），若存在则调用 `resDB.InitShareGridRes()` 注册，同时将路径追加到 `outResPathList`

### FillRenderLayerSceneDB

- 查找场景中名为 `SLGRenderGrid` 的根节点（带 `Grid` 组件）
- 若根节点是 Prefab 实例，先 `UnpackPrefabInstance`（OutermostRoot）解除关联，确保后续采集子对象
- 遍历所有子层，通过 `SLGLayerConfigMgr.GetRenderLayer(childName)` 查层配置：
  - **普通静态层** → `FillMapRenderLayer()`
  - **动态层**（`renderLayerIsDynamic = true`）→ `FillDynamicMapRenderLayer()`

#### FillMapRenderLayer（静态层）

对层下所有 Prefab 实例（`SLGEditUtils.CollectAllPrefabByRootNode`）：

| 采集字段 | 来源 |
|---|---|
| `prefabPath` | `AssetDatabase.GetAssetPath(prefab)` |
| `pos` | `transform.position`（Y 轴归零：`SLGUtils.ResetPosY`）|
| `rot` | `transform.eulerAngles` |
| `scale` | `MeshRenderer.transform.localScale` |
| `uvScaleOffset` | `SLGUtils.CalcUVScaleOffsetByMesh(mesh)`（Vector4）|
| `matPath` | `AssetDatabase.GetAssetPath(sharedMaterial)` |
| `renderQueue` | `SLGEditUtils.CalcSLGRenderQueue(isOpacity, offset)` |

调用链：`sceneDB.FillAreaMapDB()` → 按世界坐标定位 Area → `resDB.AddRes()` 注册资源 → `areaDB.mapLayerSet.FillRenderBlockDB()` 写入块数据

> 地图层资源如何按材质合批、UV 子区域如何提取、运行时为何替换为 shareGrid Mesh，详见 → [地图层 GPU Instancing 合批设计](map-layer-instancing.md)

#### FillDynamicMapRenderLayer（动态层）

动态层额外从子节点名中解析 `dynamicMapIndex`（整数后缀），写入 `areaDB.dynamicMapLayerSet.FillRenderBlockDB()`，动态层资源用 `prefabPath`（而非 `matPath`）作为去重键。

### FillInfoLayerSceneDB

遍历 `SLGLayerConfigMgr.infoLayerCfgList`，对每条配置拼出 Prefab 路径并通过 `AssetDatabase.LoadAssetAtPath` 验证资源存在，调用 `sceneDB.FillAreaInfoLayerDB()` 写入：

- layerID、resPath、renderQueue、isZWriteOn
- `SLGInfoLayerType`（普通信息层 vs 属性信息层）
- `SLGAreaGridPropertyLayerType` + `propertyTexSeq`（属性贴图序列尺寸）

信息层 Prefab 路径同时追加到 `outResPathList`（去重）。

### CalcAllAreaBounds

遍历 25 个 `SLGAreaDB`，调用 `CalcBounds()` 根据已填充的渲染块位置计算各 Area 的 AABB，供运行时视锥剔除使用。

---

## 4) 属性层采集：SLGPropertyGridEdit.StreamerExport

**文件**：`Editor/Scripts/PropertyGridEdit/SLGPropertyGridEdit_Streamer.cs`

### FillPropertyLayerSceneDB

- 查找场景中名为 `SLGPropertyGrid` 的根节点（带 `Grid` 组件），同样先解包 Prefab
- 遍历子节点，按节点名分发：
  - `SelPropertyLayer` → `FillSceneSelPropertyInfo()`：写入选中属性（`SelectPropertyType`）
  - `ResLvPropertyLayer` → `FillSceneResLvPropertyInfo()`：写入资源等级（`ResLvType`）
- 坐标转换：`SLGUtils.ConvertSLG3DPosToLogicPos(pos)` 将世界坐标 → 逻辑网格坐标

### CalcAllPropertyGrid

遍历全图所有 50×50 = 2500 个逻辑格，对每格调用 `GetOrCreatePropertyGridDB()` 确保 DB 条目存在，并调用 `Null()` 初始化默认属性值（清零/重置）。

### FillAllAreaPropertyLayer

遍历 `infoLayerSet.propertyLayerList`，对属于 `ResLvState` 类型的属性层调用 `FillAreaResLvStateLayer()`，将属性数据按 Area 分块写入对应的 `SLGAreaPropertyInfoLayerDB`。

---

## 5) 产出数据结构：SLGSceneDB

`SLGSceneDB` 是一个 `ScriptableObject`（`.asset` 文件），包含三块：

```
SLGSceneDB
├── resDB : SLGSceneResDB          — 资源索引表
│   ├── shareGridResDB             — 共享格 Prefab 路径（Resources 相对路径）
│   ├── resDBList[]                — 渲染资源列表，按材质路径去重
│   │   └── SLGResDB { resPath, matPath, renderQueue, zWriteOn }
│   └── customResDBList[]          — 信息层/自定义资源列表
│
├── propDB : SLGScenePropDB        — 属性格数据
│   └── propertyGridDBList[]       — 每个逻辑格的 SLGPropertyGridDB
│       └── { selectPropertyType, resLvType, ... }
│
└── areaSetDB : SLGAreaSetDB       — 25 个 Area 的渲染数据
    ├── infoLayerSet               — 全局信息层配置
    │   ├── layerList[]            — 普通信息层（颜色叠加）
    │   └── propertyLayerList[]    — 属性信息层（贴图序列）
    └── areaDBList[25]             — 5×5 Area 分块
        └── SLGAreaDB
            ├── mapLayerSet        — 静态渲染层块（GPU Instancing 数据）
            ├── dynamicMapLayerSet — 动态渲染层块
            ├── bounds             — AABB（视锥剔除用）
            └── gridSet            — 格子集合（100 格/Area）
```

---

## 6) 资源路径转换规则

导出时所有资源路径存两份：

| 字段 | 格式 | 用途 |
|---|---|---|
| `realResPathList`（NonSerialized）| `Assets/Resources/.../xxx.prefab`（完整 Assets 路径）| 打包时收集依赖 |
| `resDB.resPath`（序列化）| `scene/common/.../xxx`（Resources 相对路径，小写，无扩展名）| 运行时 `Resources.Load<T>()` |

转换由 `SLGSceneResDB.ConvertToResourcesPath()` 完成：去掉 `assets/resources/` 前缀 + 去掉扩展名 + 全部小写。

---

## 7) 与运行时的边界

| 阶段 | 职责 |
|---|---|
| **Editor 导出** | 遍历场景 GameObject，采集 Transform / Mesh / Material，写入 `SLGSceneDB` |
| **运行时初始化** | `SLGSceneMgr.Init(sceneDB)` 读取 DB，通过 `Resources.Load` 加载资源，构建 GPU Instancing 渲染批次 |
| **运行时帧更新** | `SLGSceneMgr.Update()` 按 Area AABB 做视锥剔除，提交可见批次 |

运行时**不持有任何场景 GameObject**，全部数据来自 `SLGSceneDB`。场景中的原始 Prefab 只在编辑器阶段使用，打包后可由 Addressables 管理按需加载。

---

[返回技术总览](../README.md)
