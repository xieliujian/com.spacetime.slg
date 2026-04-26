using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 格子可选中与资源等级等属性的枚举（存入 <see cref="SLGPropertyGridDB.selType"/> 对应语义由业务约定）。
    /// </summary>
    public enum SLGSelGridProp
    {
        /// <summary>可选中</summary>
        CanSel,
        /// <summary>不可选中</summary>
        UnSel,
        /// <summary>动态</summary>
        Dynamic,
    }

    /// <summary>
    /// 逻辑格子的属性数据（选中类型、资源等级等），序列化在 <see cref="SLGScenePropDB"/> 中。
    /// </summary>
    [System.Serializable]
    public class SLGPropertyGridDB
    {
        /// <summary>
        /// 逻辑格子坐标。
        /// </summary>
        [SerializeField]
        public Vector2Int pos;

        /// <summary>
        /// 格子在数据列表中的序号或业务索引。
        /// </summary>
        [SerializeField]
        public int index;

        /// <summary>
        /// 选中/可选状态（对应 <see cref="SLGSelGridProp"/>）。
        /// </summary>
        [SerializeField]
        public byte selType;

        /// <summary>
        /// 资源等级或富集类型等业务字段。
        /// </summary>
        [SerializeField]
        public byte resLvType;

        /// <summary>
        /// 将属性恢复为默认可选中与零资源等级。
        /// </summary>
        public void ResetProperty()
        {
            selType = (int)SLGSelGridProp.CanSel;
            resLvType = 0;
        }

        /// <summary>
        /// 占位：可扩展为清空扩展字段。
        /// </summary>
        public void Null()
        {

        }
    }
}
