using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;
using Logger = ST.Core.Logging.Logger;

namespace ST.SLG
{
    /// <summary>
    /// SLG 编辑器工具类：提供预制体与路径、安全删除资源、按层配置计算渲染顺序、根节点 Grid/Tilemap 创建与通用编辑辅助。
    /// </summary>
    public class SLGEditUtils
    {
        /// <summary>
        /// 逻辑/信息层预制体资源路径前缀（相对于 Assets 目录）。
        /// </summary>
        public const string SLG_INFO_PREFAB_PATH_PREFIX = "Assets/scene/common/res/slg/logicprefab/";

        /// <summary>
        /// 全局共用的 SLG 共享格预制体名（无扩展名）。
        /// </summary>
        public const string SLG_SHARE_GRID_PREFAB_NAME = "slg_share_grid";

        /// <summary>
        /// Unity 预制体资源文件扩展名。
        /// </summary>
        public const string PREFAB_SUFFIX = ".prefab";

        const string SCENE_SUFFIX = ".unity";

        const string SLG_SCENE_SUFFIX_NAME = "_SLG";

        const string ASSET_SUFFIX = ".asset";

        [MenuItem("MHT/SLG/同步场景渲染根节点", false, 142)]
        static void CreateOrSyncSceneRenderRootNode()
        {
            SLGRenderGridEdit.CreateOrSyncSceneRenderRootNode();
        }

        [MenuItem("MHT/SLG/同步场景属性根节点", false, 143)]
        static void CreateOrSyncScenePropertyRootNode()
        {
            SLGPropertyGridEdit.CreateOrSyncScenePropertyRootNode();
        }

        [MenuItem("MHT/SLG/同步场景动态对象组", false, 144)]
        static void CreateOrSyncSLGSceneDynamicObjGroup()
        {
            SLGDynamicObjGroupEdit.CreateOrSyncSLGSceneDynamicObjGroup();
        }

        //[MenuItem("MHT/SLG/BottomLayer合并", false, 145)]
        static void CombineAllRenderLayerBottomMap()
        {
            SLGRenderGridEdit.CombineAllRenderLayerBottomMap();
        }

        /// <summary>
        /// 从工程中安全删除指定 Assets 路径的资源；若不存在则忽略。
        /// </summary>
        /// <param name="assetsPath">Assets 相对路径，例如 "Assets/..."</param>
        public static void SafeRemoveAsset(string assetsPath)
        {
            Logger.LogDebugF("[SLG] SafeRemoveAsset {0}", assetsPath);

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetsPath);

            if (obj != null)
            {
                var succeed = AssetDatabase.DeleteAsset(assetsPath);
            }
        }

