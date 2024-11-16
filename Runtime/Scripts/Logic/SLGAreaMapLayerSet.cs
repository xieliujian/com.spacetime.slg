
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaMapLayerSet
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaMapLayerSetDB m_AreaMapLayerSetDB;

        /// <summary>
        /// 
        /// </summary>
        List<SLGAreaMapLayer> m_LayerList = new List<SLGAreaMapLayer>();

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
        /// <param name="areaMapLayerSetDB"></param>
        public void SetAreaMapLayerSetDB(SLGAreaMapLayerSetDB areaMapLayerSetDB)
        {
            m_AreaMapLayerSetDB = areaMapLayerSetDB;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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

