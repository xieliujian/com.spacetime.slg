# 架构总览

[返回技术总览](../README.md)

## 模块划分

```
com.spacetime.slg/
├── Runtime/Scripts/
│   ├── Common/        # 常量定义、工具函数
│   ├── DB/            # 纯数据层（ScriptableObject）
│   └── Logic/         # 运行时逻辑与渲染
├── Editor/Scripts/
│   ├── Define/        # 编辑器全局常量（SLGEditDefine）
│   ├── Config/        # 图层配置（SLGLayerConfig / SLGLayerConfigMgr）
│   ├── RenderGridEdit/    # 渲染网格编辑工具
│   ├── PropertyGridEdit/  # 属性网格编辑工具
│   ├── DynamicObjGroupEdit/ # 动态对象组编辑工具
│   ├── Inspector/     # 自定义 Inspector
│   ├── Utils/         # 编辑器通用工具函数（SLGEditUtils）
│   └── Tools/         # 独立辅助工具（HexColor Inspector 等）
└── Shaders/           # 自定义 Shader
```

---

## 组件层级

```
SLGSceneMgr  (单例，主入口)
├── SLGScene
│   ├── SLGAreaSet
│   │   └── SLGArea × 25
│   │       ├── SLGAreaMapLayerSet
│   │       │   └── SLGAreaMapLayer × N
│   │       │       └── SLGAreaMapBlock × N   ← GPU Instancing 渲染块
│   │       ├── SLGAreaDynamicMapLayerSet
│   │       │   └── SLGAreaDynamicMapLayer × N
│   │       └── SLGAreaInfoLayerSet
│   │           ├── SLGAreaGridInfoLayer × N  ← 网格信息叠加层
│   │           └── SLGAreaPropertyInfoLayer  ← 属性信息叠加层
│   ├── SLGSceneProperty                      ← 全局网格属性查询
│   └── SLGSceneLineLayer                     ← 场景线段渲染
│       └── SLGSceneLineBlock × N
└── SLGResMgr                                 ← 资源加载与缓存
```

---

## 单例入口

### SLGSceneMgr

所有运行时操作的唯一入口，通过 `SLGSceneMgr.Instance` 访问。

| 方法 | 说明 |
|------|------|
| `Init(SLGSceneDB)` | 初始化场景，传入场景数据库 |
| `Update()` | 每帧调用，驱动视锥剔除与渲染提交 |
| `Destroy()` | 销毁场景，释放资源 |
| `FindGridProperty(Vector2Int)` | 查询指定逻辑坐标的网格属性 |
| `AddAreaGridInfo / RemoveAreaGridInfo` | 添加/移除信息图层着色 |
| `SetAreaPropertyLayerVisible` | 控制属性图层可见性 |
| `AddSceneLineInfo / RemoveSceneLineInfo` | 添加/移除场景线段 |
| `SubmitGPUByLayer` | 批量提交指定图层的 GPU 数据 |
| `FillMiniMapTexture` | 生成小地图纹理 |
| `SetDynamicMapIndex` | 切换动态对象可见状态 |

### SLGSceneMgrMono

挂载到 GameObject 上，在 `LateUpdate` 自动调用 `SLGSceneMgr.Instance.Update()`，无需手动驱动。

---

## 引擎抽象层（SLGWarp）

`SLGWarp` 单例通过 `ISLGWarp` 接口将 Package 与宿主引擎解耦，提供：

- **Camera 访问**：`GetCamera()`
- **资源加载**：`LoadAsset<T>()` / `UnloadAsset()`
- **玩家位置**：`GetPlayerPos()`

默认实现为 `SLGWarpInternal`（直接使用 `Camera.main` 和 `Resources.Load`）。  
自定义接入时实现 `ISLGWarp` 并调用 `SLGWarp.Instance.SetWarp(impl)`。

---

## 调试支持

`SLGSceneMgrDebug` 提供运行时调试命令（需 `DEBUG_MODE` 宏）：

- 测试指定坐标的网格信息图层
- 测试场景线段绘制
- GPU Skin 实例化压力测试

---

[返回技术总览](../README.md)
