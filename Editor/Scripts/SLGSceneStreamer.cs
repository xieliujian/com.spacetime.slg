using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;
using System.IO;
using System;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGSceneStreamer
    {
        /// <summary>
        /// 
        /// </summary>
        public static SLGSceneDB ExportSceneDB(out string outScenePath, out List<string> outResPathList)
        {
            outScenePath = "";
            outResPathList = new List<string>();

            var scene = EditorSceneManager.GetActiveScene();
            if (scene == null)
                return null;

            var scenePath = scene.path;
            outScenePath = scenePath;

            var sceneDir = Path.GetDirectoryName(scenePath);

            var path = SLGEditUtils.GetSLGSceneDBPath(scenePath);
            SLGEditUtils.SafeRemoveAsset(path);

            // 没有格子不导出数据
            var grid = GameObject.FindObjectOfType<Grid>();
            if (grid == null)
                return null;

            SLGSceneDB sceneDB = ScriptableObject.CreateInstance<SLGSceneDB>();
            AssetDatabase.CreateAsset(sceneDB, path);

            SLGEditUtils.ReloadLayerCfgMgr();
            SLGRenderGridEdit.StreamerExport(sceneDB, out outResPathList);
            SLGPropertyGridEdit.StreamerExport(sceneDB);
            SLGPropertyGridEdit.ExportExcel(sceneDB, sceneDir);

            EditorUtility.SetDirty(sceneDB);
            AssetDatabase.SaveAssets();

            return sceneDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPathList"></param>
        public static void CreateSLGAllRes(List<string> resPathList)
        {
            if (resPathList.Count <= 0)
                return;

            string SLG_STREAM_SCENE_RES_GO_ROOT_NAME = "SLG";

            GameObject rootGo = new GameObject(SLG_STREAM_SCENE_RES_GO_ROOT_NAME);
            rootGo.AddComponent<SLGSceneMgrMono>();

            foreach(var path in resPathList)
            {
                GameObject prefabGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefabGo == null)
                    continue;

                GameObject go = PrefabUtility.InstantiatePrefab(prefabGo) as GameObject;
                if (go == null)
                    continue;

                go.transform.SetParent(rootGo.transform);
                go.SetActive(false);
            }
        }
    }
}

