using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 区域信息图层集合，管理一个区域内所有信息图层（网格图层、属性图层）的生命周期与渲染。
    /// </summary>
    public class SLGAreaInfoLayerSet
    {
        SLGResMgr m_ResMgr;

        int m_AreaIndex;

        SLGAreaInfoLayerSetDB m_AreaInfoLayerSetDB;

        SLGAreaGridSetDB m_AreaGridSetDB;

        Dictionary<int, SLGAreaInfoLayer> m_AreaLayerDict = new Dictionary<int, SLGAreaInfoLayer>();

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器实例。</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 设置当前区域索引，用于属性图层定位数据。
        /// </summary>
        /// <param name="index">区域索引。</param>
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 设置信息图层集合的数据库配置。
        /// </summary>
        /// <param name="areaInfoLayerSetDB">信息图层集合数据库对象。</param>
        public void SetAreaInfoLayerSetDB(SLGAreaInfoLayerSetDB areaInfoLayerSetDB)
        {
            m_AreaInfoLayerSetDB = areaInfoLayerSetDB;
        }

        /// <summary>
        /// 设置区域网格集合数据库，供网格信息图层使用。
        /// </summary>
        /// <param name="areaGridSetDB">区域网格集合数据库对象。</param>
        public void SetAreaGridSetDB(SLGAreaGridSetDB areaGridSetDB)
        {
            m_AreaGridSetDB = areaGridSetDB;
        }

        /// <summary>
        /// 初始化图层集合，根据数据库配置创建所有信息图层。
        /// </summary>
        public void Init()
        {
            InitLayerDict();
        }

        /// <summary>
        /// 渲染集合内所有信息图层。
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
        /// 销毁图层集合，释放所有信息图层资源。
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
        /// 根据图层 ID 查找对应的信息图层。
        /// </summary>
        /// <param name="layerID">图层 ID。</param>
        /// <returns>对应的信息图层，不存在则返回 null。</returns>
        public SLGAreaInfoLayer FindAreaInfoLayer(int layerID)
        {
            SLGAreaInfoLayer block = null;
            m_AreaLayerDict.TryGetValue(layerID, out block);
            return block;
        }

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
