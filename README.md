# com.spacetime.slg

**SpaceTime SLG** 是一个 Unity 策略地图（SLG）场景系统 Package，提供基于网格的地形渲染、信息图层叠加、场景线段绘制、动态对象管理以及配套的 Editor 工具链。

- Unity 版本：2020.3+
- 命名空间：`ST.SLG`
- Package 名：`com.spacetime.slg`，版本 `1.0.0`

---

## 技术文档

| 文档 | 说明 |
|------|------|
| [架构总览](readme/architecture.md) | 模块划分、组件层级、单例入口、引擎抽象层 |
| [网格坐标系统](readme/grid-system.md) | 逻辑坐标、世界坐标、Area 分区、常量定义 |
| [数据层（DB）](readme/db-layer.md) | ScriptableObject 数据结构、序列化关系 |
| [渲染系统](readme/rendering.md) | 地图层渲染、信息图层、场景线段、GPU Instancing、视锥剔除 |
| [Editor 工具](readme/editor-tools.md) | 属性网格编辑、渲染网格编辑、动态对象组、场景流式导入导出 |

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
