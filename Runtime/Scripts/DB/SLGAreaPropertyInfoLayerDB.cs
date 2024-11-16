using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SLGAreaPropertyInfoLayerDB : SLGAreaInfoLayerDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGDefine.SLGAreaGridPropertyLayerType propertyType;

        /// <summary>
        /// 
        /// </summary>
        public int propertyTexSeqWidth;

        /// <summary>
        /// 
        /// </summary>
        public int propertyTexSeqHeight;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaPropertyInfoBlockDB> blockList = new List<SLGAreaPropertyInfoBlockDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAreaSelStateLayer()
        {
            return propertyType == SLGDefine.SLGAreaGridPropertyLayerType.SelState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAreaResLvStateLayer()
        {
            return propertyType == SLGDefine.SLGAreaGridPropertyLayerType.ResLvState;
        }
    }
}

