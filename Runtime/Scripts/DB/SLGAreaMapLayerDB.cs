using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGAreaMapLayerDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapBlockDB> blockList = new List<SLGAreaMapBlockDB>();
    }
}

