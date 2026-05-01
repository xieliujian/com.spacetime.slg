using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Logger = ST.Core.Logging.Logger;

namespace ST.SLG
{
    /// <summary>
    /// SLG 渲染格流式导出：从场景 <c>SLGRenderGrid</c> 采集数据写入 <c>SLGSceneDB</c>，并收集依赖资源路径。
    /// </summary>
    public partial class SLGRenderGridEdit
    {
        /// <summary>
        /// 将当前场景中的渲染/信息层与区域数据导出到 <paramref name="sceneDB"/>，并输出涉及的预制体与共享格等资源路径。
        /// </summary>
        /// <param name="sceneDB">要写入的 SLG 场景数据库</param>
        /// <param name="outResPathList">本导出涉及的 prefab 与共享资源等路径列表</param>
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

        static void InitSceneDB(SLGSceneDB sceneDB)
        {
            sceneDB.Init();

            // 共享格预制体用于地图块合批/占位，能加载则登记进 resDB
            var resPath = SLGEditDefine.s_SLGInfoPrefab_PathPrefix + SLGEditDefine.s_SLGShareGrid_PrefabName + SLGEditDefine.s_Prefab_Suffix;
            var resPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);
            if (resPrefab != null)
            {
                sceneDB.resDB.InitShareGridRes(resPath);
            }
        }

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

                var resPath = SLGEditDefine.s_SLGInfoPrefab_PathPrefix + resName + SLGEditDefine.s_Prefab_Suffix;
                var resPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);
                if (resPrefab == null)
                    return;

                sceneDB.FillAreaInfoLayerDB(layerID, resPath, renderQueue, isZWriteOn, infoLayerType, areaPropertyLayerType, propertyTexSeq);
            }
        }

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
                    Logger.LogErrorF("[SceneStreamerSLG][FillDynamicMapRenderLayer] [childName]({0})", child.name);
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

        static void CalcAllAreaBounds(SLGSceneDB sceneDB)
        {
            var areaSetDB = sceneDB.areaSetDB;

            areaSetDB.CalcAllAreaBounds();
        }
    }
}
