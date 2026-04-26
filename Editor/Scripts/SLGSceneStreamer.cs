using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;
using System.IO;
using System;

namespace ST.SLG
{
    /// <summary>
    /// SLG 场景流式导出：从当前打开场景收集渲染与属性数据，生成 <see cref="SLGSceneDB"/> 资源及可选资源实例根。
    /// </summary>
    public class SLGSceneStreamer
    {
        /// <summary>
        /// 从当前活动场景导出 SLG 数据：创建并写入 <see cref="SLGSceneDB"/> 资产，并输出场景路径与引用到的资源路径列表。
        /// </summary>
        /// <param name="outScenePath">成功时为主场景的 Assets 路径；失败时可能为空串。</param>
        /// <param name="outResPathList">流式收集到的预制体/共享网格等资源路径列表。</param>
        /// <returns>创建或复用的 <see cref="SLGSceneDB"/>；无网格等无法导出时返回 null。</returns>
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
        /// 在场景中创建名为 “SLG” 的根节点并挂载 <see cref="SLGSceneMgrMono"/>，将给定路径上的预制体实例化到其下并默认失活，用于流式预载集合。
        /// </summary>
        /// <param name="resPathList">预制体资源在 Assets 下的路径列表。</param>
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
