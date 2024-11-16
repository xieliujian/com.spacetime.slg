using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SLGAreaGridSetDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public Vector2Int startPos;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public Vector2Int endPos;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGAreaGridDB> gridList = new List<SLGAreaGridDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public SLGAreaGridDB FindAreaGridDB(int gridIndex)
        {
            if (gridIndex < 0 || gridIndex >= gridList.Count)
                return null;

            return gridList[gridIndex];
        }

        /// <summary>
        /// ±à¼­Æ÷Ê¹ÓÃ
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="logicBoundMin"></param>
        /// <param name="logicBoundMax"></param>
        public void CalcStartEndPos(Vector3 logicBoundMin, Vector3 logicBoundMax)
        {
            startPos = SLGUtils.ConvertSLG3DPosToLogicPos(logicBoundMin);
            endPos = SLGUtils.ConvertSLG3DPosToLogicPos(logicBoundMax);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
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
        /// 
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
