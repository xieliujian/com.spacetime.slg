using System.IO;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 编辑器 Play 模式专用的 ISLGWarp 实现，使用 Resources.Load 加载资源。
    /// 仅依赖 UnityEngine runtime API，可在运行时程序集中引用。
    /// </summary>
    public class SLGWarpEditor : ISLGWarp
    {
        Camera m_Camera;

        /// <summary>
        /// 构造函数，传入场景摄像机引用。
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
        public bool IsSoulEngine() => false;

        /// <summary>
        /// 获取主玩家世界坐标（返回摄像机位置）。
        /// </summary>
        public Vector3 GetMainPlayerPos()
        {
            return m_Camera != null ? m_Camera.transform.position : Vector3.zero;
        }

        /// <summary>
        /// 获取主摄像机。
        /// </summary>
        public Camera GetMainCamera() => m_Camera;

        /// <summary>
        /// 获取当前激活的摄像机。
        /// </summary>
        public Camera GetCurrentActiveCamera() => m_Camera;

        /// <summary>
        /// 按 Resources 相对路径同步加载资源。
        /// </summary>
        /// <param name="fullName">Resources 目录下的相对路径（如 "scene/common/res/slg/logicprefab/xxx"）</param>
        public UnityEngine.Object GetResource(string fullName)
        {
            string resourcePath = fullName;
            if (Path.HasExtension(resourcePath))
            {
                resourcePath = Path.ChangeExtension(resourcePath, null).TrimEnd('.');
            }

            var asset = Resources.Load(resourcePath);
            if (asset == null)
            {
                Debug.LogWarning($"[SLGWarpEditor] Failed to load resource: {fullName} (tried: {resourcePath})");
            }

            return asset;
        }

        /// <summary>
        /// 将预制体名映射为 Resources 相对路径。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        public string GetPrefabFullName(string prefabName)
        {
            if (prefabName.StartsWith("Assets/"))
                prefabName = prefabName.Substring(7);

            if (prefabName.Contains("/"))
            {
                if (Path.HasExtension(prefabName))
                    return Path.ChangeExtension(prefabName, null).TrimEnd('.');
                return prefabName;
            }

            return SLGDefine.SLG_LOGIC_PREFAB_PATH + prefabName;
        }

        /// <summary>
        /// 同步回调（编辑器模式下直接执行）。
        /// </summary>
        /// <param name="func">加载完成回调</param>
        /// <param name="bundlePath">Bundle 路径（编辑器模式下忽略）</param>
        public void StartLoadRes(SLGRun func, string bundlePath)
        {
            func?.Invoke();
        }
    }
}
