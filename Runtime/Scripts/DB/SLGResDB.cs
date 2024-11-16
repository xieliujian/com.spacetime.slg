using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGResDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public string resPath;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int renderQueue;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public bool zWriteOn;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public string matPath;
    }
}

