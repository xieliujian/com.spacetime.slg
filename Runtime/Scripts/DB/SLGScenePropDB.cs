using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGScenePropDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGPropertyGridDB> propGridList = new List<SLGPropertyGridDB>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propPos"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
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

