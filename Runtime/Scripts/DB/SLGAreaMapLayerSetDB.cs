
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域地图层集合数据库，管理静态地图的所有渲染层。
    /// </summary>
    [System.Serializable]
    public class SLGAreaMapLayerSetDB
    {
        /// <summary>
        /// 地图层数据库列表。
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapLayerDB> layerList = new List<SLGAreaMapLayerDB>();

        /// <summary>
        /// 向指定层填充渲染块数据。
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <param name="resDB">场景资源数据库</param>
        /// <param name="resId">资源ID</param>
        /// <param name="pos">世界坐标位置</param>
        /// <param name="rot">旋转角度</param>
        /// <param name="scale">缩放比例</param>
        /// <param name="uvScaleOffset">UV缩放偏移</param>
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
        /// 获取或创建指定ID的地图层数据库。
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <returns>对应地图层数据库</returns>
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

