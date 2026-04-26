
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单行 SLG 层配置：描述该条是否为信息层/渲染层、层 ID 与显示名、渲染队列与深度、动态子地图与信息层/区域属性等。
    /// </summary>
    public class SLGLayerConfig
    {
        /// <summary>
        /// 是否为信息层（true 为信息层，false 为渲染层）。
        /// </summary>
        public bool isInfoLayer;

        /// <summary>
        /// 层 ID，用于排序与查找。
        /// </summary>
        public int layerID;

        /// <summary>
        /// 层在场景中的显示名称/节点名（与挂点对应）。
        /// </summary>
        public string layerName;

        /// <summary>
        /// 是否为半透明（Opacity）层。
        /// </summary>
        public bool isOpacityLayer;

        /// <summary>
        /// 相对基础 RenderQueue 的偏移量。
        /// </summary>
        public int renderQueueOffset;

        /// <summary>
        /// 是否开启深度写入（ZWrite）。
        /// </summary>
        public bool isZWriteOn;

        /// <summary>
        /// 该渲染层是否按「动态子地图」管理（子节点会随多路线/地图块切换时增删，用于大场景流式）。
        /// </summary>
        public bool renderLayerIsDynamic;

        /// <summary>
        /// 动态子地图在 SLG 渲染层根下的子根节点名；其子节点为各子地图分块根。
        /// </summary>
        public string renderDynamicLayerRootName;

        /// <summary>
        /// 信息层类型（非信息层时可忽略）。
        /// </summary>
        public SLGDefine.SLGInfoLayerType infoLayerType = SLGDefine.SLGInfoLayerType.Invalid;

        /// <summary>
        /// 信息层所用预制体资源名。
        /// </summary>
        public string infoLayerPrefabName;

        /// <summary>
        /// 区域格属性层类型（与属性格贴图序列对应）。
        /// </summary>
        public SLGDefine.SLGAreaGridPropertyLayerType infoAreaPropertyLayerType = SLGDefine.SLGAreaGridPropertyLayerType.Invalid;

        /// <summary>
        /// 区域属性层贴图序列的列数（与 <see cref="infoAreaPropertyLayerTexSeqHeight"/> 共同定义帧格布局）。
        /// </summary>
        public int infoAreaPropertyLayerTexSeqWidth = 1;

        /// <summary>
        /// 区域属性层贴图序列的行数。
        /// </summary>
        public int infoAreaPropertyLayerTexSeqHeight = 1;

        /// <summary>
        /// 自 CSV 一行字符串数组解析并填充本结构（字段顺序与表头一致，以逗号分隔；末尾可含分号区分的序列尺寸）。
        /// </summary>
        /// <param name="strArray">与表头列对应的一行单元格文本数组</param>
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
