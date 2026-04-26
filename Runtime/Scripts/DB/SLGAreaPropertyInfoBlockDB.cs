using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 属性信息层中单批 GPU 实例块：实例矩阵与 UV 参数列表。
    /// </summary>
    [Serializable]
    public class SLGAreaPropertyInfoBlockDB
    {
        /// <summary>
        /// 属性四边形实例变换矩阵。
        /// </summary>
        [SerializeField]
        public List<Matrix4x4> matrixList = new List<Matrix4x4>();

        /// <summary>
        /// 与矩阵对应的 UV 缩放偏移。
        /// </summary>
        [SerializeField]
        public List<Vector4> uvScaleOffsetList = new List<Vector4>();
    }
}
