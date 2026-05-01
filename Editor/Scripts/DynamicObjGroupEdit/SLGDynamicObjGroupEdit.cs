using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ST.Core.Logging.Logger;

namespace ST.SLG
{
    /// <summary>
    /// 编辑器用：在全局根下创建或补全 SLG 场景动态物件组根节点与若干 <see cref="SLGSceneDynamicObjGroup"/> 子组。
    /// </summary>
    public class SLGDynamicObjGroupEdit
    {

        /// <summary>
        /// 在全局根下若尚无动态物件组根，则创建根节点与编号 1–5 的子组，每组挂载 <see cref="SLGSceneDynamicObjGroup"/> 并设置 <c>groupIndex</c>。
        /// </summary>
        public static void CreateOrSyncSLGSceneDynamicObjGroup()
        {
            var globalGo = SLGUtils.FindGlobalRoot();
            if (globalGo == null)
            {
                Logger.LogError("[SLG]不能创建场景动态物件组");
                return;
            }

            var rootTrans = SLGUtils.FindSLGSceneDynamicObjGroupRoot(globalGo);
            if (rootTrans == null)
            {
                CreateAllSLGSceneDynamicObjGroup(globalGo);
            }
        }

        static void CreateAllSLGSceneDynamicObjGroup(GameObject globalGo)
        {
            GameObject rootGo = new GameObject(SLGUtils.SLG_SCENE_DYNAMIC_OBJ_GROUP_ROOT_NAME);
            rootGo.transform.SetParent(globalGo.transform);
            SLGUtils.ResetTransfrom(rootGo.transform);

            for (int i = 0; i < 5; i++)
            {
                var index = i + 1;
                GameObject go = new GameObject(index.ToString());

                var group = go.AddComponent<SLGSceneDynamicObjGroup>();
                if (group != null)
                {
                    group.groupIndex = index;
                }

                go.transform.SetParent(rootGo.transform);
                SLGUtils.ResetTransfrom(go.transform);
            }
        }
    }
}



