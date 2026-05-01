using UnityEngine;
using UnityEditor;
using static ST.SLG.SLGDefine;

namespace ST.SLG
{
    /// <summary>
    /// 编辑器专用的 ISLGWarp 实现，用于流化场景测试。
    /// </summary>
    public class SLGWarpEditor : ISLGWarp
    {
        Camera m_Camera;

        /// <summary>
        /// 构造函数，传入摄像机引用。
        /// </summary>
        /// <param name="camera">场景摄像机</param>
        public SLGWarpEditor(Camera camera)
        {
            m_Camera = camera;
        }

        /// <summary>
        /// 设置摄像机引用。
        /// </summary>
        /// <param name="camera">场景摄像机</param>
        public void SetCamera(Camera camera)
        {
            m_Camera = camera;
        }

        /// <summary>
        /// 是否运行在 Soul 引擎环境下（编辑器模式恒为 false）。
        /// </summary>
        /// <returns>是否为 Soul 引擎</returns>
        public bool IsSoulEngine()
        {
            return false;
        }

        /// <summary>
        /// 获取主玩家世界坐标（编辑器模式返回摄像机位置）。
        /// </summary>
        /// <returns>主玩家位置</returns>
        public Vector3 GetMainPlayerPos()
        {
            return m_Camera != null ? m_Camera.transform.position : Vector3.zero;
        }

        /// <summary>
        /// 获取主摄像机。
        /// </summary>
        /// <returns>主摄像机</returns>
        public Camera GetMainCamera()
        {
            return m_Camera;
        }

        /// <summary>
        /// 获取当前激活的摄像机。
        /// </summary>
        /// <returns>当前激活摄像机</returns>
        public Camera GetCurrentActiveCamera()
        {
            return m_Camera;
        }

        /// <summary>
        /// 按完整资源名同步加载资源（编辑器模式使用 AssetDatabase）。
        /// </summary>
        /// <param name="fullName">资源相对路径（不含 Assets/ 前缀）</param>
        /// <returns>加载到的 Unity 对象</returns>
        public UnityEngine.Object GetResource(string fullName)
        {
            // 编辑器模式下使用 AssetDatabase 加载资源
            // fullName 应该是相对路径，如 "scene/common/res/slg/logicprefab/xxx.prefab"

            string assetPath = fullName;

            // 如果已经是 Assets/ 开头，直接使用
            if (!assetPath.StartsWith("Assets/"))
            {
                assetPath = "Assets/" + assetPath;
            }

            // 如果没有扩展名，添加 .prefab
            if (!System.IO.Path.HasExtension(assetPath))
            {
                assetPath += ".prefab";
            }

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

            if (asset == null)
            {
                UnityEngine.Debug.LogWarning($"[SLGWarpEditor] Failed to load resource: {fullName} (tried: {assetPath})");
            }

            return asset;
        }

        /// <summary>
        /// 将预制体短名解析为资源系统使用的完整名称（返回相对路径，不含 Assets/ 前缀）。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        /// <returns>完整资源相对路径</returns>
        public string GetPrefabFullName(string prefabName)
        {
            // 返回相对路径（不含 Assets/ 前缀）
            if (prefabName.StartsWith("Assets/"))
            {
                return prefabName.Substring(7); // 去掉 "Assets/"
            }

            // 构建完整相对路径
            string fullPath = SLGDefine.SLG_LOGIC_PREFAB_PATH + prefabName;

            // 如果没有扩展名，添加 .prefab
            if (!System.IO.Path.HasExtension(fullPath))
            {
                fullPath += ".prefab";
            }

            return fullPath;
        }

        /// <summary>
        /// 异步或分帧加载资源（编辑器模式下同步回调）。
        /// </summary>
        /// <param name="func">加载完成后的回调</param>
        /// <param name="bundlePath">Bundle 或资源路径</param>
        public void StartLoadRes(SLGRun func, string bundlePath)
        {
            func?.Invoke();
        }
    }
}
