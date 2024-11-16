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
    public class SLGAreaInfoLayerDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// ∂ØÃ¨≤„¿‡–Õ
        /// </summary>
        [SerializeField]
        public SLGDefine.SLGInfoLayerType infoLayerType;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public string resPath;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSceneLineType()
        {
            return infoLayerType == SLGDefine.SLGInfoLayerType.SceneLine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAreaGridType()
        {
            return infoLayerType == SLGDefine.SLGInfoLayerType.AreaGrid;
        }
    }
}

