
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaSet
    {
        /// <summary>
        /// 
        /// </summary>
        SLGSceneDB m_SceneDB;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaSetDB m_AreaSetDB;

        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGScene m_Scene;

        /// <summary>
        /// ±‹√‚GC
        /// </summary>
        Plane[] m_FrustumPlaneArray;

        /// <summary>
        /// 
        /// </summary>
        List<SLGArea> m_AreaList = new List<SLGArea>();

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
        /// <param name="areaSetDB"></param>
        public void SetAreaSetDB(SLGAreaSetDB areaSetDB)
        {
            m_AreaSetDB = areaSetDB;
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
        /// <param name="scene"></param>
        public void SetScene(SLGScene scene)
        {
            m_Scene = scene;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frustumPlaneArray"></param>
        public void SetFrustumPlaneArray(Plane[] frustumPlaneArray)
        {
            m_FrustumPlaneArray = frustumPlaneArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAreaListExist()
        {
            return m_AreaList.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            Destroy();
            InitAreaList();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            foreach (var area in m_AreaList)
            {
                if (area == null)
                    continue;

                area.Destroy();
            }

            m_AreaList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            if (m_AreaList.Count <= 0)
                return;

            if (m_FrustumPlaneArray == null)
                return;

            RenderAreaList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        public void RemoveAreaGridInfo(int layerID, Vector2Int logicPos)
        {
            int areaIndex = SLGUtils.CalcAreaIndexByLogicPos(logicPos);
            if (areaIndex < 0 || areaIndex >= m_AreaList.Count)
                return;

            SLGArea area = m_AreaList[areaIndex];
            if (area == null)
                return;

            area.RemoveAreaGridInfo(layerID, logicPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public void AddAreaGridInfo(int layerID, Vector2Int logicPos, Color color)
        {
            int areaIndex = SLGUtils.CalcAreaIndexByLogicPos(logicPos);
            if (areaIndex < 0 || areaIndex >= m_AreaList.Count)
                return;

            SLGArea area = m_AreaList[areaIndex];
            if (area == null)
                return;

            area.AddAreaGridInfo(layerID, logicPos, color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="tex"></param>
        public void FillMiniMapTexture(int layerID, Texture2D tex)
        {
            foreach(var area in m_AreaList)
            {
                if (area == null)
                    continue;

                area.FillMiniMapTexture(layerID, tex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visible"></param>
        public void SetAreaPropertyLayerVisible(int layerID, bool visible)
        {
            foreach(var area in m_AreaList)
            {
                if (area == null)
                    continue;

                area.SetAreaPropertyLayerVisible(layerID, visible);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        public void SubmitGPUByLayer(int layerID)
        {
            foreach (var area in m_AreaList)
            {
                if (area == null)
                    continue;

                area.AreaInfoSubmitGPU(layerID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="areaIndex"></param>
        public void AreaInfoSubmitGPU(int layerID, int areaIndex)
        {
            if (areaIndex < 0 || areaIndex >= m_AreaList.Count)
                return;

            SLGArea area = m_AreaList[areaIndex];
            if (area == null)
                return;

            area.AreaInfoSubmitGPU(layerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        public void AreaInfoSubmitGPU(int layerID, Vector2Int logicPos)
        {
            int areaIndex = SLGUtils.CalcAreaIndexByLogicPos(logicPos);
            AreaInfoSubmitGPU(layerID, areaIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        void InitAreaList()
        {
            if (m_AreaSetDB == null)
                return;

            var areaDBList = m_AreaSetDB.areaDBList;
            if (areaDBList == null || areaDBList.Count <= 0)
                return;

            for (int i = 0; i < areaDBList.Count; i++)
            {
                SLGAreaDB areaDB = areaDBList[i];
                if (areaDB == null)
                    continue;

                SLGArea area = new SLGArea();
                area.SetResMgr(m_ResMgr);
                area.SetAreaDB(areaDB);
                area.SetAreaIndex(i);
                area.SetAreaInfoLayerSetDB(m_AreaSetDB.infoLayerSet);
                area.SetScene(m_Scene);
                area.Init();
                m_AreaList.Add(area);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void RenderAreaList()
        {
            foreach (var area in m_AreaList)
            {
                if (area == null)
                    continue;

                bool isCull = area.IsAreaCull(m_FrustumPlaneArray);
                if (isCull)
                    continue;

                area.Render();
            }
        }
    }
}

