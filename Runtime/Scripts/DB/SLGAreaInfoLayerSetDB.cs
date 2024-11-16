using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    [Serializable]
    public class SLGAreaInfoLayerSetDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaInfoLayerDB> layerList = new List<SLGAreaInfoLayerDB>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaPropertyInfoLayerDB> propertyLayerList = new List<SLGAreaPropertyInfoLayerDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SLGAreaInfoLayerDB GetLineLayer()
        {
            foreach(var layer in layerList)
            {
                if (layer == null)
                    continue;

                if (layer.IsSceneLineType())
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="resPath"></param>
        /// <param name="yAxisOffset"></param>
        public void FillAreaInfoLayerDB(int layerID, string resPath, 
            SLGDefine.SLGInfoLayerType infoLayerType, 
            SLGDefine.SLGAreaGridPropertyLayerType areaPropertyLayerType, Vector2Int propertyTexSeq)
        {
            var lowResPath = resPath.ToLower();

            if (areaPropertyLayerType == SLGDefine.SLGAreaGridPropertyLayerType.Invalid)
            {
                CreateAreaInfoLayerDB(layerID, lowResPath, infoLayerType);
            }
            else
            {
                CreateAreaPropertyInfoLayerDB(layerID, lowResPath, infoLayerType, areaPropertyLayerType, propertyTexSeq);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public SLGAreaPropertyInfoLayerDB FindAreaPropertyInfoLayer(SLGDefine.SLGAreaGridPropertyLayerType propertyType)
        {
            foreach(var layer in propertyLayerList)
            {
                if (layer == null)
                    continue;

                if (layer.propertyType == propertyType)
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="lowResPath"></param>
        /// <param name="infoLayerType"></param>
        void CreateAreaPropertyInfoLayerDB(int layerID, string lowResPath,
            SLGDefine.SLGInfoLayerType infoLayerType, SLGDefine.SLGAreaGridPropertyLayerType propertyType, Vector2Int propertyTexSeq)
        {
            var layer = FindAreaPropertyInfoLayerDB(layerID);
            if (layer != null)
                return;

            layer = new SLGAreaPropertyInfoLayerDB();
            layer.layerID = layerID;
            layer.infoLayerType = infoLayerType;
            layer.resPath = lowResPath;
            layer.propertyType = propertyType;
            layer.propertyTexSeqWidth = propertyTexSeq.x;
            layer.propertyTexSeqHeight = propertyTexSeq.y;
            propertyLayerList.Add(layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="resPath"></param>
        /// <param name="infoLayerType"></param>
        void CreateAreaInfoLayerDB(int layerID, string lowResPath,
            SLGDefine.SLGInfoLayerType infoLayerType)
        {
            var layer = FindAreaInfoLayerDB(layerID);
            if (layer != null)
                return;

            layer = new SLGAreaInfoLayerDB();
            layer.layerID = layerID;
            layer.infoLayerType = infoLayerType;
            layer.resPath = lowResPath;
            layerList.Add(layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        SLGAreaPropertyInfoLayerDB FindAreaPropertyInfoLayerDB(int layerID)
        {
            SLGAreaPropertyInfoLayerDB findInfoLayer = null;

            foreach (var layer in propertyLayerList)
            {
                if (layer == null)
                    continue;

                if (layerID == layer.layerID)
                {
                    findInfoLayer = layer;
                    break;
                }
            }

            return findInfoLayer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <returns></returns>
        SLGAreaInfoLayerDB FindAreaInfoLayerDB(int layerID)
        {
            SLGAreaInfoLayerDB findInfoLayer = null;

            foreach(var layer in layerList)
            {
                if (layer == null)
                    continue;

                if (layerID == layer.layerID)
                {
                    findInfoLayer = layer;
                    break;
                }
            }

            return findInfoLayer;
        }
    }
}

