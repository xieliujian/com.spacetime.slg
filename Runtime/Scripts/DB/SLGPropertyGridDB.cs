using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public enum SLGSelGridProp
    {
        CanSel,     // 可选中
        UnSel,      // 不可选中
        Dynamic,    // 动态
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGPropertyGridDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public Vector2Int pos;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int index;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public byte selType;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public byte resLvType;

        /// <summary>
        /// 
        /// </summary>
        public void ResetProperty()
        {
            selType = (int)SLGSelGridProp.CanSel;
            resLvType = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Null()
        {

        }
    }
}

