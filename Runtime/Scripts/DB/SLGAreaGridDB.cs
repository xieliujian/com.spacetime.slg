using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域格子数据库，记录单个逻辑格子的坐标和变换矩阵。
    /// </summary>
    [Serializable]
    public class SLGAreaGridDB
    {
        /// <summary>
        /// 格子的逻辑坐标。
        /// </summary>
        [SerializeField]
        public Vector2Int pos;

        /// <summary>
        /// 格子的世界变换矩阵。
        /// </summary>
        [SerializeField]
        public Matrix4x4 matrix;


    }
}

