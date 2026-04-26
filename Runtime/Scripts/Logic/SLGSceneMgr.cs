using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG场景管理器，负责管理场景的初始化、更新、销毁及各类场景信息的增删操作。
    /// </summary>
    public partial class SLGSceneMgr
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        static SLGSceneMgr s_Instance;

        /// <summary>
        /// 获取单例实例
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
        /// 场景实例
        /// </summary>
        SLGScene m_Scene = new SLGScene();

        /// <summary>
        /// 资源管理器实例
        /// </summary>
        SLGResMgr m_ResMgr = new SLGResMgr();

        /// <summary>
        /// 使用场景配置初始化场景管理器。
        /// </summary>
        /// <param name="sceneDB">场景配置数据。</param>
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
        /// 每帧更新，驱动场景渲染。
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
        /// 销毁场景管理器，释放所有资源。
        /// </summary>
        public void Destroy()
        {
            m_ResMgr.Destroy();
            m_Scene.Destroy();
        }

        /// <summary>
        /// 根据格子坐标查找对应的格子属性数据。
        /// </summary>
        /// <param name="gridPos">格子逻辑坐标。</param>
        /// <returns>对应的格子属性数据，不存在则返回 null。</returns>
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
        /// 移除指定图层上某个逻辑坐标的区域格子信息。
        /// </summary>
        /// <param name="layerType">信息图层类型。</param>
        /// <param name="logicPos">格子逻辑坐标。</param>
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
        /// 向指定图层的逻辑坐标添加区域格子信息。
        /// </summary>
        /// <param name="layerType">信息图层类型。</param>
        /// <param name="logicPos">格子逻辑坐标。</param>
        /// <param name="color">格子颜色。</param>
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
        /// 设置指定区域属性图层的显示状态。
        /// </summary>
        /// <param name="layerType">信息图层类型。</param>
        /// <param name="visible">是否可见。</param>
        public void SetAreaPropertyLayerVisible(SLGDefine.SLGInfoLayer layerType, bool visible)
        {
            m_Scene.SetAreaPropertyLayerVisible((int)layerType, visible);
        }

        /// <summary>
        /// 使用3D世界坐标添加场景连线信息。
        /// </summary>
        /// <param name="uniqueID">连线唯一标识。</param>
        /// <param name="startPos">起始3D坐标。</param>
        /// <param name="endPos">结束3D坐标。</param>
        /// <param name="enemy">是否为敌方连线。</param>
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
        /// 使用逻辑格子坐标添加场景连线信息，内部自动转换为3D坐标。
        /// </summary>
        /// <param name="uniqueID">连线唯一标识。</param>
        /// <param name="logicStartPos">起始逻辑格子坐标。</param>
        /// <param name="logicEndPos">结束逻辑格子坐标。</param>
        /// <param name="enemy">是否为敌方连线。</param>
        public void AddSceneLineInfo(uint uniqueID, Vector2Int logicStartPos, Vector2Int logicEndPos, bool enemy)
        {
            Vector3 startPos = SLGUtils.ConvertSLGLogicPosTo3DPos(logicStartPos);
            Vector3 endPos = SLGUtils.ConvertSLGLogicPosTo3DPos(logicEndPos);
            AddSceneLineInfo(uniqueID, startPos, endPos, enemy);
        }

        /// <summary>
        /// 根据唯一标识移除场景连线信息。
        /// </summary>
        /// <param name="uniqueID">连线唯一标识。</param>
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
        /// 将指定图层的数据提交到GPU进行渲染。
        /// </summary>
        /// <param name="layerType">信息图层类型。</param>
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
        /// 将指定图层的区域信息填充到小地图纹理中。
        /// </summary>
        /// <param name="tex">目标小地图纹理。</param>
        /// <param name="layerType">信息图层类型。</param>
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
        /// 设置当前动态地图索引，控制动态对象组的显示。
        /// </summary>
        /// <param name="index">动态地图索引。</param>
        public void SetDynamicMapIndex(int index)
        {
            m_Scene.SetDynamicMapIndex(index);
        }
    }
}



