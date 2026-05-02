
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ST.SLG
{
    /// <summary>
    /// 场景资源管理：加载共享格子资源、编号场景资源字典与按路径索引的自定义资源（如连线 Mesh），供地图层与信息层使用。
    /// </summary>
    public class SLGResMgr
    {
        SLGSceneResDB m_ResDB;

        SLGRes m_ShareGridRes = new SLGRes();

        Dictionary<int, SLGRes> m_SceneResDict = new Dictionary<int, SLGRes>();

        Dictionary<string, SLGRes> m_CustomResDict = new Dictionary<string, SLGRes>();

        /// <summary>
        /// 绑定 <see cref="SLGSceneResDB"/> 配置。
        /// </summary>
        /// <param name="resDB">场景资源 DB</param>
        public void SetResDB(SLGSceneResDB resDB)
        {
            m_ResDB = resDB;
        }

        /// <summary>
        /// 按资源 ID 查找场景资源（与 <see cref="SLGSceneResDB.resDBList"/> 下标对应）。
        /// </summary>
        /// <param name="resID">资源 ID</param>
        /// <returns>运行时资源包装，未加载返回 null</returns>
        public SLGRes FindSceneRes(int resID)
        {
            SLGRes res = null;
            m_SceneResDict.TryGetValue(resID, out res);
            return res;
        }

        /// <summary>
        /// 按配置中的资源路径查找自定义资源（如 SceneLine 预制）。
        /// 自动将全路径规范化为 Resources 相对路径（去前缀、去扩展名、转小写），兼容新旧 sceneDB。
        /// </summary>
        /// <param name="resPath">资源路径（全路径或 Resources 相对路径均可）</param>
        /// <returns>运行时资源包装，未加载返回 null</returns>
        public SLGRes FindCustomRes(string resPath)
        {
            SLGRes res = null;
            m_CustomResDict.TryGetValue(resPath, out res);
            if (res != null)
                return res;

            var normalizedPath = NormalizeResPath(resPath);
            if (normalizedPath != resPath)
                m_CustomResDict.TryGetValue(normalizedPath, out res);

            return res;
        }

        static string NormalizeResPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            path = path.ToLower();

            const string prefix = "assets/resources/";
            if (path.StartsWith(prefix))
                path = path.Substring(prefix.Length);

            int dotIdx = path.LastIndexOf('.');
            if (dotIdx > 0)
                path = path.Substring(0, dotIdx);

            return path;
        }

        /// <summary>
        /// 加载共享网格与全部场景/自定义资源条目。
        /// </summary>
        public void Init()
        {
            InitShareGridRes();
            InitSceneResDict();
            InitCustomResDict();
        }

        /// <summary>
        /// 释放字典内资源引用并清空。
        /// </summary>
        public void Destroy()
        {
            m_ShareGridRes.Destroy();
            DestroySceneResDict();
            DestroyCustomResDict();
        }

        void DestroyCustomResDict()
        {
            foreach (var iter in m_CustomResDict)
            {
                SLGRes res = iter.Value;
                if (res == null)
                    continue;

                res.Destroy();
            }

            m_CustomResDict.Clear();
        }

        void DestroySceneResDict()
        {
            foreach (var iter in m_SceneResDict)
            {
                SLGRes res = iter.Value;
                if (res == null)
                    continue;

                res.Destroy();
            }

            m_SceneResDict.Clear();
        }

        void InitShareGridRes()
        {
            var shareGridResDB = m_ResDB.shareGridResDB;
            if (shareGridResDB == null)
                return;

            var resPath = shareGridResDB.resPath;
            var resGo = SLGWarp.S.warp.GetResource(resPath) as GameObject;
            if (resGo == null)
                return;

            m_ShareGridRes.SetResDB(shareGridResDB);
            m_ShareGridRes.SetResGo(resGo);
            m_ShareGridRes.InitMesh();
        }

        void InitSceneResDict()
        {
            DestroySceneResDict();

            var resDBList = m_ResDB.resDBList;
            if (resDBList.Count <= 0)
                return;

            for (int i = 0; i < resDBList.Count; i++)
            {
                var resDB = resDBList[i];
                if (resDB == null)
                    continue;

                var resPath = resDB.resPath;

                if (string.IsNullOrEmpty(resPath))
                    continue;

                var resGo = SLGWarp.S.warp.GetResource(resPath) as GameObject;
                if (resGo == null)
                    continue;

                int resID = i;

                SLGRes res = new SLGRes();
                m_SceneResDict.Add(resID, res);

                res.SetResID(resID);
                res.SetResDB(resDB);
                res.SetResGo(resGo);
                res.InitMesh();
                res.SetMesh(m_ShareGridRes.mesh);
            }
        }

        void InitCustomResDict()
        {
            DestroyCustomResDict();

            var resDBList = m_ResDB.customResDBList;
            if (resDBList.Count <= 0)
                return;

            for (int i = 0; i < resDBList.Count; i++)
            {
                var resDB = resDBList[i];
                if (resDB == null)
                    continue;

                var resPath = resDB.resPath;

                if (string.IsNullOrEmpty(resPath))
                    continue;

                if (m_CustomResDict.ContainsKey(resPath))
                    continue;

                var resGo = SLGWarp.S.warp.GetResource(resPath) as GameObject;
                if (resGo == null)
                    continue;

                SLGRes res = new SLGRes();
                m_CustomResDict.Add(resPath, res);

                res.SetResDB(resDB);
                res.SetResGo(resGo);
                res.InitMesh();
                res.SetMesh(m_ShareGridRes.mesh);
            }
        }
    }
}
