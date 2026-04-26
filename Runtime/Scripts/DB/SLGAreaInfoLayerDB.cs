using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单块区域上某一信息层的数据库描述：层 ID、类型、预制体或资源路径等。
    /// </summary>
    [Serializable]
    public class SLGAreaInfoLayerDB
    {
        /// <summary>
        /// 层 ID（与 <see cref="SLGDefine.SLGInfoLayerType"/> 及层表一致）。
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// 信息层类型（场景线、区域格、路径点等）。
        /// </summary>
        [SerializeField]
        public SLGDefine.SLGInfoLayerType infoLayerType;

        /// <summary>
        /// 该层信息预制体在工程中的资源路径（Assets 相对路径）。
        /// </summary>
        [SerializeField]
        public string resPath;

        /// <summary>
        /// 是否为「场景线」类信息层。
        /// </summary>
        /// <returns>是 SceneLine 类型则为 true</returns>
        public bool IsSceneLineType()
        {
            return infoLayerType == SLGDefine.SLGInfoLayerType.SceneLine;
        }

        /// <summary>
        /// 是否为「区域格」类信息层（大地图格上色等）。
        /// </summary>
        /// <returns>是 AreaGrid 类型则为 true</returns>
        public bool IsAreaGridType()
        {
            return infoLayerType == SLGDefine.SLGInfoLayerType.AreaGrid;
        }
    }
}
