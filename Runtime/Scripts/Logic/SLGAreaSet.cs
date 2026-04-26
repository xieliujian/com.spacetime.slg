using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 全图 25 个 <see cref="SLGArea"/> 的管理器：负责初始化、视锥裁剪下的渲染，以及按逻辑坐标分发格子信息、小地图与显隐等操作。
    /// </summary>
    public class SLGAreaSet
    {
        SLGSceneDB m_SceneDB;

        SLGAreaSetDB m_AreaSetDB;

        SLGResMgr m_ResMgr;

        SLGScene m_Scene;

        // 与 SLGScene 共用视锥平面缓冲，避免 GC
        Plane[] m_FrustumPlaneArray;

        List<SLGArea> m_AreaList = new List<SLGArea>();

        /// <summary>
        /// 绑定顶层场景配置（用于扩展，部分流程使用 <see cref="SetAreaSetDB"/>）。
        /// </summary>
        /// <param name="sceneDB">场景 DB</param>
        public void SetSceneDB(SLGSceneDB sceneDB)
        {
            m_SceneDB = sceneDB;
        }

        /// <summary>
        /// 绑定区域集合数据（25 块区域的 DB 列表与信息层配置）。
        /// </summary>
        /// <param name="areaSetDB">区域集 DB</param>
        public void SetAreaSetDB(SLGAreaSetDB areaSetDB)
        {
            m_AreaSetDB = areaSetDB;
        }

        /// <summary>
        /// 注入资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置所属场景（传入各 <see cref="SLGArea"/>）。
        /// </summary>
        /// <param name="scene">场景对象</param>
        public void SetScene(SLGScene scene)
        {
            m_Scene = scene;
        }

        /// <summary>
        /// 设置与 <see cref="SLGScene"/> 共享的视锥平面数组。
        /// </summary>
        /// <param name="frustumPlaneArray">6 个视锥平面</param>
        public void SetFrustumPlaneArray(Plane[] frustumPlaneArray)
        {
            m_FrustumPlaneArray = frustumPlaneArray;
        }

        /// <summary>
        /// 是否已成功创建至少一个区域实例。
        /// </summary>
        /// <returns>区域列表非空返回 true</returns>
        public bool IsAreaListExist()
        {
            return m_AreaList.Count > 0;
        }

        /// <summary>
        /// 根据 DB 构建全部区域并初始化子层。
        /// </summary>
        public void Init()
        {
            Destroy();
            InitAreaList();
        }

        /// <summary>
        /// 销毁所有区域子系统并清空列表。
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
        /// 对所有未被视锥剔除的区域调用 <see cref="SLGArea.Render"/>。
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
        /// 按逻辑坐标定位到所属区域，并移除该格在指定层上的信息。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
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
        /// 按逻辑坐标定位到所属区域，并添加格子信息颜色。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        /// <param name="color">颜色</param>
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
        /// 对所有区域执行小地图纹理填充（每层聚合）。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="tex">目标纹理</param>
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
        /// 设置所有区域上指定信息层的可见性。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="visible">是否可见</param>
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
        /// 对所有区域提交指定信息层的 GPU 实例数据。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
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
        /// 对指定区域索引提交该层 GPU 数据。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="areaIndex">区域索引 0-24</param>
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
        /// 根据逻辑坐标解析区域索引，并提交该层 GPU 数据。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        public void AreaInfoSubmitGPU(int layerID, Vector2Int logicPos)
        {
            int areaIndex = SLGUtils.CalcAreaIndexByLogicPos(logicPos);
            AreaInfoSubmitGPU(layerID, areaIndex);
        }

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
