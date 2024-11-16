using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGSceneResDB
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGResDB shareGridResDB = new SLGResDB();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGResDB> resDBList = new List<SLGResDB>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<SLGResDB> customResDBList = new List<SLGResDB>();

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public string realShareGridResPath = "";

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public List<string> realResPathList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public List<string> realCustomResPathList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPath"></param>
        public void InitShareGridRes(string resPath)
        {
            var newResPath = resPath.ToLower();
            shareGridResDB.resPath = newResPath;

            realShareGridResPath = resPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="res"></param>
        public void AddRes(string resPath, string matPath, int renderQueue, bool isZWriteOn)
        {
            var newResPath = resPath.ToLower();

            var isExist = IsExistRes(matPath);
            if (isExist)
                return;

            SLGResDB resDB = new SLGResDB();
            resDB.resPath = newResPath;
            resDB.renderQueue = renderQueue;
            resDB.zWriteOn = isZWriteOn;
            resDB.matPath = matPath;

            resDBList.Add(resDB);
            realResPathList.Add(resPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPath"></param>
        public void AddCustomRes(string resPath, int renderQueue, bool isZWriteOn)
        {
            var newResPath = resPath.ToLower();

            var isExist = IsExistCustomRes(newResPath);
            if (isExist)
                return;

            SLGResDB resDB = new SLGResDB();
            resDB.resPath = newResPath;
            resDB.renderQueue = renderQueue;
            resDB.zWriteOn = isZWriteOn;

            customResDBList.Add(resDB);

            realCustomResPathList.Add(resPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resID"></param>
        /// <returns></returns>
        public SLGResDB FindResByResID(int resID)
        {
            for (int i = 0; i < resDBList.Count; i++)
            {
                SLGResDB resDB = resDBList[i];
                if (resDB == null)
                    continue;

                if (i == resID)
                {
                    return resDB;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public int FindResId(string _matPath)
        {
            for (int i = 0; i < resDBList.Count; i++)
            {
                SLGResDB resDB = resDBList[i];
                if (resDB == null)
                    continue;

                var matPath = resDB.matPath;
                if (matPath == _matPath)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_resPath"></param>
        /// <returns></returns>
        bool IsExistCustomRes(string _resPath)
        {
            foreach (var res in customResDBList)
            {
                if (res == null)
                    continue;

                if (res.resPath == _resPath)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        bool IsExistRes(string _matPath)
        {
            foreach (var res in resDBList)
            {
                if (res == null)
                    continue;

                if (res.matPath == _matPath)
                    return true;
            }

            return false;
        }
    }
}

