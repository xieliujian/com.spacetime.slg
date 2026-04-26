using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// SLG 场景运行时根对象：管理 25 块区域集合、格子属性查询、场景连线和当前动态地表索引，与 <see cref="SLGSceneDB"/> 配置对应。
    /// </summary>
    public class SLGScene
    {
        SLGSceneDB m_SceneDB;

        SLGResMgr m_ResMgr;

        SLGSceneProperty m_SceneProp = new SLGSceneProperty();

        SLGAreaSet m_AreaSet = new SLGAreaSet();

        // 视锥平面缓存，用于区域裁剪，避免每帧分配
        Plane[] m_FrustumPlaneArray = new Plane[SLGUtils.FRUSTUM_PLANE_NUM];

        SLGSceneLineLayer m_LineLayer = new SLGSceneLineLayer();

        int m_CurDynamicMapIndex;

        /// <summary>
        /// 当前选中的动态地表索引（与动态物体组显示联动）。
        /// </summary>
        public int curDynamicMapIndex
        {
            get { return m_CurDynamicMapIndex; }
            set { m_CurDynamicMapIndex = value; }
        }

        /// <summary>
        /// 绑定场景配置数据（ScriptableObject）。
        /// </summary>
        /// <param name="sceneDB">场景 DB</param>
        public void SetSceneDB(SLGSceneDB sceneDB)
        {
            m_SceneDB = sceneDB;
        }

        /// <summary>
        /// 绑定资源管理器（网格、材质、自定义资源加载）。
        /// </summary>
        /// <param name="resMgr">资源管理器</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置当前动态地表索引并同步场景内动态物体组的可见性。
        /// </summary>
        /// <param name="index">动态地表索引</param>
        public void SetDynamicMapIndex(int index)
        {
            m_CurDynamicMapIndex = index;
            SLGUtils.SetSLGSceneDynamicObjGroup(index);
        }

        /// <summary>
        /// 初始化区域集、属性数据与连线层。
        /// </summary>
        public void Init()
        {
            InitAreaSet();
            InitSceneProperty();
            InitSceneLineLayer();
            SetDynamicMapIndex(m_CurDynamicMapIndex);
        }

        /// <summary>
        /// 释放场景内各子系统资源。
        /// </summary>
        public void Destroy()
        {
            m_SceneProp.Destroy();
            m_AreaSet.Destroy();
            m_LineLayer.Destroy();
            m_CurDynamicMapIndex = -1;
        }

        /// <summary>
        /// 每帧渲染：视锥裁剪后绘制可见区域，并绘制场景连线层。
        /// </summary>
        public void Render()
        {
            if (m_AreaSet == null || !m_AreaSet.IsAreaListExist())
                return;

            Camera cam = SLGWarp.S.warp.GetMainCamera();
            if (cam == null || cam.cullingMask == 0)
                return;

            RenderAreaSet(cam);
            RenderLineLayer();
        }

        /// <summary>
        /// 按逻辑格子坐标查询属性格子数据。
        /// </summary>
        /// <param name="gridPos">逻辑格子坐标</param>
        /// <returns>属性 DB，不存在则返回 null</returns>
        public SLGPropertyGridDB FindGridProperty(Vector2Int gridPos)
        {
            return m_SceneProp.FindGridProperty(gridPos);
        }

        /// <summary>
        /// 在指定信息层上移除某格子的覆盖数据。
        /// </summary>
        /// <param name="layerID">信息层 ID（与配置一致）</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        public void RemoveAreaGridInfo(int layerID, Vector2Int logicPos)
        {
            m_AreaSet.RemoveAreaGridInfo(layerID, logicPos);
        }

        /// <summary>
        /// 在指定信息层上为某格子添加或更新颜色覆盖。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        /// <param name="color">显示颜色</param>
        public void AddAreaGridInfo(int layerID, Vector2Int logicPos, UnityEngine.Color color)
        {
            m_AreaSet.AddAreaGridInfo(layerID, logicPos, color);
        }

        /// <summary>
        /// 将指定信息层当前数据绘制到小地图纹理。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="tex">目标纹理</param>
        public void FillMiniMapTexture(int layerID, Texture2D tex)
        {
            m_AreaSet.FillMiniMapTexture(layerID, tex);
        }

        /// <summary>
        /// 设置某属性/信息层是否参与渲染。
        /// </summary>
        /// <param name="layerID">层 ID</param>
        /// <param name="visible">是否可见</param>
        public void SetAreaPropertyLayerVisible(int layerID, bool visible)
        {
            m_AreaSet.SetAreaPropertyLayerVisible(layerID, visible);
        }

        /// <summary>
        /// 添加或更新一条场景连线（世界坐标端点）。
        /// </summary>
        /// <param name="uniqueID">连线唯一 ID</param>
        /// <param name="startPos">起点世界坐标</param>
        /// <param name="endPos">终点世界坐标</param>
        /// <param name="enemy">是否为敌方样式</param>
        public void AddSceneLineInfo(uint uniqueID, Vector3 startPos, Vector3 endPos, bool enemy)
        {
            m_LineLayer.AddSceneLineInfo(uniqueID, startPos, endPos, enemy);
        }

        /// <summary>
        /// 按唯一 ID 移除场景连线。
        /// </summary>
        /// <param name="uniqueID">连线唯一 ID</param>
        public void RemoveSceneLineInfo(uint uniqueID)
        {
            m_LineLayer.RemoveSceneLineInfo(uniqueID);
        }

        /// <summary>
        /// 将指定信息层上未提交的 GPU 数据提交（例如批量改色后调用）。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        public void SubmitGPUByLayer(int layerID)
        {
            // SceneLine 相对场景，修改必然刷新
            m_AreaSet.SubmitGPUByLayer(layerID);
        }

        void RenderLineLayer()
        {
            m_LineLayer.Render();
        }

        void RenderAreaSet(Camera cam)
        {
            GeometryUtility.CalculateFrustumPlanes(cam, m_FrustumPlaneArray);
            if (m_FrustumPlaneArray == null)
                return;

            m_AreaSet.Render();
        }

        void InitAreaSet()
        {
            if (m_SceneDB == null)
                return;

            m_AreaSet.SetResMgr(m_ResMgr);
            m_AreaSet.SetSceneDB(m_SceneDB);
            m_AreaSet.SetAreaSetDB(m_SceneDB.areaSetDB);
            m_AreaSet.SetFrustumPlaneArray(m_FrustumPlaneArray);
            m_AreaSet.SetScene(this);
            m_AreaSet.Init();
        }

        void InitSceneLineLayer()
        {
            var areaSetDB = m_SceneDB.areaSetDB;
            if (areaSetDB == null)
                return;

            var infoLayerSet = areaSetDB.infoLayerSet;
            if (infoLayerSet == null)
                return;

            SLGAreaInfoLayerDB infoLayer = infoLayerSet.GetLineLayer();
            if (infoLayer == null)
                return;

            m_LineLayer.SetResMgr(m_ResMgr);
            m_LineLayer.SetAreaInfoLayerDB(infoLayer);
            m_LineLayer.Init();
        }

        void InitSceneProperty()
        {
            if (m_SceneDB == null)
                return;

            m_SceneProp.SetScenePropDB(m_SceneDB.propDB);
            m_SceneProp.Init();
        }
    }
}
