using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 编辑器全局常量定义：汇总 Editor 脚本中所有常量与只读配置，统一管理路径、节点名、Excel 配置等。
    /// </summary>
    public static class SLGEditDefine
    {
        // ── Excel / Table ──────────────────────────────────────────────────

        /// <summary>
        /// 默认层配置 Excel 的绝对文件路径（依赖 Application.dataPath，运行时求值）。
        /// </summary>
        public static readonly string SLG_LAYER_CONFIG_ABSOLUTE_PATH =
            Application.dataPath + "/../Packages/com.spacetime.slg/Editor/Excel/SLGLayer_WPS.xlsx";

        /// <summary>
        /// Excel 中英文列名所在行索引（0-based），第 1 行为英文字段名。
        /// </summary>
        public const int TABLE_HEADER_ROW_INDEX = 1;

        /// <summary>
        /// Excel 中有效数据起始行索引（0-based），跳过中文说明行与英文列名行。
        /// </summary>
        public const int TABLE_DATA_ROW_START_INDEX = 2;

        // ── 路径与文件扩展名 ──────────────────────────────────────────────

        /// <summary>
        /// 逻辑/信息层预制体资源路径前缀（相对于 Assets 目录）。
        /// </summary>
        public const string SLG_INFO_PREFAB_PATH_PREFIX = "Assets/scene/common/res/slg/logicprefab/";

        /// <summary>
        /// 全局共用的 SLG 共享格预制体名（无扩展名）。
        /// </summary>
        public const string SLG_SHARE_GRID_PREFAB_NAME = "slg_share_grid";

        /// <summary>
        /// Unity 预制体资源文件扩展名。
        /// </summary>
        public const string PREFAB_SUFFIX = ".prefab";

        /// <summary>
        /// Unity 场景文件扩展名。
        /// </summary>
        public const string SCENE_SUFFIX = ".unity";

        /// <summary>
        /// SLG 场景数据资源文件名后缀（拼接于场景名之后）。
        /// </summary>
        public const string SLG_SCENE_SUFFIX_NAME = "_SLG";

        /// <summary>
        /// Unity ScriptableObject 资源文件扩展名。
        /// </summary>
        public const string ASSET_SUFFIX = ".asset";

        // ── 场景根节点名 ──────────────────────────────────────────────────

        /// <summary>
        /// 场景中 SLG 渲染根节点的 GameObject 名称（挂有 Grid 组件）。
        /// </summary>
        public const string SLG_RENDER_ROOT_NAME = "SLGRenderGrid";

        /// <summary>
        /// 场景中 SLG 属性根节点的 GameObject 名称（挂有 Grid 组件）。
        /// </summary>
        public const string SLG_PROPERTY_ROOT_NAME = "SLGPropertyGrid";

        // ── 属性层节点名与 Prefab 前缀 ───────────────────────────────────

        /// <summary>
        /// 资源等级属性层对应的 Prefab 名称前缀（后接等级数字，例如 slg_reslvproperty_1）。
        /// </summary>
        public const string SLG_RES_LV_PROPERTY_PREFAB_PREFIX = "slg_reslvproperty_";

        /// <summary>
        /// 资源等级属性层在场景树中的子节点名。
        /// </summary>
        public const string SLG_EDITLAYER_RES_LV_PROPERTY_NODE_NAME = "ResLvPropertyLayer";

        /// <summary>
        /// 选择类属性层在场景树中的子节点名。
        /// </summary>
        public const string SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME = "SelPropertyLayer";

        /// <summary>
        /// 选择属性格三种状态的 Prefab 名称数组：[0] 可选、[1] 不可选、[2] 动态。
        /// </summary>
        public static readonly string[] s_SLGSelPropPrefabArray =
        {
            "slg_selproperty_sel",
            "slg_selproperty_unsel",
            "slg_selproperty_dynamic"
        };

        // ── 导出文件名 ────────────────────────────────────────────────────

#if ART_SCENE_PROJECT
        /// <summary>
        /// 属性格 Excel 导出文件名（仅 ART_SCENE_PROJECT 下启用）。
        /// </summary>
        public static readonly string SLG_PROPERTY_EXCEL_FILENAME = "slg_scene_property.xlsx";
#endif

        // ── 摄像机 Inspector ──────────────────────────────────────────────

        /// <summary>
        /// 编辑器中滚轮模拟按钮的单次滚动量。
        /// </summary>
        public const float SLG_CAMERA_SCROLL_UNIT = 0.01f;

        /// <summary>
        /// 编辑器中 W/S 按钮的单次垂直移动量。
        /// </summary>
        public const float SLG_CAMERA_VERTICAL_UNIT = 0.1f;

        /// <summary>
        /// 编辑器中 A/D 按钮的单次水平移动量。
        /// </summary>
        public const float SLG_CAMERA_HORIZONTAL_UNIT = 0.1f;
    }
}
