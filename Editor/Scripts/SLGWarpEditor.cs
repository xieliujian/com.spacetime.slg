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
        /// 按完整资源名同步加载资源（使用 Resources.Load）。
        /// </summary>
        /// <param name="fullName">资源相对路径（Resources 目录下的相对路径，如 "scene/common/res/slg/logicprefab/xxx"）</param>
        /// <returns>加载到的 Unity 对象</returns>
        public UnityEngine.Object GetResource(string fullName)
        {
            // 使用 Resources.Load 加载资源
            // fullName 应该是 Resources 目录下的相对路径
            // 例如: "scene/common/res/slg/logicprefab/slg_share_grid"

            // 去掉扩展名（Resources.Load 不需要扩展名）
            string resourcePath = fullName;
            if (System.IO.Path.HasExtension(resourcePath))
            {
                resourcePath = System.IO.Path.ChangeExtension(resourcePath, null).TrimEnd('.');
            }

            var asset = Resources.Load(resourcePath);

            if (asset == null)
            {
                UnityEngine.Debug.LogWarning($"[SLGWarpEditor] Failed to load resource: {fullName} (tried: {resourcePath})");
            }

            return asset;
        }

        /// <summary>
        /// 将预制体短名解析为资源系统使用的完整名称（返回 Resources 相对路径）。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        /// <returns>Resources 目录下的完整相对路径</returns>
        public string GetPrefabFullName(string prefabName)
        {
            // 返回 Resources 目录下的相对路径
            // 例如: "scene/common/res/slg/logicprefab/slg_share_grid"

            if (prefabName.StartsWith("Assets/"))
            {
                prefabName = prefabName.Substring(7); // 去掉 "Assets/"
            }

            // 如果已经包含路径前缀，直接返回
            if (prefabName.Contains("/"))
            {
                // 去掉扩展名
                if (System.IO.Path.HasExtension(prefabName))
                {
                    return System.IO.Path.ChangeExtension(prefabName, null).TrimEnd('.');
                }
                return prefabName;
            }

            // 构建完整相对路径
            string fullPath = SLGDefine.SLG_LOGIC_PREFAB_PATH + prefabName;

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
