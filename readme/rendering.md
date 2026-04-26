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

叠加在地形之上，用于显示阵营、火力、路径点等游戏信息。

### SLGAreaGridInfoLayer

- 以逻辑坐标为 Key，存储每格的颜色
- 调用 `AddAreaGridInfo(layerType, logicPos, color)` 写入，`SubmitGPUByLayer(layerType)` 后生效
- 内部重建 Mesh（顶点着色），通过 `Graphics.DrawMesh` 渲染

### SLGAreaPropertyInfoLayer

- 按 `ResLvType` 分块，每块对应一种资源等级的颜色与矩阵集合
- 数据来自 `SLGAreaPropertyInfoLayerDB`，初始化时一次性加载
- 可见性由 `SetAreaPropertyLayerVisible` 控制

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
