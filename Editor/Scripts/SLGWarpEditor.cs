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
        /// <param name="fullName">资源完整路径或标识</param>
        /// <returns>加载到的 Unity 对象</returns>
        public UnityEngine.Object GetResource(string fullName)
        {
            // 编辑器模式下使用 AssetDatabase 加载资源
            // fullName 可能是相对路径或完整路径

            // 如果已经是 Assets/ 开头的完整路径，直接加载
            if (fullName.StartsWith("Assets/"))
            {
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullName);
            }

            // 否则尝试构建完整路径
            string assetPath = fullName;

            // 如果没有扩展名，尝试添加 .prefab
            if (!System.IO.Path.HasExtension(assetPath))
            {
                assetPath += ".prefab";
            }

            // 如果不是绝对路径，尝试在 SLG 资源目录下查找
            if (!assetPath.StartsWith("Assets/"))
            {
                assetPath = SLGEditDefine.s_SLGInfoPrefab_PathPrefix + assetPath;
            }

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

            if (asset == null)
            {
                UnityEngine.Debug.LogWarning($"[SLGWarpEditor] Failed to load resource: {fullName} (tried: {assetPath})");
            }

            return asset;
        }

        /// <summary>
        /// 将预制体短名解析为资源系统使用的完整名称。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        /// <returns>完整资源名</returns>
        public string GetPrefabFullName(string prefabName)
        {
            // 编辑器模式下返回完整的 Assets 路径
            if (prefabName.StartsWith("Assets/"))
            {
                return prefabName;
            }

            // 构建完整路径
            string fullPath = prefabName;

            // 如果没有扩展名，添加 .prefab
            if (!System.IO.Path.HasExtension(fullPath))
            {
                fullPath += ".prefab";
            }

            // 如果不是绝对路径，添加 SLG 资源目录前缀
            if (!fullPath.StartsWith("Assets/"))
            {
                fullPath = SLGEditDefine.s_SLGInfoPrefab_PathPrefix + fullPath;
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