        /// <summary>
        /// 判断 GameObject 是否可视为「独立预制体根」：用于 SLG 收集时与父级是否也为预制体实例等规则。
        /// </summary>
        /// <param name="gameObject">待检测对象</param>
        /// <param name="bIgnore">当父节点为预制体时是否仍把自身当根判断（默认 false）</param>
        /// <returns>可视为可单独收集的预制体实例或资源则 true，否则 false</returns>
        public static bool IsPrefabObject(GameObject gameObject, bool bIgnore = false)
        {
            bool bIsPrefab = PrefabUtility.IsPartOfPrefabAsset(gameObject) ||
                PrefabUtility.IsPartOfPrefabInstance(gameObject);

            if (bIsPrefab)
            {
                if (gameObject.transform.parent != null)
                {
                    // 若父也是 prefab 的一部分，则子不算独立预制体根
                    GameObject parentObject = gameObject.transform.parent.gameObject;

                    bool bParentIsPrefab = PrefabUtility.IsPartOfPrefabAsset(parentObject) ||
                        PrefabUtility.IsPartOfPrefabInstance(parentObject);
                    if (bParentIsPrefab)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取与 GameObject 关联的预制体资源在 Project 中的 Assets 路径。
        /// </summary>
        /// <param name="gameObject">场景或工程中的游戏对象</param>
        /// <returns>预制体 .prefab 的 Assets 路径，无法解析时返回空字符串</returns>
        public static string GetObjectAssetPath(GameObject gameObject)
        {
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            }

            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
            }

#if UNITY_2022_1_OR_NEWER
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
#else
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
#endif
            if (prefabStage != null)
            {
                return prefabStage.assetPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// 在指定根节点下递归收集所有满足 <see cref="IsPrefabObject"/> 的预制体根对象。
        /// </summary>
        /// <param name="parent">根游戏对象</param>
        /// <param name="ignoreUnVis">为 true 时跳过未激活（activeSelf 为 false）的节点</param>
        /// <returns>收集到的预制体根列表</returns>
        public static List<GameObject> CollectAllPrefabByRootNode(GameObject parent, bool ignoreUnVis = false)
        {
            List<GameObject> prefabList = new List<GameObject>();

            if (parent == null)
                return prefabList;

            var transArray = parent.GetComponentsInChildren<Transform>(true);
            foreach (var trans in transArray)
            {
                if (trans == null)
                    continue;

                var go = trans.gameObject;

                if (ignoreUnVis)
                {
                    if (!go.activeSelf)
                        continue;
                }

                bool bPerfab = IsPrefabObject(go, false);
                if (!bPerfab)
                    continue;

                prefabList.Add(go);
            }

            return prefabList;
        }

        /// <summary>
        /// 由 .unity 场景路径推导出同目录下 SLG 场景数据资源（<c>*_SLG.asset</c>）路径。
        /// </summary>
        /// <param name="scenePath">场景文件的 Assets 路径，带 .unity 后缀</param>
        /// <returns>SLG 数据 <see cref="SLGSceneDB"/> 的 Assets 路径</returns>
        public static string GetSLGSceneDBPath(string scenePath)
        {
            string path = scenePath.Replace(SCENE_SUFFIX, "");
            path += SLG_SCENE_SUFFIX_NAME;
            path += ASSET_SUFFIX;
            return path;
        }

        /// <summary>
        /// 将 Transform 的本地 Y 坐标置为 0，保留 XZ。
        /// </summary>
        /// <param name="trans">目标变换</param>
        public static void ResetTransPosY(Transform trans)
        {
            var pos = trans.localPosition;
            trans.localPosition = new Vector3(pos.x, 0f, pos.z);
        }

        /// <summary>
        /// 将世界坐标 Y 分量置 0 的副本，供 SLG 平面高度对齐。
        /// </summary>
        /// <param name="pos">输入位置</param>
        /// <returns>Y 为 0 的 <see cref="Vector3"/></returns>
        public static Vector3 ResetPosY(Vector3 pos)
        {
            return new Vector3(pos.x, 0f, pos.z);
        }

        /// <summary>
        /// 由是否透明及偏移量计算 SLG 渲染使用的最终 RenderQueue：不透明与透明基准分离（约 2000+ / 3000+ 段）。
        /// </summary>
        /// <param name="isOpacity">是否为透明层</param>
        /// <param name="renderQueueOffset">在对应段内的增量偏移</param>
        /// <returns>最终 RenderQueue 整数值</returns>
        public static int CalcSLGRenderQueue(bool isOpacity, int renderQueueOffset)
        {
            int renderQueue = 2000;

            if (isOpacity)
            {
                renderQueue = 2000 + renderQueueOffset;
            }
            else
            {
                renderQueue = 3000 + renderQueueOffset;
            }

            return renderQueue;
        }

        /// <summary>
        /// 重新加载并初始化默认 SLG 层配置表（<see cref="SLGLayerConfigMgr"/>），编辑工具在改表或打开窗口前宜调用。
        /// </summary>
        public static void ReloadLayerCfgMgr()
        {
            SLGLayerConfigMgr layerCfgMgr = SLGLayerConfigMgr.S;
            layerCfgMgr.LoadDefaultConfig();
            layerCfgMgr.Init();
        }

        /// <summary>
        /// 将根 <see cref="Grid"/> 的单元格大小与排布同步为 SLG 约定值。
        /// </summary>
        /// <param name="grid">场景中的 SLG 根 Grid</param>
        public static void SyncSLGRootGridProperty(Grid grid)
        {
            if (grid == null)
                return;

            grid.cellSize = new Vector3(SLGDefine.s_SLG_Grid_UnitSize, SLGDefine.s_SLG_Grid_UnitSize, 0);
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
        }

        /// <summary>
        /// 按渲染层在配置表中的顺序计算层在 Y 上的微小抬升，避免同平面 Z 冲突。
        /// </summary>
        /// <param name="layerIndex">层在表或子节点中的序号（自 0）</param>
        /// <returns>用于 localPosition Y 的偏移量</returns>
        public static float CalcSLGLayerPosYOffset(int layerIndex)
        {
            return (layerIndex + 1) * 0.01f;
        }

        /// <summary>
        /// 在指定根节点下创建子 Tilemap 与 TilemapRenderer，作为 SLG 渲染/属性子层。
        /// </summary>
        /// <param name="rootGo">根节点（通常为场景 SLG 根）</param>
        /// <param name="layerName">子节点名即层名</param>
        /// <param name="layerIndex">传给 <see cref="CalcSLGLayerPosYOffset"/> 的层序号</param>
        public static void CreateSLGLayer(GameObject rootGo, string layerName, int layerIndex)
        {
            var layerGo = new GameObject(layerName);
            if (layerGo == null)
                return;

            layerGo.AddComponent<Tilemap>();
            layerGo.AddComponent<TilemapRenderer>();

            layerGo.transform.SetParent(rootGo.transform);
            SLGUtils.ResetTransfrom(layerGo.transform);

            float y = CalcSLGLayerPosYOffset(layerIndex);
            layerGo.transform.localPosition = new Vector3(0f, y, 0f);
        }
    }
}
