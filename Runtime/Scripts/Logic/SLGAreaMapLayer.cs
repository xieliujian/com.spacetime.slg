using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaMapLayer : SLGAreaLayer
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaMapLayerDB m_AreaMapLayerDB;

        /// <summary>
        /// 
        /// </summary>
        List<SLGAreaMapBlock> m_BlockList = new List<SLGAreaMapBlock>();

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
        public void SetAreaMapLayerDB(SLGAreaMapLayerDB areaMapLayerDB)
        {
            m_AreaMapLayerDB = areaMapLayerDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetLayerIndex()
        {
            if (m_AreaMapLayerDB == null)
                return -1;

            return m_AreaMapLayerDB.layerID;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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

