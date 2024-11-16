
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGLayerConfig
    {
        /// <summary>
        /// 是否信息层
        /// </summary>
        public bool isInfoLayer;

        /// <summary>
        /// 层ID
        /// </summary>
        public int layerID;

        /// <summary>
        /// 层名字
        /// </summary>
        public string layerName;

        /// <summary>
        /// 是否Opacity层
        /// </summary>
        public bool isOpacityLayer;

        /// <summary>
        /// RenderQueue偏移
        /// </summary>
        public int renderQueueOffset;

        /// <summary>
        /// 是否写深度
        /// </summary>
        public bool isZWriteOn;

        /// <summary>
        /// 
        /// </summary>
        public bool renderLayerIsDynamic;

        /// <summary>
        /// 
        /// </summary>
        public string renderDynamicLayerRootName;

        /// <summary>
        /// 信息层类型
        /// </summary>
        public SLGDefine.SLGInfoLayerType infoLayerType = SLGDefine.SLGInfoLayerType.Invalid;

        /// <summary>
        /// 信息层prefab名字
        /// </summary>
        public string infoLayerPrefabName;

        /// <summary>
        /// 区域属性层类型
        /// </summary>
        public SLGDefine.SLGAreaGridPropertyLayerType infoAreaPropertyLayerType = SLGDefine.SLGAreaGridPropertyLayerType.Invalid;

        /// <summary>
        /// 
        /// </summary>
        public int infoAreaPropertyLayerTexSeqWidth = 1;

        /// <summary>
        /// 
        /// </summary>
        public int infoAreaPropertyLayerTexSeqHeight = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strArray"></param>
        public void LoadConfig(string[] strArray)
        {
            int index = 0;
            var isInfoLayer = int.Parse(strArray[index]);
            this.isInfoLayer = (isInfoLayer > 0) ? true : false;

            index++;
            var layerID = int.Parse(strArray[index]);
            this.layerID = layerID;

            index++;
            var layerName = strArray[index];
            this.layerName = layerName;

            index++;
            var isOpacityLayer = int.Parse(strArray[index]);
            this.isOpacityLayer = (isOpacityLayer > 0) ? true : false;

            index++;
            var renderQueueOffset = int.Parse(strArray[index]);
            this.renderQueueOffset = renderQueueOffset;

            index++;
            var isZWriteOn = int.Parse(strArray[index]);
            this.isZWriteOn = (isZWriteOn > 0) ? true : false;

            index++;
            var renderLayerIsDynamic = int.Parse(strArray[index]);
            this.renderLayerIsDynamic = (renderLayerIsDynamic > 0) ? true : false;

            index++;
            var renderDynamicLayerRootName = strArray[index];
            this.renderDynamicLayerRootName = renderDynamicLayerRootName;

            index++;
            var infoLayerType = int.Parse(strArray[index]);
            this.infoLayerType = (SLGDefine.SLGInfoLayerType)infoLayerType;

            index++;
            var infoLayerPrefabName = strArray[index];
            this.infoLayerPrefabName = infoLayerPrefabName;

            index++;
            var infoAreaPropertyLayerType = int.Parse(strArray[index]);
            this.infoAreaPropertyLayerType = (SLGDefine.SLGAreaGridPropertyLayerType)infoAreaPropertyLayerType;

            index++;
            LoadConfig_InfoAreaPropertyTexSeqInfo(strArray, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="index"></param>
        void LoadConfig_InfoAreaPropertyTexSeqInfo(string[] strArray, int index)
        {
            var infoAreaPropertyTexSeqInfo = strArray[index];
            if (string.IsNullOrEmpty(infoAreaPropertyTexSeqInfo))
                return;

            var seqInfoArray = infoAreaPropertyTexSeqInfo.Split(';');
            if (seqInfoArray == null || seqInfoArray.Length != 2)
                return;

            infoAreaPropertyLayerTexSeqWidth = int.Parse(seqInfoArray[0]);
            infoAreaPropertyLayerTexSeqHeight = int.Parse(seqInfoArray[1]);
        }
    }
}
