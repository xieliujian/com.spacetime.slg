using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SLGAreaDynamicMapLayerDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public int index;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapBlockDB> blockList = new List<SLGAreaMapBlockDB>();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SLGAreaDynamicMapLayerSetDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaDynamicMapLayerDB> dynamicMapList = new List<SLGAreaDynamicMapLayerDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="resDB"></param>
        /// <param name="resId"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        public void FillRenderBlockDB(int index, int layerID, SLGSceneResDB resDB, int resId, 
            Vector3 pos, Vector3 rot, Vector3 scale, Vector4 uvScaleOffset)
        {
            var dynamicMapDB = GetOrCreateDynamicMapDB(index);
            if (dynamicMapDB == null)
                return;

            dynamicMapDB.layerID = layerID;

            var blockList = dynamicMapDB.blockList;
            SLGUtils.FillRenderBlockDB(blockList, resDB, resId, pos, rot, scale, uvScaleOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SLGAreaDynamicMapLayerDB GetOrCreateDynamicMapDB(int index)
        {
            SLGAreaDynamicMapLayerDB findDB = null;

            foreach (var db in dynamicMapList)
            {
                if (db == null)
                    continue;

                if (index == db.index)
                {
                    findDB = db;
                    break;
                }
            }

            if (findDB == null)
            {
                findDB = new SLGAreaDynamicMapLayerDB();
                findDB.index = index;
                dynamicMapList.Add(findDB);
            }

            return findDB;
        }
    }
}

