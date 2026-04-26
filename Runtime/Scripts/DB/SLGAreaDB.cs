using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域数据库，存储单个区域分块的逻辑边界、渲染包围盒、地图层、格子集合及场景对象列表。
    /// </summary>
    [System.Serializable]
    public class SLGAreaDB
    {
        /// <summary>
        /// 区域逻辑边界最小点，用于判断坐标归属。
        /// </summary>
        [SerializeField]
        public Vector3 logicBoundMin;

        /// <summary>
        /// 区域逻辑边界最大点，用于判断坐标归属。
        /// </summary>
        [SerializeField]
        public Vector3 logicBoundMax;

        /// <summary>
        /// 区域渲染包围盒最小点（含扩展边距）。
        /// </summary>
        [SerializeField]
        public Vector3 boundMin;

        /// <summary>
        /// 区域渲染包围盒最大点（含扩展边距）。
        /// </summary>
        [SerializeField]
        public Vector3 boundMax;

        /// <summary>
        /// 静态地图层集合，存储该区域的所有静态渲染层数据。
        /// </summary>
        [SerializeField]
        public SLGAreaMapLayerSetDB mapLayerSet = new SLGAreaMapLayerSetDB();

        /// <summary>
        /// 动态地图层集合，存储该区域的所有动态渲染层数据。
        /// </summary>
        [SerializeField]
        public SLGAreaDynamicMapLayerSetDB dynamicMapLayerSet = new SLGAreaDynamicMapLayerSetDB();

        /// <summary>
        /// 格子集合数据库，存储该区域内所有逻辑格子信息。
        /// </summary>
        [SerializeField]
        public SLGAreaGridSetDB gridSet = new SLGAreaGridSetDB();

        /// <summary>
        /// 该区域内的场景对象列表（运行时，不序列化），用于计算包围盒。
        /// </summary>
        [NonSerialized]
        public List<GameObject> objList = new List<GameObject>();

        /// <summary>
        /// 根据逻辑边界初始化格子集合，计算起止格子坐标并生成所有格子。
        /// </summary>
        public void InitGridSet()
        {
            gridSet.CalcStartEndPos(logicBoundMin, logicBoundMax);
            gridSet.Init();
        }

        /// <summary>
        /// 向区域对象列表中添加场景对象。
        /// </summary>
        /// <param name="obj">要添加的场景对象</param>
        public void AddObj(GameObject obj)
        {
            objList.Add(obj);
        }

        /// <summary>
        /// 根据区域内所有对象计算渲染包围盒，并附加额外边距以防止边缘裁剪。
        /// </summary>
        public void CalcBounds()
        {
            var bounds = SLGUtils.CalcObjListBounds(objList);

            boundMin = bounds.min;
            boundMax = bounds.max;

            boundMin.y = 0f;
            boundMax.y = 1f;

            // 扩展包围盒以防止旋转头尾边缘片漏渲染问题
            // 之前之所以有这个问题，是因为SLG渲染管线之前有一个延迟刷新一帧
            // 等到SLG刷新修改的延迟刷新之后，不再需要这个extraSize了。
            float extraSize = 5f;
            boundMin -= new Vector3(extraSize, 0f, extraSize);
            boundMax += new Vector3(extraSize, 0f, extraSize);
        }

        /// <summary>
        /// 根据区域的行列索引和地图尺寸计算逻辑边界。
        /// </summary>
        /// <param name="x">区域列索引</param>
        /// <param name="y">区域行索引</param>
        /// <param name="gridWidth">地图总格子列数</param>
        /// <param name="gridHeight">地图总格子行数</param>
        public void CalcLogicBound(int x, int y, int gridWidth, int gridHeight)
        {
            logicBoundMin.x = gridWidth * (-0.5f) + x * SLGDefine.s_SLG_Area_HorizontalSize;
            logicBoundMin.y = 0f;
            logicBoundMin.z = gridHeight * (-0.5f) + y * SLGDefine.s_SLG_Area_VerticalSize;

            logicBoundMax.x = logicBoundMin.x + SLGDefine.s_SLG_Area_HorizontalSize;
            logicBoundMax.y = 1f;
            logicBoundMax.z = logicBoundMin.z + SLGDefine.s_SLG_Area_VerticalSize;
        }

        /// <summary>
        /// 判断给定世界坐标是否在该区域的逻辑边界内。
        /// </summary>
        /// <param name="pos">世界坐标位置</param>
        /// <returns>在区域内返回true，否则返回false</returns>
        public bool IsInArea(Vector3 pos)
        {
            if (pos.x >= logicBoundMin.x && pos.z >= logicBoundMin.z &&
                pos.x < logicBoundMax.x && pos.z < logicBoundMax.z)
            {
                return true;
            }

            return false;
        }
    }
}

