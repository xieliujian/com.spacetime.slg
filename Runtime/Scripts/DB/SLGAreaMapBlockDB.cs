using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGAreaMapBlockDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int resID;

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

