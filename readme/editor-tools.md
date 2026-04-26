# Editor 工具

[返回技术总览](../README.md)

`Editor/Scripts/` 提供一套围绕 SLG 场景数据的可视化编辑工具，主要分为渲染网格编辑、属性网格编辑和动态对象组管理。

---

## 1) 渲染网格编辑（RenderGridEdit）

核心类：
- `SLGRenderGridEdit`
- `SLGRenderGridEdit_Streamer`

作用：
- 编辑地形渲染格（Map Layer）
- 将场景对象采样并写入 `SLGAreaMapLayerSetDB` / `SLGAreaMapBlockDB`
- 支持场景流式导入导出（Streamer）

产出：
- Area 维度的渲染层 DB 数据，供运行时 GPU Instancing 直接使用

---

## 2) 属性网格编辑（PropertyGridEdit）

核心类（partial 拆分）：
- `SLGPropertyGridEdit`
- `SLGPropertyGridEdit_SelProp`
- `SLGPropertyGridEdit_ResLv`
- `SLGPropertyGridEdit_Excel`
- `SLGPropertyGridEdit_Streamer`

作用：
- 编辑 `SLGPropertyGridDB` 的两类属性：
  - 选中属性（`SelectPropertyType`）
  - 资源等级（`ResLvType`）
- 支持将属性数据导出到 Excel（`ART_SCENE_PROJECT` 宏下启用）
- 支持流式导入导出

产出：
- `SLGScenePropDB` 与属性信息图层相关 DB

---

## 3) 动态对象组编辑（DynamicObjGroupEdit）

核心类：
- `SLGDynamicObjGroupEdit`

作用：
- 在场景中创建/维护动态对象分组结构
- 运行时通过 `SetDynamicMapIndex(index)` 批量切换对象组显隐

---

## 4) 图层配置（Config）

核心类：
- `SLGLayerConfig`
- `SLGLayerConfigMgr`

数据源：
- `Editor/Excel/SLGLayer_WPS.csv`

作用：
- 定义渲染层和信息层配置（索引、名称、颜色等）
- 编辑器工具读取配置，保持图层编辑行为一致

---

## 5) 场景流式支持

核心类：
- `SLGSceneStreamer`

作用：
- 与 Render/Property 编辑器联动
- 在大场景下按区域导入/导出场景数据，避免一次性全量处理

---

## 6) 辅助工具

- `SLGEditUtils`：编辑态通用工具函数
- `SLGTools_HexColorInspector`：HexColor 工具自定义 Inspector

---

## Editor 与 Runtime 的边界

- **Editor 负责生产数据**（ScriptableObject DB）
- **Runtime 负责消费数据**（初始化后只读渲染）

这意味着运行时逻辑通常不直接依赖 Unity Editor API，数据结构变更需要同步更新 Editor 导出流程与 Runtime 读取流程。

---

[返回技术总览](../README.md)
