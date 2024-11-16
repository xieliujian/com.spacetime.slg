using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGDefine
    {
        /// <summary>
        /// 
        /// </summary>
        public const int SLG_GRID_UNIT_SIZE = 4;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_GRID_HORIZONTAL_NUM = 50;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_GRID_VERTICAL_NUM = 50;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_MAP_HORIZONTAL_SIZE = SLG_GRID_HORIZONTAL_NUM * SLG_GRID_UNIT_SIZE;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_MAP_VERTICAL_SIZE = SLG_GRID_VERTICAL_NUM * SLG_GRID_UNIT_SIZE;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_LOGIC_GRID_HORIZONTAL_OFFSET = SLG_GRID_HORIZONTAL_NUM / 2 + 1;

        /// <summary>
        /// 
        /// </summary>
        public const int SLG_LOGIC_GRID_VERTICAL_OFFSET = SLG_GRID_VERTICAL_NUM / 2 + 1;

        /// <summary>
        /// 尺寸是4的倍数
        /// </summary>
        public const int SLG_AREA_HORIZONTAL_SIZE = 40;

        /// <summary>
        /// 尺寸是4的倍数
        /// </summary>
        public const int SLG_AREA_VERTICAL_SIZE = 40;

        /// <summary>
        /// 区域水平格子数目
        /// </summary>
        public const int SLG_AREA_HORIZONTAL_GRID_NUM = SLG_AREA_HORIZONTAL_SIZE / SLG_GRID_UNIT_SIZE;

        /// <summary>
        /// 区域垂直格子数目
        /// </summary>
        public const int SLG_AREA_VERTICAL_GRID_NUM = SLG_AREA_VERTICAL_SIZE / SLG_GRID_UNIT_SIZE;

        /// <summary>
        /// 区域总的格子数目
        /// </summary>
        public const int SLG_AREA_TOTAL_GRID_NUM = SLG_AREA_HORIZONTAL_GRID_NUM * SLG_AREA_VERTICAL_GRID_NUM;

        /// <summary>
        /// 水平区域数目
        /// </summary>
        public const int SLG_AREA_HORIZONTAL_NUM = SLG_GRID_HORIZONTAL_NUM * SLG_GRID_UNIT_SIZE / SLG_AREA_HORIZONTAL_SIZE;

        /// <summary>
        /// 垂直区域数目
        /// </summary>
        public const int SLG_AREA_VERTICAL_NUM = SLG_GRID_VERTICAL_NUM * SLG_GRID_UNIT_SIZE / SLG_AREA_VERTICAL_SIZE;

        /// <summary>
        /// 小地图一个区块占用的像素数目
        /// </summary>
        public const int SLG_MINIMAP_GRID_PIXEL_COUNT = 1;

        /// <summary>
        /// 小地图贴图宽度
        /// </summary>
        public const int SLG_MINIMAP_TEX_WIDTH = SLG_GRID_HORIZONTAL_NUM * SLG_MINIMAP_GRID_PIXEL_COUNT;

        /// <summary>
        /// 小地图贴图高度
        /// </summary>
        public const int SLG_MINIMAP_TEX_HEIGHT = SLG_GRID_VERTICAL_NUM * SLG_MINIMAP_GRID_PIXEL_COUNT;

        /// <summary>
        /// 每次表格添加数据，这个枚举就要变化（待优化）
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
        /// 
        /// </summary>
        public enum SLGInfoLayerType
        {
            Invalid = -1,        // 无效
            AreaGrid,           // 区域格子类型
            SceneLine           // 场景线段类型
        }

        /// <summary>
        /// 区域格子属性层类型
        /// </summary>
        public enum SLGAreaGridPropertyLayerType
        {
            Invalid = -1,        // 无效
            SelState,           // 选择状态
            ResLvState          // 资源等级状态
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void SLGRun();

        /// <summary>
        /// Shader
        /// </summary>
        public static readonly int SLG_SHADER_FLAG_ID = Shader.PropertyToID("_SLGShaderFlag");

        public static readonly int SLG_SHADER_SCENEOBJ_BASECOLOR_ID = Shader.PropertyToID("_BaseColor");
        public static readonly int SLG_SHADER_SCENEOBJ_UV_SCALE_OFFSET_ID = Shader.PropertyToID("_UVScaleOffset");
        public static readonly int SLG_SHADER_SCENEOBJ_ZCLIP_ID = Shader.PropertyToID("_ZClip");
        public static readonly int SLG_SHADER_SCENEOBJ_ZTEST_ID = Shader.PropertyToID("_ZTest");
        public static readonly int SLG_SHADER_SCENEOBJ_ZWRITE_ID = Shader.PropertyToID("_ZWrite");

        public static readonly int SLG_SHADER_SCENELINE_ENEMY_ID = Shader.PropertyToID("_Enemy");
        public static readonly int SLG_SHADER_SCENELINE_UV_SCALE_OFFSET_ID = Shader.PropertyToID("_UVScaleOffset");
    }
}

