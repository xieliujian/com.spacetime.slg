
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 区域静态地图图层集合，管理一个区域内所有静态地图图层的生命周期与渲染。
    /// </summary>
    public class SLGAreaMapLayerSet
    {
        SLGResMgr m_ResMgr;

        SLGAreaMapLayerSetDB m_AreaMapLayerSetDB;

        List<SLGAreaMapLayer> m_LayerList = new List<SLGAreaMapLayer>();

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器实例。</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置地图图层集合的数据库配置。
        /// </summary>
        /// <param name="areaMapLayerSetDB">地图图层集合数据库对象。</param>
        public void SetAreaMapLayerSetDB(SLGAreaMapLayerSetDB areaMapLayerSetDB)
        {
            m_AreaMapLayerSetDB = areaMapLayerSetDB;
        }

        /// <summary>
        /// 初始化图层集合，根据数据库配置创建所有静态地图图层。
        /// </summary>
        public void Init()
        {
            if (m_AreaMapLayerSetDB == null || m_ResMgr == null)
                return;

            foreach(var layerDB in m_AreaMapLayerSetDB.layerList)
            {
                if (layerDB == null)
                    continue;

                SLGAreaMapLayer layer = new SLGAreaMapLayer();
                layer.SetResMgr(m_ResMgr);
                layer.SetAreaMapLayerDB(layerDB);
                layer.Init();

                m_LayerList.Add(layer);
            }
        }

        /// <summary>
        /// 渲染集合内所有静态地图图层。
        /// </summary>
        public void Render()
        {
            foreach (var layer in m_LayerList)
            {
                if (layer == null)
                    continue;

                layer.Render();
            }
        }

        /// <summary>
        /// 销毁图层集合，释放所有图层资源。
        /// </summary>
        public void Destroy()
        {
            foreach (var layer in m_LayerList)
            {
                if (layer == null)
                    continue;

                layer.Destroy();
            }

            m_LayerList.Clear();
        }
    }
}
