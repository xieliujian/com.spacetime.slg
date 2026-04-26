using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 基于格子的属性信息层配置（选中态、资源等级等）：在 <see cref="SLGAreaInfoLayerDB"/> 基础上扩展属性类型与分块数据。
    /// </summary>
    [Serializable]
    public class SLGAreaPropertyInfoLayerDB : SLGAreaInfoLayerDB
    {
        /// <summary>
        /// 属性层语义类型（选中、资源等级等）。
        /// </summary>
        [SerializeField]
        public SLGDefine.SLGAreaGridPropertyLayerType propertyType;

        /// <summary>
        /// 属性纹理图集横向格子数（序列图宽度方向格数）。
        /// </summary>
        public int propertyTexSeqWidth;

        /// <summary>
        /// 属性纹理图集纵向格子数。
        /// </summary>
        public int propertyTexSeqHeight;

        /// <summary>
        /// GPU 分块列表，每块一批实例。
        /// </summary>
        [SerializeField]
        public List<SLGAreaPropertyInfoBlockDB> blockList = new List<SLGAreaPropertyInfoBlockDB>();

        /// <summary>
        /// 是否为格子选中状态层。
        /// </summary>
        /// <returns>SelState 返回 true</returns>
        public bool IsAreaSelStateLayer()
        {
            return propertyType == SLGDefine.SLGAreaGridPropertyLayerType.SelState;
        }

        /// <summary>
        /// 是否为资源等级展示层。
        /// </summary>
        /// <returns>ResLvState 返回 true</returns>
        public bool IsAreaResLvStateLayer()
        {
            return propertyType == SLGDefine.SLGAreaGridPropertyLayerType.ResLvState;
        }
    }
}
