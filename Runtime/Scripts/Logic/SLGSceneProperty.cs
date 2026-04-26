using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 按逻辑格索引查询属性数据：自 <see cref="SLGScenePropDB"/> 构建 <see cref="SLGPropertyGridDB"/> 字典，供 <see cref="SLGScene"/> 使用。
    /// </summary>
    public class SLGSceneProperty
    {
        SLGScenePropDB m_ScenePropDB;

        Dictionary<Vector2Int, SLGPropertyGridDB> m_PropGridDict = new Dictionary<Vector2Int, SLGPropertyGridDB>();

        /// <summary>
        /// 设置场景级属性数据库引用。
        /// </summary>
        /// <param name="scenePropDB">场景属性 DB</param>
        public void SetScenePropDB(SLGScenePropDB scenePropDB)
        {
            m_ScenePropDB = scenePropDB;
        }

        /// <summary>
        /// 按逻辑格坐标查询属性格块数据。
        /// </summary>
        /// <param name="gridPos">逻辑格子坐标</param>
        /// <returns>存在则返回对应 DB，否则为 null</returns>
        public SLGPropertyGridDB FindGridProperty(Vector2Int gridPos)
        {
            SLGPropertyGridDB gridDB = null;
            m_PropGridDict.TryGetValue(gridPos, out gridDB);
            return gridDB;
        }

        /// <summary>
        /// 从已绑定的场景 DB 重建格字典（开局或重载数据时调用）。
        /// </summary>
        public void Init()
        {
            InitPropGridDict();
        }

        /// <summary>
        /// 清空格字典，释放场景引用。
        /// </summary>
        public void Destroy()
        {
            m_PropGridDict.Clear();
        }

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
                    //Debugger.LogDebugF("[SLGSceneProperty][InitPropGridDict] {0} 属性格重复", propPos);
                    continue;
                }

                m_PropGridDict.Add(propPos, propGrid);
            }
        }
    }
}
