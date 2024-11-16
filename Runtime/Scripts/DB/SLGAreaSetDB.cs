using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SLGAreaSetDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGAreaInfoLayerSetDB infoLayerSet = new SLGAreaInfoLayerSetDB();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaDB> areaDBList = new List<SLGAreaDB>();

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            int width = SLGDefine.SLG_MAP_HORIZONTAL_SIZE;
            int height = SLGDefine.SLG_MAP_VERTICAL_SIZE;

            int areaX = SLGDefine.SLG_AREA_HORIZONTAL_NUM;
            int areaY = SLGDefine.SLG_AREA_VERTICAL_NUM;

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
        /// 
        /// </summary>
        /// <param name="pos"></param>
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="resName"></param>
        /// <param name="yAxisOffset"></param>
        /// <param name="resDB"></param>
        public void FillAreaInfoLayerDB(int layerID, string resPath, SLGSceneResDB resDB, int renderQueue, bool isZWriteOn,
            SLGDefine.SLGInfoLayerType infoLayerType, SLGDefine.SLGAreaGridPropertyLayerType areaPropertyLayerType,
            Vector2Int propertyTexSeq)
        {
            resDB.AddCustomRes(resPath, renderQueue, isZWriteOn);
            infoLayerSet.FillAreaInfoLayerDB(layerID, resPath, infoLayerType, areaPropertyLayerType, propertyTexSeq);
        }
    }
}

