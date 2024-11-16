using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGSceneProperty
    {
        /// <summary>
        /// 
        /// </summary>
        SLGScenePropDB m_ScenePropDB;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<Vector2Int, SLGPropertyGridDB> m_PropGridDict = new Dictionary<Vector2Int, SLGPropertyGridDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenePropDB"></param>
        public void SetScenePropDB(SLGScenePropDB scenePropDB)
        {
            m_ScenePropDB = scenePropDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public SLGPropertyGridDB FindGridProperty(Vector2Int gridPos)
        {
            SLGPropertyGridDB gridDB = null;
            m_PropGridDict.TryGetValue(gridPos, out gridDB);
            return gridDB;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            InitPropGridDict();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_PropGridDict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        void InitPropGridDict()
        {
            m_PropGridDict.Clear();

            if (m_ScenePropDB == null)
                return;

            foreach (var propGrid in m_ScenePropDB.propGridList)
            {
                if (propGrid == null)
                    continue;

                Vector2Int propPos = propGrid.pos;
                if (m_PropGridDict.ContainsKey(propPos))
                {
                    //Debugger.LogDebugF("[SLGSceneProperty][InitPropGridDict] {0} Êý¾ÝÖØ¸´", propPos);
                    continue;
                }

                m_PropGridDict.Add(propPos, propGrid);
            }
        }
    }
}

