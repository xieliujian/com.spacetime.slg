using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域集合数据库，管理所有区域分块及信息层集合。
    /// </summary>
    [Serializable]
    public class SLGAreaSetDB
    {
        /// <summary>
        /// 信息层集合数据库，存储场景线、属性等信息层配置。
        /// </summary>
        [SerializeField]
        public SLGAreaInfoLayerSetDB infoLayerSet = new SLGAreaInfoLayerSetDB();

        /// <summary>
        /// 区域数据库列表，按行列顺序存储所有区域分块。
        /// </summary>
        [SerializeField]
        public List<SLGAreaDB> areaDBList = new List<SLGAreaDB>();

        /// <summary>
        /// 初始化所有区域分块，按地图尺寸和区域数量创建区域并计算逻辑边界与格子集合。
        /// </summary>
        public void Init()
        {
            int width = SLGDefine.s_SLG_Map_HorizontalSize;
            int height = SLGDefine.s_SLG_Map_VerticalSize;

            int areaX = SLGDefine.s_SLG_Area_HorizontalNum;
            int areaY = SLGDefine.s_SLG_Area_VerticalNum;

            for (int y = 0; y < areaY; y++)
            {
                for (int x = 0; x < areaX; x++)
                {
                    var areaDB = new SLGAreaDB();
                    areaDB.CalcLogicBound(x, y, width, height);
                    areaDB.InitGridSet();
                    areaDBList.Add(areaDB);
                }
            }
        }

        /// <summary>
        /// 根据世界坐标查找所在区域的数据库。
        /// </summary>
        /// <param name="pos">世界坐标位置</param>
        /// <returns>包含该坐标的区域数据库，未找到则返回null</returns>
        public SLGAreaDB GetAreaDB(Vector3 pos)
        {
            SLGAreaDB findAreaDB = null;

            foreach (var areaDB in areaDBList)
            {
                if (areaDB == null)
                    continue;

                var isInArea = areaDB.IsInArea(pos);
                if (isInArea)
                {
                    findAreaDB = areaDB;
                    break;
                }
            }

            return findAreaDB;
        }

        /// <summary>
        /// 计算所有区域的渲染包围盒。
        /// </summary>
        public void CalcAllAreaBounds()
        {
            if (areaDBList.Count <= 0)
                return;

            foreach (var areaDB in areaDBList)
            {
                if (areaDB == null)
                    continue;

                areaDB.CalcBounds();
            }
        }

        /// <summary>
        /// 填充区域信息层数据，同时将资源注册到资源数据库。
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <param name="resPath">资源路径</param>
        /// <param name="resDB">场景资源数据库</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        /// <param name="infoLayerType">信息层类型</param>
        /// <param name="areaPropertyLayerType">区域格子属性层类型</param>
        /// <param name="propertyTexSeq">属性贴图序列尺寸</param>
        public void FillAreaInfoLayerDB(int layerID, string resPath, SLGSceneResDB resDB, int renderQueue, bool isZWriteOn,
            SLGDefine.SLGInfoLayerType infoLayerType, SLGDefine.SLGAreaGridPropertyLayerType areaPropertyLayerType,
            Vector2Int propertyTexSeq)
        {
            resDB.AddCustomRes(resPath, renderQueue, isZWriteOn);
            infoLayerSet.FillAreaInfoLayerDB(layerID, resPath, infoLayerType, areaPropertyLayerType, propertyTexSeq);
        }
    }
}

