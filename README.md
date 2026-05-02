# com.spacetime.slg

**SpaceTime SLG** 是一个 Unity 策略地图（SLG）场景系统 Package，提供基于网格的地形渲染、信息图层叠加、场景线段绘制、动态对象管理以及配套的 Editor 工具链。

- Unity 版本：2020.3+
- 命名空间：`ST.SLG`
- Package 名：`com.spacetime.slg`，版本 `1.0.0`

---

## 技术文档（核心入口）

> 建议先读这 4 篇，建立整体心智模型；其余专题再按需展开。

| 文档 | 说明 |
|------|------|
| [架构总览](readme/architecture.md) | 模块划分、组件层级、单例入口、引擎抽象层 |
| [网格坐标系统](readme/grid-system.md) | 逻辑坐标、世界坐标、Area 分区、常量定义 |
| [渲染系统](readme/rendering.md) | 地图层渲染、信息图层、场景线段、GPU Instancing、视锥剔除 |
| [Editor 工具](readme/editor-tools.md) | 属性网格编辑、渲染网格编辑、动态对象组、场景流式导入导出 |

---

## 核心设计（必读）

> 记录全项目最核心、需要结合代码理解的 Why 与关键约束；建议按 01 → 03 顺序阅读。

| 优先级 | 文档 | 说明 |
|------|------|------|
| P0 | [01 地图层 GPU Instancing 合批设计](readme/map-layer-instancing.md) | 地图层资源如何按材质合批、UV 子区域提取、shareGrid Mesh 替换策略及其成立前提 |
| P0 | [02 信息层线段同屏 N 条线段设计](readme/scene-line-layer.md) | 线段层如何通过 ID 映射、300 槽位分块与 Instancing 支持同屏 N 条线段 |
| P0 | [03 信息格子层设计](readme/info-grid-layer-design.md) | 信息格子层的双路径数据模型（运行时写入 vs 预计算入库）与 prefab 旋转支持 |

---

## 技术专题

| 文档 | 说明 |
|------|------|
| [数据层（DB）](readme/db-layer.md) | ScriptableObject 数据结构、序列化关系 |
| [场景流化数据](readme/streaming.md) | 流化导出流程、DB 数据结构、资源路径转换、与运行时的边界 |

---

## 快速接入

```csharp
// 1. 实现 ISLGWarp 并注册（可选，不注册则使用内置默认实现）
SLGWarp.Instance.SetWarp(new MyWarpImpl());

// 2. 初始化场景
SLGSceneMgr.Instance.Init(sceneDB);

// 3. 每帧驱动（或挂载 SLGSceneMgrMono 自动驱动）
void LateUpdate() => SLGSceneMgr.Instance.Update();

// 4. 销毁
SLGSceneMgr.Instance.Destroy();
```

## Assembly Definitions

| asmdef | 说明 |
|--------|------|
| `com.spacetime.slg.runtime` | 运行时，仅依赖 Unity 核心 |
| `com.spacetime.slg.editor` | Editor 工具，依赖 runtime |
| `com.spacetime.slg.shaders` | Shader 资源 |
