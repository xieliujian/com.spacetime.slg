
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    [System.Serializable]
    public class SLGAreaMapLayerSetDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapLayerDB> layerList = new List<SLGAreaMapLayerDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="obj"></param>
        /// <param name="resDB"></param>
        /// <param name="resId"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        public void FillRenderBlockDB(int layerID, SLGSceneResDB resDB, int resId, 
            Vector3 pos, Vector3 rot, Vector3 scale, Vector4 uvScaleOffset)
        {
            var layerDB = GetOrCreateMapLayerDB(layerID);
            if (layerDB == null)
                return;

            var blockList = layerDB.blockList;
            SLGUtils.FillRenderBlockDB(blockList, resDB, resId, pos, rot, scale, uvScaleOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerType"></param>
        /// <returns></returns>
        public SLGAreaMapLayerDB GetOrCreateMapLayerDB(int layerID)
        {
            SLGAreaMapLayerDB findDB = null;

            foreach (var db in layerList)
            {
                if (db == null)
                    continue;

                if (layerID == db.layerID)
                {
                    findDB = db;
                    break;
                }
            }

            if (findDB == null)
            {
                findDB = new SLGAreaMapLayerDB();
                findDB.layerID = layerID;
                layerList.Add(findDB);
            }

            return findDB;
        }
    }
}

