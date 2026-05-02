# 地图层 GPU Instancing 合批设计

[返回技术总览](../README.md)

描述**不同 Tile Prefab 如何在运行时合并为一次 `DrawMeshInstanced` 调用**的完整数据链路，包含编辑器导出阶段的采集逻辑和运行时的 Mesh/UV 替换策略。

---

## 问题背景

Tilemap 场景中通常刷有数百到数千个 Tile Prefab 实例，它们共享同一张纹理图集但各自引用**不同的 UV 子区域**（草地、泥地、石头分别占图集不同位置）。若逐实例绘制会产生大量 DrawCall，因此需要在导出阶段将同材质的所有实例合并为单次 GPU Instancing 调用。

---

## 1) 导出阶段：按材质分组

每个 Tile Prefab 实例的采集字段：

| 字段 | 来源 |
|---|---|
| `pos / rot / scale` | `transform.position / eulerAngles / localScale` |
| `uvScaleOffset` | `SLGUtils.CalcUVScaleOffsetByMesh(mesh)` |
| `prefabPath` | `AssetDatabase.GetAssetPath(prefab)`（首次注册用）|
| `matPath` | `AssetDatabase.GetAssetPath(sharedMaterial)`（**分组键**）|

**`matPath` 是分组键**，同一材质的所有实例归入同一个 `SLGAreaMapBlockDB`：

```
resDB.AddRes(prefabPath, matPath, ...)
    // 以 matPath 去重；首次注册时存入 prefabPath，后续同 matPath 跳过

resId = resDB.FindResId(matPath)
    // matPath → resId（即 resDBList 的下标）

FillRenderBlockDB(..., resId, matrix, uvScaleOffset)
    // resId 相同 → matrix 和 uvScaleOffset 追加到同一个 Block
```

同一 `matPath` 无论被多少种 Prefab 引用，最终归入**同一个 Block**，一次 DrawCall 搞定。

---

## 2) UV 子区域提取：CalcUVScaleOffsetByMesh

Tile Prefab 的 Mesh 是一个 Quad，4 个 UV 顶点的坐标直接指向图集中的子区域（如 `(0.25, 0.0)` ~ `(0.5, 0.5)`）。

```csharp
// SLGUtils.CalcUVScaleOffsetByMesh(mesh)
// 遍历 4 个 UV 顶点，取包围盒
min = (min_u, min_v)   // 子区域左下角
max = (max_u, max_v)   // 子区域右上角

uvScaleOffset = Vector4(
    max.x - min.x,   // x = 子区域宽度（U 方向缩放）
    max.y - min.y,   // y = 子区域高度（V 方向缩放）
    min.x,           // z = 子区域起始 U（偏移）
    min.y            // w = 子区域起始 V（偏移）
)
```

前提条件：Mesh 必须恰好有 **4 个 UV 顶点**，否则返回 `Vector4.zero` 并报警告。

每个实例的 `uvScaleOffset` 独立记录它在图集中的位置与尺寸，与其他实例无关。

---

## 3) 矩阵计算：Matrix4x4.TRS

```csharp
// SLGUtils.FillRenderBlockDB 内部
var quat = Quaternion.Euler(rot);
var matrix = Matrix4x4.TRS(pos, quat, scale);
block.matrixList.Add(matrix);
block.uvScaleOffsetList.Add(uvScaleOffset);
```

矩阵编码了实例的完整世界变换，运行时 GPU 直接使用，不再做任何计算。

---

## 4) 运行时：统一替换为 shareGrid Mesh

`SLGResMgr.InitSceneResDict()` 加载每条 resDB 记录：

```csharp
var resGo = Resources.Load<GameObject>(resDB.resPath);  // 加载该材质对应的 prefab

SLGRes res = new SLGRes();
res.InitMesh();                        // 从 prefab 上提取 mat / meshScale / meshRotation
res.SetMesh(m_ShareGridRes.mesh);      // ← 关键：用 shareGrid 标准 Quad 覆盖原 prefab 的 Mesh
```

`InitMesh()` 主要目的是**提取材质**并开启 GPU Instancing：

```csharp
m_Mesh = meshFilter.sharedMesh;        // 先拿到原 mesh（会被立即覆盖）
m_Mat  = render.sharedMaterial;        // 保留材质（包含图集纹理）
m_Mat.enableInstancing = true;         // 确保开启 GPU Instancing
m_Mat.renderQueue = resDB.renderQueue;
```

覆盖之后各资源的来源：

| 资源 | 来源 | 说明 |
|---|---|---|
| **Mesh** | `shareGridResDB` 的 shareGrid Prefab | 标准 Quad，UV 0~1 |
| **Material** | 原始 Tile Prefab | 含图集纹理 |
| **uvScaleOffset**（per-instance）| 导出时从 Tile Mesh UV 提取 | 图集子区域坐标 |

---

## 5) Shader UV 重映射

运行时 Shader 对每个实例：

```
finalUV = quadUV * uvScaleOffset.xy + uvScaleOffset.zw
        = [0,1]  × (宽,高)           + (偏移_u, 偏移_v)
```

shareGrid Quad 的标准 UV `[0,1]` 经过 per-instance 的缩放和偏移后，精确映射到该 Tile 在图集中的子区域，采样出正确的地形纹理。

---

## 6) 完整数据链路

