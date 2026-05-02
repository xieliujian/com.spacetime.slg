using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 全局常量与枚举定义类
    /// </summary>
    public class SLGDefine
    {
        /// <summary>
        /// 单个格子的世界单位尺寸
        /// </summary>
        public static readonly int s_SLG_Grid_UnitSize = 4;

        /// <summary>
        /// 地图水平方向格子总数
        /// </summary>
        public static readonly int s_SLG_Grid_HorizontalNum = 50;

        /// <summary>
        /// 地图垂直方向格子总数
        /// </summary>
        public static readonly int s_SLG_Grid_VerticalNum = 50;

        /// <summary>
        /// 地图水平方向世界尺寸（格子数 × 单格尺寸）
        /// </summary>
        public static readonly int s_SLG_Map_HorizontalSize = s_SLG_Grid_HorizontalNum * s_SLG_Grid_UnitSize;

        /// <summary>
        /// 地图垂直方向世界尺寸（格子数 × 单格尺寸）
        /// </summary>
        public static readonly int s_SLG_Map_VerticalSize = s_SLG_Grid_VerticalNum * s_SLG_Grid_UnitSize;

        /// <summary>
        /// 逻辑坐标水平方向偏移量，用于将世界坐标转换为逻辑坐标
        /// </summary>
        public static readonly int s_SLG_LogicGrid_HorizontalOffset = s_SLG_Grid_HorizontalNum / 2 + 1;

        /// <summary>
        /// 逻辑坐标垂直方向偏移量，用于将世界坐标转换为逻辑坐标
        /// </summary>
        public static readonly int s_SLG_LogicGrid_VerticalOffset = s_SLG_Grid_VerticalNum / 2 + 1;

        /// <summary>
        /// 单个区域水平方向世界尺寸
        /// </summary>
        public static readonly int s_SLG_Area_HorizontalSize = 40;

        /// <summary>
        /// 单个区域垂直方向世界尺寸
        /// </summary>
        public static readonly int s_SLG_Area_VerticalSize = 40;

        /// <summary>
        /// 单个区域水平方向格子数
        /// </summary>
        public static readonly int s_SLG_Area_HorizontalGridNum = s_SLG_Area_HorizontalSize / s_SLG_Grid_UnitSize;

        /// <summary>
        /// 单个区域垂直方向格子数
        /// </summary>
        public static readonly int s_SLG_Area_VerticalGridNum = s_SLG_Area_VerticalSize / s_SLG_Grid_UnitSize;

        /// <summary>
        /// 单个区域内格子总数
        /// </summary>
        public static readonly int s_SLG_Area_TotalGridNum = s_SLG_Area_HorizontalGridNum * s_SLG_Area_VerticalGridNum;

        /// <summary>
        /// 地图水平方向区域总数
        /// </summary>
        public static readonly int s_SLG_Area_HorizontalNum = s_SLG_Grid_HorizontalNum * s_SLG_Grid_UnitSize / s_SLG_Area_HorizontalSize;

        /// <summary>
        /// 地图垂直方向区域总数
        /// </summary>
        public static readonly int s_SLG_Area_VerticalNum = s_SLG_Grid_VerticalNum * s_SLG_Grid_UnitSize / s_SLG_Area_VerticalSize;

        /// <summary>
        /// 小地图中每个格子对应的像素数
        /// </summary>
        public static readonly int s_SLG_MiniMap_GridPixelCount = 1;

        /// <summary>
        /// 小地图纹理宽度（像素）
        /// </summary>
        public static readonly int s_SLG_MiniMap_TexWidth = s_SLG_Grid_HorizontalNum * s_SLG_MiniMap_GridPixelCount;

        /// <summary>
        /// 小地图纹理高度（像素）
        /// </summary>
        public static readonly int s_SLG_MiniMap_TexHeight = s_SLG_Grid_VerticalNum * s_SLG_MiniMap_GridPixelCount;

        /// <summary>
        /// SLG 逻辑预制体相对路径（不含 Assets/ 前缀，用于跨环境资源加载）
        /// </summary>
        public const string SLG_LOGIC_PREFAB_PATH = "scene/common/res/slg/logicprefab/";

        /// <summary>
        /// 每帧存储的图层数据，该枚举尽量不要改变，便于优化缓存
        /// </summary>
        public enum SLGInfoLayer
        {
            CampInfo = 5,
            FireInfo,
            AreaTargetPos,
            AreaWayPoint,
            SceneLine,
            ResLvProperty
        }

        /// <summary>
        /// 信息图层类型，区分区域格子与场景线段两种渲染层
        /// </summary>
        public enum SLGInfoLayerType
        {
            Invalid = -1,        // 无效
            AreaGrid,           // 区域格子图层
            SceneLine           // 场景线段图层
        }

        /// <summary>
        /// 区域格子属性图层类型，用于区分选中状态与资源等级状态
        /// </summary>
        public enum SLGAreaGridPropertyLayerType
        {
            Invalid = -1,        // 无效
            SelState,           // 选中状态
            ResLvState          // 资源等级状态
        }

        // ── 摄像机输入轴名称 ──────────────────────────────────────────────

        /// <summary>Unity Input 水平轴名称。</summary>
        public static readonly string s_SLGCamera_Input_Horizontal = "Horizontal";

        /// <summary>Unity Input 垂直轴名称。</summary>
        public static readonly string s_SLGCamera_Input_Vertical = "Vertical";

        /// <summary>Unity Input 鼠标滚轮轴名称。</summary>
        public static readonly string s_SLGCamera_Input_MouseScrollWheel = "Mouse ScrollWheel";

        // ── 摄像机初始位置 ────────────────────────────────────────────────

        /// <summary>摄像机初始位置 X 分量。</summary>
        public static readonly float s_SLGCamera_InitPosX = 0f;

        /// <summary>摄像机初始位置 Z 分量。</summary>
        public static readonly float s_SLGCamera_InitPosZ = 0f;

        // ── SLGSceneCamera 默认参数 ───────────────────────────────────────

        /// <summary>场景预览摄像机视野角（度）。</summary>
        public static readonly float s_SLGSceneCamera_Fov = 5f;

        /// <summary>场景预览摄像机远裁剪面距离。</summary>
        public static readonly float s_SLGSceneCamera_Far = 2500f;

        /// <summary>场景预览摄像机固定欧拉角 X 分量（俯仰角）。</summary>
        public static readonly float s_SLGSceneCamera_EulerX = 40f;

        /// <summary>场景预览摄像机固定欧拉角 Y 分量（偏航角）。</summary>
        public static readonly float s_SLGSceneCamera_EulerY = 45f;

        /// <summary>场景预览摄像机允许的最小世界高度。</summary>
        public static readonly float s_SLGSceneCamera_MinHeight = 200f;

        /// <summary>场景预览摄像机允许的最大世界高度。</summary>
        public static readonly float s_SLGSceneCamera_MaxHeight = 350f;

        /// <summary>场景预览摄像机平移速度。</summary>
        public static readonly float s_SLGSceneCamera_MoveSpeed = 50f;

        /// <summary>场景预览摄像机滚轮缩放速度。</summary>
        public static readonly float s_SLGSceneCamera_WheelSpeed = 1f;

        // ── SLGGameCamera 默认参数 ────────────────────────────────────────


        /// <summary>游戏运行时摄像机视野角（度）。</summary>
        public static readonly float s_SLGGameCamera_Fov = 15f;

        /// <summary>游戏运行时摄像机近裁剪面距离。</summary>
        public static readonly float s_SLGGameCamera_Near = 0.3f;

        /// <summary>游戏运行时摄像机远裁剪面距离。</summary>
        public static readonly float s_SLGGameCamera_Far = 2500f;

        /// <summary>游戏运行时摄像机固定欧拉角 X 分量（俯仰角）。</summary>
        public static readonly float s_SLGGameCamera_EulerX = 39.6f;

        /// <summary>游戏运行时摄像机固定欧拉角 Y 分量（偏航角）。</summary>
        public static readonly float s_SLGGameCamera_EulerY = 45f;

        /// <summary>游戏运行时摄像机允许的最小世界高度。</summary>
        public static readonly float s_SLGGameCamera_MinHeight = 50f;

        /// <summary>游戏运行时摄像机允许的最大世界高度。</summary>
        public static readonly float s_SLGGameCamera_MaxHeight = 300f;

        /// <summary>游戏运行时摄像机平移速度。</summary>
        public static readonly float s_SLGGameCamera_MoveSpeed = 50f;

        /// <summary>游戏运行时摄像机滚轮缩放速度。</summary>
        public static readonly float s_SLGGameCamera_WheelSpeed = 1f;

        /// <summary>游戏运行时摄像机鼠标拖拽平移速度。</summary>
        public static readonly float s_SLGGameCamera_DragSpeed = 0.5f;

        /// <summary>游戏运行时摄像机拖拽所用的鼠标按键（0=左键，1=右键，2=中键）。</summary>
        public static readonly int s_SLGGameCamera_DragMouseButton = 1;

        /// <summary>
        /// 通用无参无返回值委托，用于 SLG 模块内的回调传递
        /// </summary>
        public delegate void SLGRun();

        /// <summary>
        /// Shader 标志位属性 ID（_SLGShaderFlag）
        /// </summary>
        public static readonly int s_SLG_Shader_FlagId = Shader.PropertyToID("_SLGShaderFlag");

        /// <summary>
        /// 场景物体基础颜色 Shader 属性 ID（_BaseColor）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneObj_BaseColorId = Shader.PropertyToID("_BaseColor");
        /// <summary>
        /// 场景物体 UV 缩放偏移 Shader 属性 ID（_UVScaleOffset）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneObj_UvScaleOffsetId = Shader.PropertyToID("_UVScaleOffset");
        /// <summary>
        /// 场景物体 Z 裁剪 Shader 属性 ID（_ZClip）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneObj_ZClipId = Shader.PropertyToID("_ZClip");
        /// <summary>
        /// 场景物体 Z 测试 Shader 属性 ID（_ZTest）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneObj_ZTestId = Shader.PropertyToID("_ZTest");
        /// <summary>
        /// 场景物体 Z 写入 Shader 属性 ID（_ZWrite）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneObj_ZWriteId = Shader.PropertyToID("_ZWrite");

        /// <summary>
        /// 场景线段敌方标识 Shader 属性 ID（_Enemy）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneLine_EnemyId = Shader.PropertyToID("_Enemy");
        /// <summary>
        /// 场景线段 UV 缩放偏移 Shader 属性 ID（_UVScaleOffset）
        /// </summary>
        public static readonly int s_SLG_Shader_SceneLine_UvScaleOffsetId = Shader.PropertyToID("_UVScaleOffset");
    }
}

