using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 动态地表分组根节点：与 <see cref="SLGUtils.SetSLGSceneDynamicObjGroup"/> 联动，按 <see cref="groupIndex"/> 控制整组显隐。
    /// </summary>
    public class SLGSceneDynamicObjGroup : MonoBehaviour
    {
        /// <summary>
        /// 与场景 DB 中动态地图层索引对应的分组编号。
        /// </summary>
        public int groupIndex;

        /// <summary>
        /// 设置该分组下根物体是否激活。
        /// </summary>
        /// <param name="visible">是否显示</param>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
