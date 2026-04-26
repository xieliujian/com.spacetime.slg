
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域动态地图图层，管理单个动态地图图层下所有地图块的初始化与渲染。
    /// </summary>
    public class SLGAreaDynamicMapLayer : SLGAreaLayer
    {
        SLGResMgr m_ResMgr;

        SLGAreaDynamicMapLayerDB m_AreaDynamicMapDB;

        List<SLGAreaMapBlock> m_BlockList = new List<SLGAreaMapBlock>();

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器实例。</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置动态地图图层的数据库配置。
        /// </summary>
        /// <param name="areaDynamicMapDB">动态地图图层数据库对象。</param>
        public void SetAreaDynamicMapDB(SLGAreaDynamicMapLayerDB areaDynamicMapDB)
        {
            m_AreaDynamicMapDB = areaDynamicMapDB;
        }

        /// <summary>
        /// 获取当前动态地图图层的索引 ID。
        /// </summary>
        /// <returns>图层 ID，若未设置则返回 -1。</returns>
        public override int GetLayerIndex()
        {
            if (m_AreaDynamicMapDB == null)
                return -1;

            return m_AreaDynamicMapDB.layerID;
        }

        /// <summary>
        /// 初始化动态地图图层，根据数据库配置创建所有地图块。
        /// </summary>
        public void Init()
        {
            if (m_AreaDynamicMapDB == null || m_ResMgr == null)
                return;

            foreach (var blockDB in m_AreaDynamicMapDB.blockList)
            {
                if (blockDB == null)
                    continue;

                SLGRes res = m_ResMgr.FindSceneRes(blockDB.resID);
                if (res == null)
                    continue;

                SLGAreaMapBlock block = new SLGAreaMapBlock();
                block.SetRenderBlockDB(blockDB);
                block.SetMesh(res.mesh);
                block.SetMat(res.mat);
                block.Init();

                m_BlockList.Add(block);
            }
        }

        /// <summary>
        /// 渲染图层内所有地图块。
        /// </summary>
        public override void Render()
        {
            foreach (var block in m_BlockList)
            {
                if (block == null)
                    continue;

                block.Render();
            }
        }

        /// <summary>
        /// 销毁图层，释放所有地图块资源。
        /// </summary>
        public void Destroy()
        {
            foreach (var block in m_BlockList)
            {
                if (block == null)
                    continue;

                block.Destroy();
            }

            m_BlockList.Clear();
        }
    }

    /// <summary>
    /// 区域动态地图图层集合，根据场景当前动态地图索引切换并渲染对应图层。
    /// </summary>
    public class SLGAreaDynamicMapLayerSet
    {
        SLGResMgr m_ResMgr;

        SLGAreaDynamicMapLayerSetDB m_DynamicMapSetDB;

        SLGScene m_Scene;

        Dictionary<int, SLGAreaDynamicMapLayer> m_DynamicMapDict = new Dictionary<int, SLGAreaDynamicMapLayer>();

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器实例。</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置动态地图图层集合的数据库配置。
        /// </summary>
        /// <param name="areaDynamicMapSetDB">动态地图图层集合数据库对象。</param>
        public void SetAreaDynamicMapSetDB(SLGAreaDynamicMapLayerSetDB areaDynamicMapSetDB)
        {
            m_DynamicMapSetDB = areaDynamicMapSetDB;
        }

        /// <summary>
        /// 设置场景引用，用于获取当前动态地图索引。
        /// </summary>
        /// <param name="scene">场景对象。</param>
        public void SetScene(SLGScene scene)
        {
            m_Scene = scene;
        }

        /// <summary>
        /// 根据索引查找对应的动态地图图层。
        /// </summary>
        /// <param name="dynamicMapIndex">动态地图索引。</param>
        /// <returns>对应的动态地图图层，不存在则返回 null。</returns>
        public SLGAreaDynamicMapLayer FindAreaDynamicMap(int dynamicMapIndex)
        {
            SLGAreaDynamicMapLayer dynamicMap = null;
            m_DynamicMapDict.TryGetValue(dynamicMapIndex, out dynamicMap);
            return dynamicMap;
        }

        /// <summary>
        /// 初始化动态地图图层集合，根据数据库配置创建所有动态地图图层。
        /// </summary>
        public void Init()
        {
            if (m_ResMgr == null || m_DynamicMapSetDB == null)
                return;

            Destroy();

            foreach (var dynamicMapDB in m_DynamicMapSetDB.dynamicMapList)
            {
                var index = dynamicMapDB.index;

                SLGAreaDynamicMapLayer dynamicMap = FindAreaDynamicMap(index);
                if (dynamicMap == null)
                {
                    dynamicMap = new SLGAreaDynamicMapLayer();
                    dynamicMap.SetResMgr(m_ResMgr);
                    dynamicMap.SetAreaDynamicMapDB(dynamicMapDB);
                    dynamicMap.Init();

                    m_DynamicMapDict.Add(index, dynamicMap);
                }
            }
        }

        /// <summary>
        /// 渲染场景当前动态地图索引对应的图层。
        /// </summary>
        public void Render()
        {
            SLGAreaDynamicMapLayer dynamicMap = FindAreaDynamicMap(m_Scene.curDynamicMapIndex);
            if (dynamicMap == null)
                return;

            dynamicMap.Render();
        }

        /// <summary>
        /// 销毁动态地图图层集合，释放所有图层资源。
        /// </summary>
        public void Destroy()
        {
            foreach(var iter in m_DynamicMapDict)
            {
                var dynamicMap = iter.Value;
                if (dynamicMap == null)
                    continue;

                dynamicMap.Destroy();
            }

            m_DynamicMapDict.Clear();
        }
    }
}
