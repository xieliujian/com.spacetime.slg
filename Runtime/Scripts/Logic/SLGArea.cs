using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGArea
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGScene m_Scene;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaInfoLayerSetDB m_AreaInfoLayerSetDB;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaDB m_AreaDB;

        /// <summary>
        /// 
        /// </summary>
        int m_AreaIndex;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaMapLayerSet m_AreaMapLayerSet = new SLGAreaMapLayerSet();

        /// <summary>
        /// 
        /// </summary>
        SLGAreaDynamicMapLayerSet m_AreaDynamicMapLayerSet = new SLGAreaDynamicMapLayerSet();

        /// <summary>
        /// 
        /// </summary>
        SLGAreaInfoLayerSet m_AreaInfoLayerSet = new SLGAreaInfoLayerSet();

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
        /// <param name="areaDB"></param>
        public void SetAreaDB(SLGAreaDB areaDB)
        {
            m_AreaDB = areaDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaInfoLayerSetDB"></param>
        public void SetAreaInfoLayerSetDB(SLGAreaInfoLayerSetDB areaInfoLayerSetDB)
        {
            m_AreaInfoLayerSetDB = areaInfoLayerSetDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        public void SetScene(SLGScene scene)
        {
            m_Scene = scene;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            Destroy();

            m_AreaMapLayerSet.SetResMgr(m_ResMgr);
            m_AreaMapLayerSet.SetAreaMapLayerSetDB(m_AreaDB.mapLayerSet);
            m_AreaMapLayerSet.Init();

            m_AreaDynamicMapLayerSet.SetResMgr(m_ResMgr);
            m_AreaDynamicMapLayerSet.SetAreaDynamicMapSetDB(m_AreaDB.dynamicMapLayerSet);
            m_AreaDynamicMapLayerSet.SetScene(m_Scene);
            m_AreaDynamicMapLayerSet.Init();

            m_AreaInfoLayerSet.SetResMgr(m_ResMgr);
            m_AreaInfoLayerSet.SetAreaIndex(m_AreaIndex);
            m_AreaInfoLayerSet.SetAreaInfoLayerSetDB(m_AreaInfoLayerSetDB);
            m_AreaInfoLayerSet.SetAreaGridSetDB(m_AreaDB.gridSet);
            m_AreaInfoLayerSet.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_AreaMapLayerSet.Destroy();
            m_AreaDynamicMapLayerSet.Destroy();
            m_AreaInfoLayerSet.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            m_AreaMapLayerSet.Render();
            m_AreaDynamicMapLayerSet.Render();
            m_AreaInfoLayerSet.Render();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frustumPlaneArray"></param>
        /// <returns></returns>
        public bool IsAreaCull(Plane[] frustumPlaneArray)
        {
            if (m_AreaDB == null)
                return true;

            SLGUtils.TestPlanesResults result = SLGUtils.TestPlanesAABBInternalFast(frustumPlaneArray, ref m_AreaDB.boundMin, ref m_AreaDB.boundMax);
            if (result == SLGUtils.TestPlanesResults.Outside)
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        public void RemoveAreaGridInfo(int layerID, Vector2Int logicPos)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.RemoveGridInfo(logicPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public void AddAreaGridInfo(int layerID, Vector2Int logicPos, Color color)
        {
            SLGAreaInfoLayer infoLayer = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoLayer == null)
                return;

            infoLayer.AddGridInfo(logicPos, color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="tex"></param>
        public void FillMiniMapTexture(int layerID, Texture2D tex)
        {
            SLGAreaInfoLayer infoLayer = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoLayer == null)
                return;

            infoLayer.FillMiniMapTexture(layerID, tex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visible"></param>
        public void SetAreaPropertyLayerVisible(int layerID, bool visible)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.SetRender(visible);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        public void AreaInfoSubmitGPU(int layerID)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.SubmitGPU();
        }

    }
}