```
编辑器场景：N 个 Tile Prefab（同一材质，各自 UV 子区域不同）
        │
        ▼ FillMapRenderLayer（逐实例）
        ├─ CalcUVScaleOffsetByMesh() → uvScaleOffset（图集坐标）
        ├─ Matrix4x4.TRS()           → matrix（世界变换）
        └─ 按 matPath 写入 SLGAreaMapBlockDB
              { resId, matrixList[], uvScaleOffsetList[] }
        │
        ▼ 序列化为 SLGSceneDB.asset
        │
        ▼ 运行时 SLGResMgr.Init()
        ├─ 加载 prefab → 提取 mat → 开启 GPU Instancing
        └─ SetMesh(shareGrid)  ← 覆盖 Mesh
        │
        ▼ SLGAreaMapBlock.Init()
        └─ uvScaleOffsetList → MaterialPropertyBlock.SetVectorArray
        │
        ▼ SLGAreaMapBlock.Render()（每帧，Area 通过视锥剔除后）
        └─ Graphics.DrawMeshInstanced(
               shareGridMesh, 0, mat,
               matrixList, matPropBlock)   ← 一次调用绘制全部实例
```

---

## 7) 为什么运行时可以替换成 shareGrid Mesh

这是整个设计中最关键也最容易让人疑惑的一步。替换能成立，依赖四个相互独立的前提。

### 前提一：地图块的 Scale 在导出时已烘入 TRS 矩阵

导出时采集的 `scale` 来自 `MeshRenderer.transform.localScale`（即美术摆放时该 Prefab 渲染器的本地缩放），并在 `SLGUtils.FillRenderBlockDB` 中直接烘入每个实例的矩阵：

```csharp
var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale);
block.matrixList.Add(matrix);
```

运行时 `DrawMeshInstanced` 用这个矩阵变换 Mesh 顶点。无论传入的是原 Tile Mesh 还是 shareGrid Mesh，GPU 都会把它们的顶点乘以同一个矩阵，渲染到相同的世界位置、旋转和尺寸。**Mesh 几何本身的大小已经不重要了。**

### 前提二：Shader 不使用 Mesh 自带的 UV，而是用 per-instance uvScaleOffset

Tile Prefab 的 Mesh UV 只在**导出阶段**被读取一次（`CalcUVScaleOffsetByMesh`），之后就不再使用。运行时 Shader 通过 `MaterialPropertyBlock.SetVectorArray` 获得每个实例的 `uvScaleOffset`，用它覆盖最终的采样坐标：

```
finalUV = quadUV * uvScaleOffset.xy + uvScaleOffset.zw
```

shareGrid Quad 的原始 UV 是标准的 `[0,1]` 范围，经过这个公式变换后，结果与原 Tile Mesh UV 指向的图集子区域完全相同。**Mesh 的 UV 顶点对渲染结果没有任何影响。**

### 前提三：所有 Tile Prefab 和 shareGrid 的几何形状等价

所有地形 Tile 都是**平面四边形（Quad）**，顶点布局相同，只有 UV 和缩放上的差异。前两个前提已经说明 UV 和缩放都通过其他渠道处理，因此几何上的等价使得替换透明。

### 前提四：地图块不使用 meshScale / meshRotation

对比信息层（`SLGAreaInfoLayer`），它的格子矩阵是从逻辑坐标动态计算的，需要 `CalcInitScaleMatrix(res.meshScale, res.meshRotation)` 将 Prefab 的原始尺寸折算成格子单位的缩放因子：

```csharp
// 信息层：meshScale 用于校正格子渲染尺寸
scale.x = meshScale.x / SLGDefine.s_SLG_Grid_UnitSize;
m_InitScaleMatrix = Matrix4x4.TRS(Vector3.zero, meshRotation, scale);
```

地图块（`SLGAreaMapBlock`）**没有这一步**——它的矩阵在导出时已经包含了完整的世界变换，`meshScale` 在地图块渲染路径中完全不参与计算。

### 结论

| 信息 | 存储位置 | 与 Mesh 的关系 |
|---|---|---|
| 世界位置 / 旋转 | per-instance Matrix4x4 | 独立，与 Mesh 无关 |
| 渲染尺寸（scale）| per-instance Matrix4x4（已烘入）| 独立，与 Mesh 无关 |
| 图集子区域（UV）| per-instance uvScaleOffset | 在导出时从 Mesh UV 提取，之后与 Mesh 无关 |
| 纹理 / 渲染队列 | 材质（mat）| 与 Mesh 无关 |
| 几何形状 | Mesh | **唯一依赖**——必须是 Quad |

只要 Mesh 是 Quad，所有决定最终像素颜色和位置的信息都已独立存储。替换成任意同形状的 Quad（即 shareGrid）不影响渲染结果，同时所有实例共享同一个 Mesh 对象，减少内存占用并让 GPU 驱动层更容易优化。

---

## 8) 关键约束

| 约束 | 说明 |
|---|---|
| Tile Mesh 必须是 4 顶点 Quad | `CalcUVScaleOffsetByMesh` 要求 `mesh.uv.Length == 4` |
| 同材质 Tile 共享 shareGrid Mesh | 所有实例几何相同，差异仅在 per-instance uvScaleOffset |
| 每个 Block 最多 1023 个实例 | `Graphics.DrawMeshInstanced` Unity API 限制 |
| 相同材质首次注册的 prefabPath 存入 DB | 后续同材质 prefab 不再注册，仅追加实例数据 |

---

[返回技术总览](../README.md)
