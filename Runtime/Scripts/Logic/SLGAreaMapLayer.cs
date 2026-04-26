using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域静态地图图层，管理该图层下所有地图块的初始化与渲染。
    /// </summary>
    public class SLGAreaMapLayer : SLGAreaLayer
    {
        SLGResMgr m_ResMgr;

        SLGAreaMapLayerDB m_AreaMapLayerDB;

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
        /// 设置地图图层数据库配置。
        /// </summary>
        /// <param name="areaMapLayerDB">地图图层数据库对象。</param>
        public void SetAreaMapLayerDB(SLGAreaMapLayerDB areaMapLayerDB)
        {
            m_AreaMapLayerDB = areaMapLayerDB;
        }

        /// <summary>
        /// 获取当前图层的索引 ID。
        /// </summary>
        /// <returns>图层 ID，若未设置则返回 -1。</returns>
        public override int GetLayerIndex()
        {
            if (m_AreaMapLayerDB == null)
                return -1;

            return m_AreaMapLayerDB.layerID;
        }

        /// <summary>
        /// 初始化图层，根据数据库配置创建所有地图块。
        /// </summary>
        public void Init()
        {
            if (m_AreaMapLayerDB == null || m_ResMgr == null)
                return;

            foreach (var blockDB in m_AreaMapLayerDB.blockList)
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
}
