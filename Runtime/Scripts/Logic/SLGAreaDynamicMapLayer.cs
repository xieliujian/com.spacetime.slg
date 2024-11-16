
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaDynamicMapLayer : SLGAreaLayer
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaDynamicMapLayerDB m_AreaDynamicMapDB;

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
        public void SetAreaDynamicMapDB(SLGAreaDynamicMapLayerDB areaDynamicMapDB)
        {
            m_AreaDynamicMapDB = areaDynamicMapDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetLayerIndex()
        {
            if (m_AreaDynamicMapDB == null)
                return -1;

            return m_AreaDynamicMapDB.layerID;
        }

        /// <summary>
        /// 
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

    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaDynamicMapLayerSet
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaDynamicMapLayerSetDB m_DynamicMapSetDB;

        /// <summary>
        /// 
        /// </summary>
        SLGScene m_Scene;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<int, SLGAreaDynamicMapLayer> m_DynamicMapDict = new Dictionary<int, SLGAreaDynamicMapLayer>();

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
        public void SetAreaDynamicMapSetDB(SLGAreaDynamicMapLayerSetDB areaDynamicMapSetDB)
        {
            m_DynamicMapSetDB = areaDynamicMapSetDB;
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
        /// <param name="dynamicMapIndex"></param>
        /// <returns></returns>
        public SLGAreaDynamicMapLayer FindAreaDynamicMap(int dynamicMapIndex)
        {
            SLGAreaDynamicMapLayer dynamicMap = null;
            m_DynamicMapDict.TryGetValue(dynamicMapIndex, out dynamicMap);
            return dynamicMap;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void Render()
        {
            SLGAreaDynamicMapLayer dynamicMap = FindAreaDynamicMap(m_Scene.curDynamicMapIndex);
            if (dynamicMap == null)
                return;

            dynamicMap.Render();
        }

        /// <summary>
        /// 
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

