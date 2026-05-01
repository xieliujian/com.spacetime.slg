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
        /// <param name="fullName">资源完整路径或标识</param>
        /// <returns>加载到的 Unity 对象</returns>
        public UnityEngine.Object GetResource(string fullName)
        {
            return Resources.Load(fullName);
        }

        /// <summary>
        /// 将预制体短名解析为资源系统使用的完整名称。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        /// <returns>完整资源名</returns>
        public string GetPrefabFullName(string prefabName)
        {
            if (prefabName.StartsWith("slg_"))
            {
                return "slg/logicprefab/" + prefabName;
            }
            return prefabName;
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
