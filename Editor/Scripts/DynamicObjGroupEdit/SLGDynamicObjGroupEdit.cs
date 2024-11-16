using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGDynamicObjGroupEdit
    {

        /// <summary>
        /// 
        /// </summary>
        public static void CreateOrSyncSLGSceneDynamicObjGroup()
        {
            var globalGo = SLGUtils.FindGlobalRoot();
            if (globalGo == null)
            {
                Debug.LogError("[SLG]不能创建场景动态物件组");
                return;
            }

            var rootTrans = SLGUtils.FindSLGSceneDynamicObjGroupRoot(globalGo);
            if (rootTrans == null)
            {
                CreateAllSLGSceneDynamicObjGroup(globalGo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

