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
    public class SLGAreaPropertyInfoBlockDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<Matrix4x4> matrixList = new List<Matrix4x4>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<Vector4> uvScaleOffsetList = new List<Vector4>();
    }
}

