using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 单个区域内全部信息层与属性信息层的配置集合，并提供按类型查找与编辑器填充入口。
    /// </summary>
    [Serializable]
    public class SLGAreaInfoLayerSetDB
    {
        /// <summary>
        /// 普通信息层（非属性纹理类）列表。
        /// </summary>
        [SerializeField]
        public List<SLGAreaInfoLayerDB> layerList = new List<SLGAreaInfoLayerDB>();

        /// <summary>
        /// 属性信息层列表（选中、资源等级等）。
        /// </summary>
        [SerializeField]
        public List<SLGAreaPropertyInfoLayerDB> propertyLayerList = new List<SLGAreaPropertyInfoLayerDB>();

        /// <summary>
        /// 查找类型为场景连线的信息层配置。
        /// </summary>
        /// <returns>首个 SceneLine 层，未配置返回 null</returns>
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
        /// 按编辑器参数创建或跳过已存在的信息层/属性层条目（路径会转为小写）。
        /// </summary>
        /// <param name="layerID">层 ID</param>
        /// <param name="resPath">资源路径</param>
        /// <param name="infoLayerType">信息层类型</param>
        /// <param name="areaPropertyLayerType">属性层类型，Invalid 表示普通信息层</param>
        /// <param name="propertyTexSeq">属性图集尺寸 (宽格数, 高格数)</param>
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
        /// 在属性层列表中按类型查找配置。
        /// </summary>
        /// <param name="propertyType">属性层类型</param>
        /// <returns>匹配项或 null</returns>
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
