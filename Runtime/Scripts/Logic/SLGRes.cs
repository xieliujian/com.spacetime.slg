using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGRes
    {
        /// <summary>
        /// 
        /// </summary>
        SLGResDB m_ResDB;

        /// <summary>
        /// 
        /// </summary>
        int m_ResID;

        /// <summary>
        /// 
        /// </summary>
        GameObject m_ResGo;

        /// <summary>
        /// 
        /// </summary>
        Mesh m_Mesh;

        /// <summary>
        /// 
        /// </summary>
        Material m_Mat;

        /// <summary>
        /// 
        /// </summary>
        Vector3 m_MeshScale;

        /// <summary>
        /// 
        /// </summary>
        public Mesh mesh
        {
            get { return m_Mesh; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material mat
        {
            get { return m_Mat; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 meshScale
        {
            get { return m_MeshScale; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_ResGo = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resID"></param>
        public void SetResID(int resID)
        {
            m_ResID = resID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resDB"></param>
        public void SetResDB(SLGResDB resDB)
        {
            m_ResDB = resDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        public void SetResGo(GameObject resGo)
        {
            m_ResGo = resGo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareGridMesh"></param>
        public void SetMesh(Mesh shareGridMesh)
        {
            m_Mesh = shareGridMesh;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitMesh()
        {
            if (m_ResGo == null || m_ResDB == null)
                return;

            var render = m_ResGo.GetComponentInChildren<MeshRenderer>();
            if (render == null)
                return;

            var meshFilter = render.GetComponent<MeshFilter>();
            if (meshFilter == null)
                return;

            m_Mesh = meshFilter.sharedMesh;
            m_Mat = render.sharedMaterial;
            m_MeshScale = render.transform.localScale;

            // …Ë÷√≤ƒ÷ Instance
            if (!m_Mat.enableInstancing)
            {
                m_Mat.enableInstancing = true;
            }

            m_Mat.SetFloat(SLGDefine.SLG_SHADER_SCENEOBJ_ZTEST_ID, (float)UnityEngine.Rendering.CompareFunction.Always);
            m_Mat.SetFloat(SLGDefine.SLG_SHADER_SCENEOBJ_ZWRITE_ID, m_ResDB.zWriteOn ? 1 : 0);

            m_Mat.renderQueue = m_ResDB.renderQueue;
        }
    }
}

