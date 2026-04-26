using System;
using UnityEngine;
using static ST.SLG.SLGDefine;

namespace ST.SLG
{
    /// <summary>
    /// 引擎适配接口：向 SLG 包提供摄像机、主玩家位置与资源加载能力，宿主项目需实现或通过 <see cref="SLGWarpInternal"/> 使用默认空实现。
    /// </summary>
	public interface ISLGWarp
	{
        /// <summary>
        /// 是否运行在 Soul 引擎环境下（默认实现恒为 false）。
        /// </summary>
        /// <returns>是否为 Soul 引擎</returns>
        bool IsSoulEngine();

        /// <summary>
        /// 获取主玩家世界坐标，用于视锥与区域裁剪等逻辑。
        /// </summary>
        /// <returns>主玩家位置</returns>
        Vector3 GetMainPlayerPos();

		/// <summary>
		/// 获取主摄像机（通常用于场景渲染与视锥计算）。
		/// </summary>
		/// <returns>主摄像机，未设置时可为 null</returns>
		Camera GetMainCamera();

		/// <summary>
		/// 获取当前激活的摄像机（例如编辑器或运行时切换后的相机）。
		/// </summary>
		/// <returns>当前激活摄像机，未设置时可为 null</returns>
		Camera GetCurrentActiveCamera();

        /// <summary>
        /// 按完整资源名同步加载资源（由宿主资源系统实现）。
        /// </summary>
        /// <param name="fullName">资源完整路径或标识</param>
        /// <returns>加载到的 Unity 对象，失败可为 null</returns>
        UnityEngine.Object GetResource(string fullName);

        /// <summary>
        /// 将预制体短名解析为资源系统使用的完整名称。
        /// </summary>
        /// <param name="prefabName">预制体名或短路径</param>
        /// <returns>完整资源名，无映射时可为空字符串</returns>
        string GetPrefabFullName(string prefabName);

        /// <summary>
        /// 异步或分帧加载资源包路径对应的资源，完成后回调 <paramref name="func"/>。
        /// </summary>
        /// <param name="func">加载完成后的回调</param>
        /// <param name="bundlePath">Bundle 或资源路径</param>
        void StartLoadRes(SLGRun func, string bundlePath);
    }

	/// <summary>
	/// <see cref="ISLGWarp"/> 的默认实现：所有查询返回空值或零，适用于仅占位或未接入宿主引擎时的编译与运行。
	/// </summary>
	public class SLGWarpInternal : ISLGWarp
	{
        /// <inheritdoc />
        public bool IsSoulEngine()
        {
            return false;
        }

        /// <inheritdoc />
        public Vector3 GetMainPlayerPos()
		{
			return Vector3.zero;
		}

        /// <inheritdoc />
        public Camera GetMainCamera()
		{
			return null;
		}

        /// <inheritdoc />
        public Camera GetCurrentActiveCamera()
		{
			return null;
		}

        /// <inheritdoc />
        public UnityEngine.Object GetResource(string fullName)
		{
			return null;
		}

        /// <inheritdoc />
        public string GetPrefabFullName(string prefabName)
        {
            return "";
        }

        /// <inheritdoc />
        public void StartLoadRes(SLGRun func, string bundlePath)
        {

        }
    }

    /// <summary>
    /// 全局引擎适配入口：持有 <see cref="ISLGWarp"/> 实现，业务侧通过 <see cref="S"/> 访问并在启动时调用 <see cref="SetWarp"/> 注入宿主适配器。
    /// </summary>
    public class SLGWarp
	{
		static SLGWarp s_Instance;

		ISLGWarp m_Interface;

        /// <summary>
        /// 单例访问器，首次访问时创建实例并挂载默认 <see cref="SLGWarpInternal"/>。
        /// </summary>
        public static SLGWarp S
		{
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SLGWarp();
                }

                return s_Instance;
            }
        }

		/// <summary>
		/// 当前注入的引擎适配接口实例。
		/// </summary>
        public ISLGWarp warp
        {
            get
            {
                return m_Interface;
            }
        }

		/// <summary>
		/// 设置自定义引擎适配实现（覆盖默认的 <see cref="SLGWarpInternal"/>）。
		/// </summary>
		/// <param name="warpInterface">宿主实现的 <see cref="ISLGWarp"/></param>
        public void SetWarp(ISLGWarp warpInterface)
        {
            m_Interface = warpInterface;

            //Debugger.LogDebugF("SLGWarp SetWarp Name:{0}", warpInterface.GetType().FullName);
        }

        SLGWarp()
		{
            m_Interface = new SLGWarpInternal();
        }
    }
}
