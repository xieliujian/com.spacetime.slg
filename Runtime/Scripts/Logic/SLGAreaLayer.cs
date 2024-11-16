using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SLGAreaLayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract int GetLayerIndex();

        /// <summary>
        /// 
        /// </summary>
        public virtual void Render()
        {

        }
    }
}

