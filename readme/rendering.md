# 渲染系统

[返回技术总览](../README.md)

## 整体策略

所有地形与信息图层均使用 **GPU Instancing**（`Graphics.DrawMeshInstanced`），以 Area 为单位批量提交，配合视锥剔除减少无效绘制。

---

## 地图层渲染

### 静态地图层（SLGAreaMapLayer / SLGAreaMapBlock）

- 每个 Area 包含一组 `SLGAreaMapLayer`，每层对应一种地形材质
- 每层内部按 1023 个实例为上限拆分为多个 `SLGAreaMapBlock`（`DrawMeshInstanced` 单次上限）
- `SLGAreaMapBlock` 直接持有预计算的 `Matrix4x4[]`，每帧直接提交，无运行时计算

### 动态地图层（SLGAreaDynamicMapLayer）

- 支持多状态切换，通过 `SLGSceneMgr.SetDynamicMapIndex(int)` 控制当前显示的状态索引
- 底层由 `SLGAreaDynamicMapLayerSet` 管理，按索引激活对应层

---

## 信息图层（Info Layer）

叠加在地形之上，分为两种完全不同的数据驱动模式：

| | SLGAreaGridInfoLayer | SLGAreaPropertyInfoLayer |
|---|---|---|
| **典型用途** | 阵营、火力、路径点等动态标记 | 资源等级（ResLvProperty）等静态属性展示 |
| **数据来源** | 运行时逻辑层逐格写入 | 编辑器导出阶段预计算，存入 DB |
| **Init 之后** | 100 格矩阵全为 `s_UnVisMatrix`，颜色全为 `Color.clear`，空白等待 | 直接从 `blockList[areaIndex]` 取出本 Area 数据块，所有格子就绪 |
| **运行时写入** | `AddGridInfo / RemoveGridInfo` 逐格更新 | 无，只读 |
| **GPU 提交** | 变更后需调 `SubmitGPUByLayer` 才推送颜色数组 | Init 时一次性写入 MaterialPropertyBlock，之后无需再提交 |
| **整层开关** | 无（逐格增删控制） | `SetAreaPropertyLayerVisible(layerType, bool)` 广播全部 25 个 Area |

---

### SLGAreaGridInfoLayer — 动态信息层

`Init()` 预分配 100 格的矩阵列表与颜色列表，全部置为不可见默认值：

```csharp
InitMatrixList();  // 100 格全填 s_UnVisMatrix（缩放为 0，GPU 剔除）
InitColorList();   // 100 格全填 Color.clear
```

逻辑层通过公开 API 逐格操作：

```csharp
// 添加单格颜色标记（标脏，不立即推送 GPU）
SLGSceneMgr.Instance.AddAreaGridInfo(SLGInfoLayer.CampInfo, logicPos, color);

// 批量改完后统一提交
SLGSceneMgr.Instance.SubmitGPUByLayer(SLGInfoLayer.CampInfo);

// 移除单格标记
SLGSceneMgr.Instance.RemoveAreaGridInfo(SLGInfoLayer.CampInfo, logicPos);
```

`Render()` 仅在 `m_DataExistDict.Count > 0` 时才调用 `DrawMeshInstanced`，无标记时跳过绘制。

---

### SLGAreaPropertyInfoLayer — 静态属性层

数据在**编辑器导出**阶段由 `FillAreaResLvStateLayer` 全量预计算并写入 `SLGAreaPropertyInfoLayerDB.blockList`（25 个 Area 各一个 Block，每 Block 含 100 格的矩阵与 UV 偏移）。

`Init()` 直接取出对应 Block 并写入 MaterialPropertyBlock，之后只读：

```csharp
m_PropertyInfoBlockDB = blockList[m_AreaIndex];              // 取本 Area 的数据块
m_MatPropBlock.SetVectorArray(s_SLG_Shader_SceneObj_UvScaleOffsetId,
    m_PropertyInfoBlockDB.uvScaleOffsetList);                // UV 一次性写入
```

无资源等级的格子在导出时已填入 `s_UnVisMatrix`，运行时 GPU 自动剔除，无需任何逻辑干预。

整层显示/隐藏：

```csharp
// 广播给全部 25 个 Area 的对应层，一次调用切全场景
SLGSceneMgr.Instance.SetAreaPropertyLayerVisible(SLGInfoLayer.ResLvProperty, true);
```

---

### 小地图纹理

```csharp
// 生成 50×50 像素的小地图纹理，每像素对应一个逻辑格
SLGSceneMgr.Instance.FillMiniMapTexture(tex, SLGInfoLayer.CampInfo);
```

---

## 场景线段（SLGSceneLineLayer）

用于绘制单位间的连线（如攻击线、路径线）。

- 以 `uint uniqueID` 标识每条线段，支持世界坐标或逻辑坐标输入
- 内部使用 `SLGSceneLineBlock` 对象池，每块最多容纳 **300 条线段**
- 区分友方（`enemy=false`）和敌方（`enemy=true`），使用不同材质

```csharp
// 添加线段
SLGSceneMgr.Instance.AddSceneLineInfo(id, startLogicPos, endLogicPos, isEnemy);

// 移除线段
SLGSceneMgr.Instance.RemoveSceneLineInfo(id);
```

---

## 视锥剔除

`SLGScene.Update()` 每帧对 25 个 Area 执行 AABB 视锥测试：

```csharp
// SLGUtils 提供快速平面-AABB 测试
SLGUtils.TestPlanesAABBInternalFast(planes, center, extents)
```

只有通过测试的 Area 才会提交渲染，未通过的 Area 跳过所有 `DrawMeshInstanced` 调用。

---

## Shader 属性 ID（SLGDefine）

运行时通过预缓存的 `int` ID 设置 Shader 属性，避免字符串查找：

| 字段 | Shader 属性 |
|------|-------------|
| `SHADER_MAIN_TEX` | `_MainTex` |
| `SHADER_COLOR` | `_Color` |
| `SHADER_UV_SCALE_OFFSET` | `_UVScaleOffset` |
| `SHADER_DYNAMIC_MAP_FLAG` | `_DynamicMapFlag` |

---

[返回技术总览](../README.md)
