using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGScene
    {
        /// <summary>
        /// 
        /// </summary>
        SLGSceneDB m_SceneDB;

        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGSceneProperty m_SceneProp = new SLGSceneProperty();

        /// <summary>
        /// 
        /// </summary>
        SLGAreaSet m_AreaSet = new SLGAreaSet();

        /// <summary>
        /// 避免GC
        /// </summary>
        Plane[] m_FrustumPlaneArray = new Plane[SLGUtils.FRUSTUM_PLANE_NUM];

        /// <summary>
        /// 
        /// </summary>
        SLGSceneLineLayer m_LineLayer = new SLGSceneLineLayer();

        /// <summary>
        /// 
        /// </summary>
        int m_CurDynamicMapIndex;

        /// <summary>
        /// 
        /// </summary>
        public int curDynamicMapIndex
        {
            get { return m_CurDynamicMapIndex; }
            set { m_CurDynamicMapIndex = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        public void SetSceneDB(SLGSceneDB sceneDB)
        {
            m_SceneDB = sceneDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resMgr"></param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetDynamicMapIndex(int index)
        {
            m_CurDynamicMapIndex = index;
            SLGUtils.SetSLGSceneDynamicObjGroup(index);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            InitAreaSet();
            InitSceneProperty();
            InitSceneLineLayer();
            SetDynamicMapIndex(m_CurDynamicMapIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_SceneProp.Destroy();
            m_AreaSet.Destroy();
            m_LineLayer.Destroy();
            m_CurDynamicMapIndex = -1;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public SLGPropertyGridDB FindGridProperty(Vector2Int gridPos)
        {
            return m_SceneProp.FindGridProperty(gridPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        public void RemoveAreaGridInfo(int layerID, Vector2Int logicPos)
        {
            m_AreaSet.RemoveAreaGridInfo(layerID, logicPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public void AddAreaGridInfo(int layerID, Vector2Int logicPos, UnityEngine.Color color)
        {
            m_AreaSet.AddAreaGridInfo(layerID, logicPos, color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="tex"></param>
        public void FillMiniMapTexture(int layerID, Texture2D tex)
        {
            m_AreaSet.FillMiniMapTexture(layerID, tex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="visible"></param>
        public void SetAreaPropertyLayerVisible(int layerID, bool visible)
        {
            m_AreaSet.SetAreaPropertyLayerVisible(layerID, visible);
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
            m_LineLayer.AddSceneLineInfo(uniqueID, startPos, endPos, enemy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        public void RemoveSceneLineInfo(uint uniqueID)
        {
            m_LineLayer.RemoveSceneLineInfo(uniqueID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        public void SubmitGPUByLayer(int layerID)
        {
            // SceneLine是相对场景，修改必然刷新
            m_AreaSet.SubmitGPUByLayer(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        void RenderLineLayer()
        {
            m_LineLayer.Render();
        }

        /// <summary>
        /// 
        /// </summary>
        void RenderAreaSet(Camera cam)
        {
            GeometryUtility.CalculateFrustumPlanes(cam, m_FrustumPlaneArray);
            if (m_FrustumPlaneArray == null)
                return;

            m_AreaSet.Render();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        void InitSceneProperty()
        {
            if (m_SceneDB == null)
                return;

            m_SceneProp.SetScenePropDB(m_SceneDB.propDB);
            m_SceneProp.Init();
        }
    }
}

