using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单个区域（10×10 逻辑格）的运行时对象：包含静态地图层、动态地图层与信息层集合。
    /// </summary>
    public class SLGArea
    {
        SLGResMgr m_ResMgr;

        SLGScene m_Scene;

        SLGAreaInfoLayerSetDB m_AreaInfoLayerSetDB;

        SLGAreaDB m_AreaDB;

        int m_AreaIndex;

        SLGAreaMapLayerSet m_AreaMapLayerSet = new SLGAreaMapLayerSet();

        SLGAreaDynamicMapLayerSet m_AreaDynamicMapLayerSet = new SLGAreaDynamicMapLayerSet();

        SLGAreaInfoLayerSet m_AreaInfoLayerSet = new SLGAreaInfoLayerSet();

        /// <summary>
        /// 注入资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 绑定本区域的数据配置。
        /// </summary>
        /// <param name="areaDB">区域 DB</param>
        public void SetAreaDB(SLGAreaDB areaDB)
        {
            m_AreaDB = areaDB;
        }

        /// <summary>
        /// 设置区域在 5×5 区域阵中的索引（0–24）。
        /// </summary>
        /// <param name="index">区域索引</param>
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 绑定本区域的信息层配置集合。
        /// </summary>
        /// <param name="areaInfoLayerSetDB">信息层集合 DB</param>
        public void SetAreaInfoLayerSetDB(SLGAreaInfoLayerSetDB areaInfoLayerSetDB)
        {
            m_AreaInfoLayerSetDB = areaInfoLayerSetDB;
        }

        /// <summary>
        /// 设置所属场景，供动态地图层访问当前动态地表索引等。
        /// </summary>
        /// <param name="scene">场景对象</param>
        public void SetScene(SLGScene scene)
        {
            m_Scene = scene;
        }

        /// <summary>
        /// 初始化地图层与信息层子系统。
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
        /// 释放本区域内所有渲染与缓冲资源。
        /// </summary>
        public void Destroy()
        {
            m_AreaMapLayerSet.Destroy();
            m_AreaDynamicMapLayerSet.Destroy();
            m_AreaInfoLayerSet.Destroy();
        }

        /// <summary>
        /// 渲染本区域地图层与信息层。
        /// </summary>
        public void Render()
        {
            m_AreaMapLayerSet.Render();
            m_AreaDynamicMapLayerSet.Render();
            m_AreaInfoLayerSet.Render();
        }

        /// <summary>
        /// 使用视锥平面与本区域 AABB 做快速裁剪测试。
        /// </summary>
        /// <param name="frustumPlaneArray">视锥平面数组</param>
        /// <returns>完全在视锥外返回 true，表示可跳过渲染</returns>
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
        /// 在指定信息层移除某格子的覆盖数据。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        public void RemoveAreaGridInfo(int layerID, Vector2Int logicPos)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.RemoveGridInfo(logicPos);
        }

        /// <summary>
        /// 在指定信息层为某格子添加或更新颜色。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="logicPos">逻辑格子坐标</param>
        /// <param name="color">颜色</param>
        public void AddAreaGridInfo(int layerID, Vector2Int logicPos, Color color)
        {
            SLGAreaInfoLayer infoLayer = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoLayer == null)
                return;

            infoLayer.AddGridInfo(logicPos, color);
        }

        /// <summary>
        /// 将小地图绘制请求转发到对应信息层。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="tex">目标纹理</param>
        public void FillMiniMapTexture(int layerID, Texture2D tex)
        {
            SLGAreaInfoLayer infoLayer = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoLayer == null)
                return;

            infoLayer.FillMiniMapTexture(layerID, tex);
        }

        /// <summary>
        /// 设置指定信息层是否渲染。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        /// <param name="visible">是否可见</param>
        public void SetAreaPropertyLayerVisible(int layerID, bool visible)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.SetRender(visible);
        }

        /// <summary>
        /// 将指定信息层的 Instance 缓冲提交到 GPU（批量更新后调用）。
        /// </summary>
        /// <param name="layerID">信息层 ID</param>
        public void AreaInfoSubmitGPU(int layerID)
        {
            SLGAreaInfoLayer infoBlock = m_AreaInfoLayerSet.FindAreaInfoLayer(layerID);
            if (infoBlock == null)
                return;

            infoBlock.SubmitGPU();
        }

    }
}
