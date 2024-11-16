using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaInfoLayerSet
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        int m_AreaIndex;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaInfoLayerSetDB m_AreaInfoLayerSetDB;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaGridSetDB m_AreaGridSetDB;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<int, SLGAreaInfoLayer> m_AreaLayerDict = new Dictionary<int, SLGAreaInfoLayer>();

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
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoLayerSetDB"></param>
        public void SetAreaInfoLayerSetDB(SLGAreaInfoLayerSetDB areaInfoLayerSetDB)
        {
            m_AreaInfoLayerSetDB = areaInfoLayerSetDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaGridDB"></param>
        public void SetAreaGridSetDB(SLGAreaGridSetDB areaGridSetDB)
        {
            m_AreaGridSetDB = areaGridSetDB;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            InitLayerDict();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            foreach (var iter in m_AreaLayerDict)
            {
                var layer = iter.Value;
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
            foreach(var iter in m_AreaLayerDict)
            {
                SLGAreaInfoLayer layer = iter.Value;
                if (layer == null)
                    continue;

                layer.Destroy();
            }

            m_AreaLayerDict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <returns></returns>
        public SLGAreaInfoLayer FindAreaInfoLayer(int layerID)
        {
            SLGAreaInfoLayer block = null;
            m_AreaLayerDict.TryGetValue(layerID, out block);
            return block;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitLayerDict()
        {
            Destroy();

            if (m_AreaInfoLayerSetDB == null)
                return;

            foreach (var layerDB in m_AreaInfoLayerSetDB.layerList)
            {
                var findBlock = FindAreaInfoLayer(layerDB.layerID);
                if (findBlock != null)
                    continue;

                if (layerDB.IsAreaGridType())
                {
                    InitGridLayer(layerDB);
                }
            }

            foreach(var layerDB in m_AreaInfoLayerSetDB.propertyLayerList)
            {
                var findBlock = FindAreaInfoLayer(layerDB.layerID);
                if (findBlock != null)
                    continue;

                if (layerDB.IsAreaResLvStateLayer())
                {
                    InitAreaResLvStateLayer(layerDB);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerDB"></param>
        void InitAreaResLvStateLayer(SLGAreaPropertyInfoLayerDB layerDB)
        {
            SLGRes res = m_ResMgr.FindCustomRes(layerDB.resPath);
            if (res == null)
                return;

            SLGAreaPropertyInfoLayer layer = new SLGAreaPropertyInfoLayer();
            layer.SetAreaIndex(m_AreaIndex);
            layer.SetAreaInfoLayerDB(layerDB);
            layer.SetAreaPropertyInfoLayerDB(layerDB);
            layer.SetMesh(res.mesh);
            layer.SetMat(res.mat);
            layer.CalcInitScaleMatrix(res.meshScale);
            layer.Init();

            m_AreaLayerDict.Add(layerDB.layerID, layer);
        }

        /// <summary>
        /// 
        /// </summary>
        void InitGridLayer(SLGAreaInfoLayerDB layerDB)
        {
            SLGRes res = m_ResMgr.FindCustomRes(layerDB.resPath);
            if (res == null)
                return;

            SLGAreaGridInfoLayer layer = new SLGAreaGridInfoLayer();
            layer.SetAreaInfoLayerDB(layerDB);
            layer.SetAreaGridSetDB(m_AreaGridSetDB);
            layer.SetMesh(res.mesh);
            layer.SetMat(res.mat);
            layer.CalcInitScaleMatrix(res.meshScale);
            layer.Init();

            m_AreaLayerDict.Add(layerDB.layerID, layer);
        }
    }
}

