using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 场景属性数据库，管理所有格子的属性信息列表。
    /// </summary>
    [System.Serializable]
    public class SLGScenePropDB
    {
        /// <summary>
        /// 属性格子数据库列表，存储所有已设置属性的格子。
        /// </summary>
        [SerializeField]
        public List<SLGPropertyGridDB> propGridList = new List<SLGPropertyGridDB>();

        /// <summary>
        /// 获取或创建指定位置的属性格子数据库，若不存在则新建并初始化默认属性。
        /// </summary>
        /// <param name="propPos">格子的逻辑坐标</param>
        /// <returns>对应位置的属性格子数据库</returns>
        public SLGPropertyGridDB GetOrCreatePropertyGridDB(Vector2Int propPos)
        {
            var findPropertyDB = FindProperty(propPos);
            if (findPropertyDB == null)
            {
                findPropertyDB = new SLGPropertyGridDB();
                findPropertyDB.pos = propPos;
                findPropertyDB.index = SLGUtils.CalcPropertyGridIndex(propPos);
                findPropertyDB.ResetProperty();
                propGridList.Add(findPropertyDB);
            }

            return findPropertyDB;
        }

        /// <summary>
        /// 查找指定位置的属性格子数据库。
        /// </summary>
        /// <param name="pos">格子的逻辑坐标</param>
        /// <returns>找到则返回对应数据库，否则返回null</returns>
        public SLGPropertyGridDB FindProperty(Vector2Int pos)
        {
            SLGPropertyGridDB findGridDB = null;

            foreach(var gridDB in propGridList)
            {
                if (gridDB == null)
                    continue;

                if (gridDB.pos == pos)
                {
                    findGridDB = gridDB;
                    break;
                }
            }

            return findGridDB;
        }
    }
}

