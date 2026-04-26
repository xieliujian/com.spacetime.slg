using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域地图层数据库，存储单个静态地图层的渲染块集合。
    /// </summary>
    [System.Serializable]
    public class SLGAreaMapLayerDB
    {
        /// <summary>
        /// 地图层ID。
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// 该层的渲染块列表。
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapBlockDB> blockList = new List<SLGAreaMapBlockDB>();
    }
}

