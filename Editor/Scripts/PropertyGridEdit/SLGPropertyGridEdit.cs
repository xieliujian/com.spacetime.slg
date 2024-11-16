using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        /// <summary>
        /// SLGÊôÐÔ¸ùÃû×Ö
        /// </summary>
        const string SLG_PROPERTY_ROOT_NAME = "SLGPropertyGrid";

        /// <summary>
        /// 
        /// </summary>
        public static void CreateOrSyncScenePropertyRootNode()
        {
            SLGEditUtils.ReloadLayerCfgMgr();

            var rootGo = GetScenePropertyRootNode();
            if (rootGo == null)
            {
                CreateScenePropertyRootNode();
            }
            else
            {
                SyncScenePropertyRootNode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void SyncScenePropertyRootNode()
        {
            var rootGo = GameObject.Find(SLG_PROPERTY_ROOT_NAME);
            if (rootGo == null)
                return;

            var grid = rootGo.GetComponent<Grid>();
            if (grid == null)
                return;

            SLGEditUtils.SyncSLGRootGridProperty(grid);

            int posBaseOffset = GetPropertyLayerBaseOffset();

            var childCount = rootGo.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childGo = rootGo.transform.GetChild(i);
                if (childGo == null)
                    continue;

                SLGUtils.ResetTransfrom(childGo.transform);

                float y = SLGEditUtils.CalcSLGLayerPosYOffset(posBaseOffset + i);
                childGo.transform.position = new Vector3(0f, y, 0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void CreateScenePropertyRootNode()
        {
            var rootGo = new GameObject(SLG_PROPERTY_ROOT_NAME);
            if (rootGo == null)
                return;

            var grid = rootGo.AddComponent<Grid>();
            if (grid == null)
                return;

            SLGEditUtils.SyncSLGRootGridProperty(grid);

            int index = GetPropertyLayerBaseOffset();
            CreateSelPropertyLayer(rootGo, index);
            CreateResLvPropertyLayer(rootGo, ++index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static int GetPropertyLayerBaseOffset()
        {
            int num = SLGLayerConfigMgr.S.GetRenderLayerNum();
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static GameObject GetScenePropertyRootNode()
        {
            var rootGo = GameObject.Find(SLG_PROPERTY_ROOT_NAME);
            if (rootGo == null)
                return null;

            var grid = rootGo.GetComponent<Grid>();
            if (grid == null)
                return null;

            return rootGo;
        }
    }
}
