using System;
using UnityEngine;
using static ST.SLG.SLGDefine;

namespace ST.SLG
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISLGWarp
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsSoulEngine();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Vector3 GetMainPlayerPos();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Camera GetMainCamera();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Camera GetCurrentActiveCamera();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        UnityEngine.Object GetResource(string fullName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        string GetPrefabFullName(string prefabName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="bundlePath"></param>
        void StartLoadRes(SLGRun func, string bundlePath);
    }

	/// <summary>
	/// 
	/// </summary>
	public class SLGWarpInternal : ISLGWarp
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsSoulEngine()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMainPlayerPos()
		{
			return Vector3.zero;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public Camera GetMainCamera()
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public Camera GetCurrentActiveCamera()
		{
			return null;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public UnityEngine.Object GetResource(string fullName)
		{
			return null;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public string GetPrefabFullName(string prefabName)
        {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="bundlePath"></param>
        public void StartLoadRes(SLGRun func, string bundlePath)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SLGWarp
	{
		/// <summary>
		/// 
		/// </summary>
		static SLGWarp s_Instance;

		/// <summary>
		/// 
		/// </summary>
		ISLGWarp m_Interface;

        /// <summary>
        /// 
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
		/// 
		/// </summary>
        public ISLGWarp warp
        {
            get
            {
                return m_Interface;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="warpInterface"></param>
        public void SetWarp(ISLGWarp warpInterface)
        {
            m_Interface = warpInterface;

            //Debugger.LogDebugF("SLGWarp SetWarp Name:{0}", warpInterface.GetType().FullName);
        }

		/// <summary>
		/// 
		/// </summary>
        SLGWarp()
		{
            m_Interface = new SLGWarpInternal();
        }
    }
}