using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单套动态地表层的配置：层 ID、地表索引及地图块列表。
    /// </summary>
    [Serializable]
    public class SLGAreaDynamicMapLayerDB
    {
        /// <summary>
        /// 动态地图层 ID。
        /// </summary>
        [SerializeField]
        public int layerID;

        /// <summary>
        /// 与场景当前动态地表索引对应的套序号。
        /// </summary>
        [SerializeField]
        public int index;

        /// <summary>
        /// 该套地表下的静态放置块数据。
        /// </summary>
        [SerializeField]
        public List<SLGAreaMapBlockDB> blockList = new List<SLGAreaMapBlockDB>();
    }

    /// <summary>
    /// 区域内多套动态地表的集合与填充入口。
    /// </summary>
    [Serializable]
    public class SLGAreaDynamicMapLayerSetDB
    {
        /// <summary>
        /// 各套动态地表 DB。
        /// </summary>
        [SerializeField]
        public List<SLGAreaDynamicMapLayerDB> dynamicMapList = new List<SLGAreaDynamicMapLayerDB>();

        /// <summary>
        /// 向指定套、指定层追加或更新一块渲染数据（委托 <see cref="SLGUtils.FillRenderBlockDB"/>）。
        /// </summary>
        /// <param name="index">动态地表套索引</param>
        /// <param name="layerID">层 ID</param>
        /// <param name="resDB">场景资源表</param>
        /// <param name="resId">资源 ID</param>
        /// <param name="pos">世界位置</param>
        /// <param name="rot">欧拉角</param>
        /// <param name="scale">缩放</param>
        /// <param name="uvScaleOffset">UV 缩放偏移</param>
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
        /// 按套索引获取或创建 <see cref="SLGAreaDynamicMapLayerDB"/>。
        /// </summary>
        /// <param name="index">套索引</param>
        /// <returns>动态地表 DB</returns>
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
