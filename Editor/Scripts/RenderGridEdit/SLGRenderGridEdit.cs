using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = ST.Core.Logging.Logger;

namespace ST.SLG
{
    /// <summary>
    /// SLG 渲染格编辑：在场景中创建或同步 <c>SLGRenderGrid</c> 根，并按层配置生成静态或动态子 Tilemap 渲染层。
    /// </summary>
    public partial class SLGRenderGridEdit
    {
        /// <summary>
        /// 场景中表示 SLG 渲染根节点的 GameObject 名称（与 <c>SLGRenderGrid</c> 根对应）。
        /// </summary>
        const string SLG_RENDER_ROOT_NAME = "SLGRenderGrid";

        /// <summary>
        /// 若不存在则创建，否则按当前层配置同步 <c>SLGRenderGrid</c> 下各子层变换与 <c>Grid</c> 参数及 Y 向分层。
        /// </summary>
        public static void CreateOrSyncSceneRenderRootNode()
        {
            SLGEditUtils.ReloadLayerCfgMgr();

            var rootGo = GetSLGSceneRenderRootNode();
            if (rootGo == null)
            {
                CreateSLGSceneRenderRootNode();
            }
            else
            {
                SyncSLGSceneRenderRootNode();
            }
        }

        /// <summary>
        /// 将同场景下 <c>BottomMap1</c> 与 <c>BottomMap2</c> 在 XZ 上对齐的格合并到临时 Tilemap 子层（调试用合层）。
        /// </summary>
        public static void CombineAllRenderLayerBottomMap()
        {
            var grid = GameObject.FindObjectOfType<Grid>();
            if (grid == null)
                return;

            var gridObj = grid.gameObject;
            if (gridObj == null)
                return;

            var isPrefab = SLGEditUtils.IsPrefabObject(gridObj.gameObject);
            if (isPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(gridObj.gameObject,
                        PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.AutomatedAction);
            }

            var bottomLayer1 = gridObj.transform.Find("BottomMap1");
            var bottomLayer2 = gridObj.transform.Find("BottomMap2");

            var obj1List = SLGEditUtils.CollectAllPrefabByRootNode(bottomLayer1.gameObject);
            var obj2List = SLGEditUtils.CollectAllPrefabByRootNode(bottomLayer2.gameObject);

            List<GameObject> newObjList = new List<GameObject>();

            foreach (var obj1 in obj1List)
            {
                if (obj1 == null)
                    continue;

                var pos1 = obj1.transform.localPosition;

                GameObject findObj = null;

                foreach (var obj2 in obj2List)
                {
                    if (obj2 == null)
                        continue;

                    var pos2 = obj2.transform.localPosition;
                    if (pos1.x == pos2.x && pos1.z == pos2.z)
                    {
                        findObj = obj2;
                        break;
                    }
                }

                if (findObj != null)
                {
                    newObjList.Add(findObj);
                }
                else
                {
                    newObjList.Add(obj1);
                }
            }

            GameObject newGo = new GameObject("TempLayer");
            newGo.AddComponent<Tilemap>();
            newGo.AddComponent<TilemapRenderer>();

            newGo.transform.SetParent(gridObj.transform);
            SLGUtils.ResetTransfrom(newGo.transform);

            foreach (var newObj in newObjList)
            {
                if (newObj == null)
                    continue;

                newObj.transform.SetParent(newGo.transform);

                var newPos = newObj.transform.localPosition;
                newObj.transform.localPosition = new Vector3(newPos.x, 0, newPos.z);
            }
        }

        /// <summary>
        /// 在场景中查找名为 <c>SLGRenderGrid</c> 的渲染根，且须挂有 <c>Grid</c> 组件。
        /// </summary>
        /// <returns>根 GameObject，未找到或不带 Grid 时返回 null</returns>
        public static GameObject GetSLGSceneRenderRootNode()
        {
            var rootGo = GameObject.Find(SLG_RENDER_ROOT_NAME);
            if (rootGo == null)
                return null;

            var grid = rootGo.GetComponent<Grid>();
            if (grid == null)
                return null;

            return rootGo;
        }

        static void CreateSLGSceneRenderRootNode()
        {
            var rootGo = new GameObject(SLG_RENDER_ROOT_NAME);
            if (rootGo == null)
                return;

            var grid = rootGo.AddComponent<Grid>();
            if (grid == null)
                return;

            SLGEditUtils.SyncSLGRootGridProperty(grid);
            CreateAllRenderLayer(rootGo);
        }

        static void CreateAllRenderLayer(GameObject rootGo)
        {
            var renderLayerCfgList = SLGLayerConfigMgr.S.renderLayerCfgList;
            var layerNum = renderLayerCfgList.Count;
            if (renderLayerCfgList == null || layerNum <= 0)
            {
                Logger.LogError("[SLG][CreateAllRenderLayer] : renderLayerCfgList 为空或未加载");
                return;
            }

            for (int i = 0; i < layerNum; i++)
            {
                var layer = renderLayerCfgList[i];
                if (layer == null)
                    continue;

                var layerName = layer.layerName;
                var layerID = layer.layerID;
                var layerRootName = layer.renderDynamicLayerRootName;

                if (layer.renderLayerIsDynamic)
                {
                    CreateDynamicMapLayer(rootGo, layerName, layerRootName, layerID);
                }
                else
                {
                    SLGEditUtils.CreateSLGLayer(rootGo, layerName, layerID);
                }
            }
        }

        static GameObject CacheDynamicMapLayerRoot(GameObject rootGo, string layerRootName, int layerIndex)
        {
            var layerTrans = rootGo.transform.Find(layerRootName);
            if (layerTrans != null)
                return layerTrans.gameObject;

            var layerGo = new GameObject(layerRootName);
            if (layerGo == null)
                return null;

            layerGo.transform.SetParent(rootGo.transform);
            SLGUtils.ResetTransfrom(layerGo.transform);

            float y = SLGEditUtils.CalcSLGLayerPosYOffset(layerIndex);
            layerGo.transform.position = new Vector3(0f, y, 0f);

            return layerGo;
        }

        static void CreateDynamicMapLayer(GameObject rootGo, string layerName, string layerRootName, int layerIndex)
        {
            var dynamicRootGo = CacheDynamicMapLayerRoot(rootGo, layerRootName, layerIndex);
            if (dynamicRootGo == null)
                return;

            SLGEditUtils.CreateSLGLayer(dynamicRootGo, layerName, -1);
        }

        static void SyncSLGSceneRenderRootNode()
        {
            var rootGo = GameObject.Find(SLG_RENDER_ROOT_NAME);
            if (rootGo == null)
                return;

            var grid = rootGo.GetComponent<Grid>();
            if (grid == null)
                return;

            SLGEditUtils.SyncSLGRootGridProperty(grid);

            var childCount = rootGo.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childGo = rootGo.transform.GetChild(i);
                if (childGo == null)
                    continue;

                SLGUtils.ResetTransfrom(childGo.transform);

                float y = SLGEditUtils.CalcSLGLayerPosYOffset(i);
                childGo.transform.position = new Vector3(0f, y, 0f);
            }
        }
    }
}
