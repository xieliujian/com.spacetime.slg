using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 区域格子集合数据库，管理单个区域内所有逻辑格子的起止范围和格子列表。
    /// </summary>
    [Serializable]
    public class SLGAreaGridSetDB
    {
        /// <summary>
        /// 格子集合的起始逻辑坐标（左下角）。
        /// </summary>
        [SerializeField]
        public Vector2Int startPos;

        /// <summary>
        /// 格子集合的结束逻辑坐标（右上角，不含）。
        /// </summary>
        [SerializeField]
        public Vector2Int endPos;

        /// <summary>
        /// 区域内所有格子的数据库列表。
        /// </summary>
        [SerializeField]
        public List<SLGAreaGridDB> gridList = new List<SLGAreaGridDB>();

        /// <summary>
        /// 根据格子索引查找对应的格子数据库。
        /// </summary>
        /// <param name="gridIndex">格子在列表中的索引</param>
        /// <returns>找到则返回格子数据库，索引越界则返回null</returns>
        public SLGAreaGridDB FindAreaGridDB(int gridIndex)
        {
            if (gridIndex < 0 || gridIndex >= gridList.Count)
                return null;

            return gridList[gridIndex];
        }

        /// <summary>
        /// 根据逻辑坐标查找对应的格子数据库（编辑器使用）。
        /// </summary>
        /// <param name="logicPos">格子的逻辑坐标</param>
        /// <returns>找到则返回格子数据库，否则返回null</returns>
        public SLGAreaGridDB FindAreaGridDB(Vector2Int logicPos)
        {
            foreach(var grid in gridList)
            {
                if (grid == null)
                    continue;

                if (grid.pos == logicPos)
                {
                    return grid;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据区域的3D逻辑边界计算格子集合的起止逻辑坐标。
        /// </summary>
        /// <param name="logicBoundMin">区域逻辑边界最小点</param>
        /// <param name="logicBoundMax">区域逻辑边界最大点</param>
        public void CalcStartEndPos(Vector3 logicBoundMin, Vector3 logicBoundMax)
        {
            startPos = SLGUtils.ConvertSLG3DPosToLogicPos(logicBoundMin);
            endPos = SLGUtils.ConvertSLG3DPosToLogicPos(logicBoundMax);
        }

        /// <summary>
        /// 判断给定逻辑坐标是否在该格子集合范围内。
        /// </summary>
        /// <param name="pos">格子的逻辑坐标</param>
        /// <returns>在范围内返回true，否则返回false</returns>
        public bool IsInArea(Vector2Int pos)
        {
            if (pos.x >= startPos.x && pos.y >= startPos.y &&
                pos.x < endPos.x && pos.y < endPos.y)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 初始化格子集合，在起止范围内创建所有格子并计算其变换矩阵。
        /// </summary>
        public void Init()
        {
            for (int y = startPos.y; y < endPos.y; y++)
            {
                for (int x = startPos.x; x < endPos.x; x++)
                {
                    SLGAreaGridDB grid = new SLGAreaGridDB();
                    grid.pos = new Vector2Int(x, y);
                    grid.matrix = SLGUtils.CalcSLGGridMatrix(grid.pos);

                    gridList.Add(grid);
                }
            }
        }
    }

}
