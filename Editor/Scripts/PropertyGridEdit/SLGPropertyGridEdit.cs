using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 属性格编辑：在场景中维护 <c>SLGPropertyGrid</c> 根与下属 Tilemap 属性子层，并与渲染层数量对齐 Y 向分层。
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        /// <summary>
        /// 场景中表示 SLG 属性根节点的 GameObject 名称（与 <c>SLGPropertyGrid</c> 根对应）。
        /// </summary>
        const string SLG_PROPERTY_ROOT_NAME = "SLGPropertyGrid";

        /// <summary>
        /// 若不存在则创建，否则按当前层配置同步属性根下各子层的 Grid 与 Y 向偏移。
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

        static int GetPropertyLayerBaseOffset()
        {
            int num = SLGLayerConfigMgr.S.GetRenderLayerNum();
            return num;
        }

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
