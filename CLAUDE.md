# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This is a Unity package (`com.spacetime.slg`, namespace `ST.SLG`) implementing a grid-based SLG (Strategy/Simulation) scene system. It targets Unity 2020.3+ and is structured as three assembly definitions: runtime, editor, and shaders.

## Build & Development

This is a Unity package — there are no standalone build commands. Development happens inside a Unity project that references this package. The three assembly definitions compile independently:

- `com.spacetime.slg.runtime` — runtime-only, no editor dependencies
- `com.spacetime.slg.editor` — editor tools, references runtime
- `com.spacetime.slg.shaders` — shader assets

To validate changes, open the host Unity project and check the Console for compile errors. There are no automated tests.

## Architecture

### Entry Point

`SLGSceneMgr` is the singleton entry point for all runtime usage. Call `Init(SLGSceneDB)` once, `Update()` each frame, and `Destroy()` on teardown. All public API lives here.

### Component Hierarchy

```
SLGSceneMgr (singleton)
├── SLGScene
│   ├── SLGAreaSet — 25 areas (5×5 grid)
│   │   └── SLGArea[0-24]
│   │       ├── SLGAreaMapLayer — terrain rendering
│   │       ├── SLGAreaInfoLayerSet — colored info overlays
│   │       └── SLGAreaPropertyInfoLayer
│   ├── SLGSceneProperty — per-grid property queries
│   └── SLGSceneLineLayer — line rendering between positions
└── SLGResMgr — resource loading
```

### Engine Abstraction

`SLGWarp` (singleton, interface `ISLGWarp`) decouples the package from the host engine. It provides camera access, resource loading, and player position. Override `SLGWarpInternal` to integrate with a custom engine or mock in tests.

### Code Style Rules

- Keep namespace as `ST.SLG`.
- In `SLGDefine`, use `public static readonly int` with `s_SLG_...` naming (example: `s_SLG_Grid_UnitSize`), not `const` + ALL_CAPS style.
- Keep Shader property IDs in `SLGDefine` as `public static readonly int s_SLG_Shader_...`.
- All public classes, methods, fields, and enums must have `/// <summary>` XML doc comments in Chinese.
- Private/internal implementation fields and methods: no comments unless the logic is non-obvious.
- Do not write empty `/// <summary>` blocks (no blank summaries).

### readme Sub-documents

- Every file under `readme/` must start with `[返回技术总览](../README.md)` on the second line (after the `#` heading) and end with `---\n\n[返回技术总览](../README.md)`.

### Grid Constants (SLGDefine)

| Constant | Value | Meaning |
|---|---|---|
| Grid unit size | 4 units | One logic grid = 4×4 world units |
| Map size | 50×50 grids | 200×200 world units total |
| Area size | 10×10 grids | 40×40 world units per area |
| Total areas | 5×5 = 25 | Indexed 0–24 |
| Logic offset | (26, 26) | Centers the coordinate origin |

### Coordinate System

Use `SLGUtils` for all coordinate conversions — never compute manually:

- `ConvertSLG3DPosToLogicPos(Vector3)` — world → logic grid
- `ConvertSLGLogicPosTo3DPos(Vector2Int)` — logic grid → world
- `CalcAreaIndexByLogicPos(Vector2Int)` — which of the 25 areas
- `CalcAreaGridIndexByLogicPos(Vector2Int)` — grid index within area (0–99)

### Info Layers

`SLGDefine.SLGInfoLayer` enum defines overlay types (CampInfo=5, FireInfo=6, AreaTargetPos=7, AreaWayPoint=8, SceneLine=9, ResLvProperty=10). Use `SLGSceneMgr.AddAreaGridInfo` / `RemoveAreaGridInfo` / `SetAreaPropertyLayerVisible` to manage them. Call `SubmitGPUByLayer` after batch updates.

### DB Layer

`Runtime/Scripts/DB/` contains pure data models (no MonoBehaviour). `SLGSceneDB` is the top-level config passed to `Init()`. `SLGSceneResDB` holds resource references.

### Editor Tools

`Editor/Scripts/` provides Unity Editor windows for authoring scene data: property grid editing, render grid editing, dynamic object group editing, layer config, and Excel import. These are standalone editor windows, not inspectors for runtime components (except those under `Inspector/`).
