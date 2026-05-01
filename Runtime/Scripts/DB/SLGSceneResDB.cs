using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 场景资源数据库，管理共享格子资源、普通渲染资源和自定义资源的集合。
    /// </summary>
    [System.Serializable]
    public class SLGSceneResDB
    {
        /// <summary>
        /// 共享格子资源数据库，所有格子共用的基础资源。
        /// </summary>
        [SerializeField]
        public SLGResDB shareGridResDB = new SLGResDB();

        /// <summary>
        /// 普通渲染资源数据库列表，按材质路径索引。
        /// </summary>
        [SerializeField]
        public List<SLGResDB> resDBList = new List<SLGResDB>();

        /// <summary>
        /// 自定义资源数据库列表，用于信息层等特殊资源。
        /// </summary>
        [SerializeField]
        public List<SLGResDB> customResDBList = new List<SLGResDB>();

        /// <summary>
        /// 共享格子资源的真实路径（运行时，不序列化）。
        /// </summary>
        [NonSerialized]
        public string realShareGridResPath = "";

        /// <summary>
        /// 普通资源的真实路径列表（运行时，不序列化）。
        /// </summary>
        [NonSerialized]
        public List<string> realResPathList = new List<string>();

        /// <summary>
        /// 自定义资源的真实路径列表（运行时，不序列化）。
        /// </summary>
        [NonSerialized]
        public List<string> realCustomResPathList = new List<string>();

        /// <summary>
        /// 初始化共享格子资源路径。
        /// </summary>
        /// <param name="resPath">资源路径</param>
        public void InitShareGridRes(string resPath)
        {
            var newResPath = ConvertToResourcesPath(resPath);
            shareGridResDB.resPath = newResPath;

            realShareGridResPath = resPath;
        }

        /// <summary>
        /// 添加普通渲染资源，若材质路径已存在则跳过。
        /// </summary>
        /// <param name="resPath">预制体资源路径</param>
        /// <param name="matPath">材质路径（用于去重）</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        public void AddRes(string resPath, string matPath, int renderQueue, bool isZWriteOn)
        {
            var newResPath = ConvertToResourcesPath(resPath);

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
        /// 添加自定义资源，若资源路径已存在则跳过。
        /// </summary>
        /// <param name="resPath">资源路径</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        public void AddCustomRes(string resPath, int renderQueue, bool isZWriteOn)
        {
            var newResPath = ConvertToResourcesPath(resPath);

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
        /// 根据资源ID查找对应的资源数据库。
        /// </summary>
        /// <param name="resID">资源ID（即列表索引）</param>
        /// <returns>找到则返回对应资源数据库，否则返回null</returns>
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
        /// 根据材质路径查找对应的资源ID。
        /// </summary>
        /// <param name="_matPath">材质路径</param>
        /// <returns>找到则返回资源ID（列表索引），否则返回-1</returns>
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

        /// <summary>
        /// 将 Assets 完整路径转换为 Resources 相对路径（用于 Resources.Load）。
        /// 例如：Assets/Resources/scene/common/res/slg/prefab/xxx.prefab -> scene/common/res/slg/prefab/xxx
        /// </summary>
        /// <param name="assetPath">Assets 完整路径</param>
        /// <returns>Resources 相对路径（小写，无扩展名）</returns>
        string ConvertToResourcesPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return "";

            string path = assetPath.ToLower();

            // 去掉 Assets/Resources/ 前缀（不区分大小写）
            const string resourcesPrefix = "assets/resources/";
            if (path.StartsWith(resourcesPrefix))
            {
                path = path.Substring(resourcesPrefix.Length);
            }

            // 去掉文件扩展名（Resources.Load 不需要扩展名）
            int lastDotIndex = path.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                path = path.Substring(0, lastDotIndex);
            }

            return path;
        }
    }
}

