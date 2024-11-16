using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGSceneMgr
    {
        /// <summary>
        /// 
        /// </summary>
        static SLGSceneMgr s_Instance;

        /// <summary>
        /// 
        /// </summary>
        public static SLGSceneMgr S
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SLGSceneMgr();
                }

                return s_Instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        SLGScene m_Scene = new SLGScene();

        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr = new SLGResMgr();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        public void Init(SLGSceneDB sceneDB)
        {
            if (sceneDB == null)
            {
                Destroy();
                return;
            }

            m_ResMgr.SetResDB(sceneDB.resDB);
            m_ResMgr.Init();

            m_Scene.SetSceneDB(sceneDB);
            m_Scene.SetResMgr(m_ResMgr);
            m_Scene.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_Update");
#endif
            
            m_Scene.Render();

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_ResMgr.Destroy();
            m_Scene.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public SLGPropertyGridDB FindGridProperty(Vector2Int gridPos)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_FindGridProperty");
#endif

            return m_Scene.FindGridProperty(gridPos);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public void RemoveAreaGridInfo(SLGDefine.SLGInfoLayer layerType, Vector2Int logicPos)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_RemoveAreaGridInfo");
#endif

            m_Scene.RemoveAreaGridInfo((int)layerType, logicPos);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public void AddAreaGridInfo(SLGDefine.SLGInfoLayer layerType, Vector2Int logicPos, UnityEngine.Color color)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_AddAreaGridInfo");
#endif

            m_Scene.AddAreaGridInfo((int)layerType, logicPos, color);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public void SetAreaPropertyLayerVisible(SLGDefine.SLGInfoLayer layerType, bool visible)
        {
            m_Scene.SetAreaPropertyLayerVisible((int)layerType, visible);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="enemy"></param>
        public void AddSceneLineInfo(uint uniqueID, Vector3 startPos, Vector3 endPos, bool enemy)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_AddSceneLineInfo");
#endif

            m_Scene.AddSceneLineInfo(uniqueID, startPos, endPos, enemy);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="logicStartPos"></param>
        /// <param name="logicEndPos"></param>
        /// <param name="enemy"></param>
        public void AddSceneLineInfo(uint uniqueID, Vector2Int logicStartPos, Vector2Int logicEndPos, bool enemy)
        {
            Vector3 startPos = SLGUtils.ConvertSLGLogicPosTo3DPos(logicStartPos);
            Vector3 endPos = SLGUtils.ConvertSLGLogicPosTo3DPos(logicEndPos);
            AddSceneLineInfo(uniqueID, startPos, endPos, enemy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        public void RemoveSceneLineInfo(uint uniqueID)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_RemoveSceneLineInfo");
#endif

            m_Scene.RemoveSceneLineInfo(uniqueID);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        public void SubmitGPUByLayer(SLGDefine.SLGInfoLayer layerType)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_SubmitGPUByLayer");
#endif

            m_Scene.SubmitGPUByLayer((int)layerType);

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void FillMiniMapTexture(Texture2D tex, SLGDefine.SLGInfoLayer layerType)
        {
#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.BeginSample("SLGSceneMgr_FillMiniMapTexture");
#endif
            if (tex != null)
            {
                m_Scene.FillMiniMapTexture((int)layerType, tex);
                tex.Apply();
            }

#if DEBUG_MODE
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetDynamicMapIndex(int index)
        {
            m_Scene.SetDynamicMapIndex(index);
        }
    }
}



