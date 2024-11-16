
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGResMgr
    {
        /// <summary>
        /// 
        /// </summary>
        SLGSceneResDB m_ResDB;

        /// <summary>
        /// 
        /// </summary>
        SLGRes m_ShareGridRes = new SLGRes();

        /// <summary>
        /// 
        /// </summary>
        Dictionary<int, SLGRes> m_SceneResDict = new Dictionary<int, SLGRes>();

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, SLGRes> m_CustomResDict = new Dictionary<string, SLGRes>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resDB"></param>
        public void SetResDB(SLGSceneResDB resDB)
        {
            m_ResDB = resDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resID"></param>
        /// <returns></returns>
        public SLGRes FindSceneRes(int resID)
        {
            SLGRes res = null;
            m_SceneResDict.TryGetValue(resID, out res);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public SLGRes FindCustomRes(string resPath)
        {
            SLGRes res = null;
            m_CustomResDict.TryGetValue(resPath, out res);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            InitShareGridRes();
            InitSceneResDict();
            InitCustomResDict();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_ShareGridRes.Destroy();
            DestroySceneResDict();
            DestroyCustomResDict();
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

