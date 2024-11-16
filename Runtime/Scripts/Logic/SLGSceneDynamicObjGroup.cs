using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGSceneDynamicObjGroup : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public int groupIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}

