# 网格坐标系统

[返回技术总览](../README.md)

## 核心常量（SLGDefine）

| 常量 | 值 | 含义 |
|------|----|------|
| `GRID_UNIT_SIZE` | 4 | 一个逻辑格 = 4×4 世界单位 |
| `MAP_GRID_WIDTH / HEIGHT` | 50 × 50 | 全图逻辑格数量 |
| `MAP_UNIT_WIDTH / HEIGHT` | 200 × 200 | 全图世界单位尺寸 |
| `AREA_GRID_WIDTH / HEIGHT` | 10 × 10 | 每个 Area 的逻辑格数量 |
| `AREA_UNIT_WIDTH / HEIGHT` | 40 × 40 | 每个 Area 的世界单位尺寸 |
| `AREA_COUNT_X / Y` | 5 × 5 | Area 总数（共 25 个） |
| `LOGIC_GRID_OFFSET` | (26, 26) | 逻辑坐标原点偏移，用于居中 |
| `MINI_MAP_SIZE` | 50 × 50 | 小地图像素尺寸（1 像素 = 1 格） |

---

## 坐标系说明

系统存在两套坐标：

- **世界坐标（Vector3）**：Unity 场景中的 3D 坐标，XZ 平面为地面
- **逻辑坐标（Vector2Int）**：以格为单位的整数坐标，原点经 `LOGIC_GRID_OFFSET` 偏移居中

```
逻辑坐标 (0,0) 对应世界坐标 (-26*4, 0, -26*4) = (-104, 0, -104)
逻辑坐标 (49,49) 对应世界坐标约 (96, 0, 96)
```

---

## 坐标转换（SLGUtils）

```csharp
// 世界坐标 → 逻辑坐标
Vector2Int logicPos = SLGUtils.ConvertSLG3DPosToLogicPos(worldPos);

// 逻辑坐标 → 世界坐标（格中心）
Vector3 worldPos = SLGUtils.ConvertSLGLogicPosTo3DPos(logicPos);

// 获取格的变换矩阵（用于 GPU Instancing）
Matrix4x4 mat = SLGUtils.CalcSLGGridMatrix(logicPos);
```

---

## Area 分区

全图 50×50 格被划分为 5×5 = **25 个 Area**，每个 Area 包含 10×10 = 100 格。

```csharp
// 逻辑坐标 → Area 索引 [0, 24]
int areaIndex = SLGUtils.CalcAreaIndexByLogicPos(logicPos);

// 逻辑坐标 → Area 内格索引 [0, 99]
int gridInArea = SLGUtils.CalcAreaGridIndexByLogicPos(logicPos);

// 逻辑坐标 → 全图格索引（用于属性查询）
int propIndex = SLGUtils.CalcPropertyGridIndex(logicPos);
```

Area 索引排列（行优先，X 为列，Y 为行）：

```
 0  1  2  3  4
 5  6  7  8  9
10 11 12 13 14
15 16 17 18 19
20 21 22 23 24
```

---

## 信息图层类型（SLGInfoLayer）

定义在 `SLGDefine.SLGInfoLayer` 枚举：

| 枚举值 | 整数 | 用途 |
|--------|------|------|
| `CampInfo` | 5 | 阵营着色 |
| `FireInfo` | 6 | 火力范围 |
| `AreaTargetPos` | 7 | 目标位置标记 |
| `AreaWayPoint` | 8 | 路径点标记 |
| `SceneLine` | 9 | 场景线段（连线） |
| `ResLvProperty` | 10 | 资源等级属性 |

---

[返回技术总览](../README.md)
