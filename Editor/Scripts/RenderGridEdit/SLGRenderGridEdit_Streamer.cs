using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGRenderGridEdit
    {
        /// <summary>
        /// 
        /// </summary>
        public static void StreamerExport(SLGSceneDB sceneDB, out List<string> outResPathList)
        {
            InitSceneDB(sceneDB);
            FillRenderLayerSceneDB(sceneDB);
            FillInfoLayerSceneDB(sceneDB);
            CalcAllAreaBounds(sceneDB);

            outResPathList = sceneDB.resDB.realResPathList;
            outResPathList.AddRange(sceneDB.resDB.realCustomResPathList);
            outResPathList.Add(sceneDB.resDB.realShareGridResPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void InitSceneDB(SLGSceneDB sceneDB)
        {
            sceneDB.Init();

            // .
            var resPath = SLGEditUtils.SLG_INFO_PREFAB_PATH_PREFIX + SLGEditUtils.SLG_SHARE_GRID_PREFAB_NAME + SLGEditUtils.PREFAB_SUFFIX;
            var resPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);
            if (resPrefab != null)
            {
                sceneDB.resDB.InitShareGridRes(resPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void FillRenderLayerSceneDB(SLGSceneDB sceneDB)
        {
            var gridObj = GetSLGSceneRenderRootNode();
            if (gridObj == null)
                return;

            var isPrefab = SLGEditUtils.IsPrefabObject(gridObj.gameObject);
            if (isPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(gridObj.gameObject,
                        PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.AutomatedAction);
            }

            var childCount = gridObj.transform.childCount;
            if (childCount <= 0)
                return;

            for (int i = 0; i < childCount; i++)
            {
                var child = gridObj.transform.GetChild(i);
                if (child == null)
                    continue;

                var childName = child.name;

                var layer = SLGLayerConfigMgr.S.GetRenderLayer(childName);
                if (layer == null)
                    continue;

                if (layer.renderLayerIsDynamic)
                {
                    FillDynamicMapRenderLayer(sceneDB, child);
                }
                else
                {
                    FillMapRenderLayer(sceneDB, child, layer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void FillInfoLayerSceneDB(SLGSceneDB sceneDB)
        {
            var layerCfgList = SLGLayerConfigMgr.S.infoLayerCfgList;
            if (layerCfgList == null || layerCfgList.Count <= 0)
                return;

            foreach (var layerCfg in layerCfgList)
            {
                if (layerCfg == null)
                    continue;

                var layerID = layerCfg.layerID;
                var resName = layerCfg.infoLayerPrefabName;
                var isZWriteOn = layerCfg.isZWriteOn;
                var renderQueue = SLGEditUtils.CalcSLGRenderQueue(layerCfg.isOpacityLayer, layerCfg.renderQueueOffset);
                var infoLayerType = layerCfg.infoLayerType;
                var areaPropertyLayerType = layerCfg.infoAreaPropertyLayerType;
                var propertyTexSeq = new Vector2Int(layerCfg.infoAreaPropertyLayerTexSeqWidth, layerCfg.infoAreaPropertyLayerTexSeqHeight);

                var resPath = SLGEditUtils.SLG_INFO_PREFAB_PATH_PREFIX + resName + SLGEditUtils.PREFAB_SUFFIX;
                var resPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);
                if (resPrefab == null)
                    return;

                sceneDB.FillAreaInfoLayerDB(layerID, resPath, renderQueue, isZWriteOn, infoLayerType, areaPropertyLayerType, propertyTexSeq);
            }
        }

        /// <summary>
        /// ÃÓ≥‰∂ØÃ¨µÿÕº‰÷»æ≤„
        /// </summary>
        /// <param name="sceneDB"></param>
        /// <param name="rootNode"></param>
        static void FillDynamicMapRenderLayer(SLGSceneDB sceneDB, Transform rootNode)
        {
            var childCount = rootNode.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = rootNode.GetChild(i);
                if (child == null)
                    continue;

                var childName = child.name;
                var layerCfg = SLGLayerConfigMgr.S.GetRenderDynamicLayer(childName);
                if (layerCfg == null)
                    continue;

                int dynamicMapIndex = -1;
                childName = childName.Replace(layerCfg.renderDynamicLayerRootName, "");

                try
                {
                    dynamicMapIndex = int.Parse(childName);
                }
                catch
                {
                    Debug.LogError($"[SceneStreamerSLG][FillDynamicMapRenderLayer] [childName]({child.name})");
                }

                if (dynamicMapIndex <= 0)
                    continue;

                var objList = SLGEditUtils.CollectAllPrefabByRootNode(child.gameObject);
                if (objList == null || objList.Count <= 0)
                    continue;

                foreach (var obj in objList)
                {
                    if (obj == null)
                        continue;

                    var prefabPath = SLGEditUtils.GetObjectAssetPath(obj);
                    var pos = obj.transform.position;
                    var rot = obj.transform.eulerAngles;

                    pos = SLGUtils.ResetPosY(pos);

                    var meshRender = obj.GetComponentInChildren<MeshRenderer>();
                    if (meshRender == null)
                        continue;

                    MeshFilter meshFilter = meshRender.GetComponent<MeshFilter>();
                    if (meshFilter == null)
                        continue;

                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh == null)
                        continue;

                    var mat = meshRender.sharedMaterial;
                    if (mat == null)
                        continue;

                    var uvScaleOffset = SLGUtils.CalcUVScaleOffsetByMesh(mesh);
                    var matPath = AssetDatabase.GetAssetPath(mat);
                    var scale = meshRender.transform.localScale;

                    var layerID = layerCfg.layerID;
                    var isZWriteOn = layerCfg.isZWriteOn;
                    var renderQueue = SLGEditUtils.CalcSLGRenderQueue(layerCfg.isOpacityLayer, layerCfg.renderQueueOffset);
                    sceneDB.FillAreaDynamicMapDB(dynamicMapIndex, layerID, obj, prefabPath, matPath, pos, rot, scale, uvScaleOffset,
                        renderQueue, isZWriteOn);
                }
            }
        }

        /// <summary>
        /// ÃÓ≥‰µÿÕº‰÷»æ≤„
        /// </summary>
        /// <param name="rootNode"></param>
        static void FillMapRenderLayer(SLGSceneDB sceneDB, Transform rootNode, SLGLayerConfig layerCfg)
        {
            var objList = SLGEditUtils.CollectAllPrefabByRootNode(rootNode.gameObject);
            if (objList == null || objList.Count <= 0)
                return;

            foreach (var obj in objList)
            {
                if (obj == null)
                    continue;

                var prefabPath = SLGEditUtils.GetObjectAssetPath(obj);
                var pos = obj.transform.position;
                var rot = obj.transform.eulerAngles;

                pos = SLGUtils.ResetPosY(pos);

                var meshRender = obj.GetComponentInChildren<MeshRenderer>();
                if (meshRender == null)
                    continue;

                MeshFilter meshFilter = meshRender.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    continue;

                Mesh mesh = meshFilter.sharedMesh;
                if (mesh == null)
                    continue;

                var mat = meshRender.sharedMaterial;
                if (mat == null)
                    continue;

                var uvScaleOffset = SLGUtils.CalcUVScaleOffsetByMesh(mesh);
                var matPath = AssetDatabase.GetAssetPath(mat);
                var scale = meshRender.transform.localScale;

                var layerID = layerCfg.layerID;
                var isZWriteOn = layerCfg.isZWriteOn;
                var renderQueue = SLGEditUtils.CalcSLGRenderQueue(layerCfg.isOpacityLayer, layerCfg.renderQueueOffset);
                sceneDB.FillAreaMapDB(layerID, obj, prefabPath, matPath, pos, rot, scale, uvScaleOffset, renderQueue, isZWriteOn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void CalcAllAreaBounds(SLGSceneDB sceneDB)
        {
            var areaSetDB = sceneDB.areaSetDB;

            areaSetDB.CalcAllAreaBounds();
        }
    }
}

