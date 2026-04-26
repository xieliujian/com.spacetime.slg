using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单个 SLG 资源的运行时包装：从预制体上采集 Mesh/材质/缩放，供批渲染与 GPU Instancing 使用。
    /// </summary>
    public class SLGRes
    {
        SLGResDB m_ResDB;

        int m_ResID;

        GameObject m_ResGo;

        Mesh m_Mesh;

        Material m_Mat;

        Vector3 m_MeshScale;

        /// <summary>
        /// 共享的网格数据（可对应共享格或合批用 Mesh）。
        /// </summary>
        public Mesh mesh
        {
            get { return m_Mesh; }
        }

        /// <summary>
        /// 与资源 DB 中配置一致的材质实例引用。
        /// </summary>
        public Material mat
        {
            get { return m_Mat; }
        }

        /// <summary>
        /// MeshRenderer 所在子物体的本地缩放，用于与 UV/矩阵一致。
        /// </summary>
        public Vector3 meshScale
        {
            get { return m_MeshScale; }
        }

        /// <summary>
        /// 释放对 GameObject 等引用，不直接销毁外部资源。
        /// </summary>
        public void Destroy()
        {
            m_ResGo = null;
        }

        /// <summary>
        /// 设置本资源在 DB 中的资源 ID。
        /// </summary>
        /// <param name="resID">资源 ID</param>
        public void SetResID(int resID)
        {
            m_ResID = resID;
        }

        /// <summary>
        /// 绑定源资源数据库，用于取渲染队列、深度等。
        /// </summary>
        /// <param name="resDB">资源 DB</param>
        public void SetResDB(SLGResDB resDB)
        {
            m_ResDB = resDB;
        }

        /// <summary>
        /// 设置从预制体实例化出的根 GameObject，供 <see cref="InitMesh"/> 采集 Mesh/Material。
        /// </summary>
        /// <param name="resGo">实例化的 GameObject</param>
        public void SetResGo(GameObject resGo)
        {
            m_ResGo = resGo;
        }

        /// <summary>
        /// 直接指定共享格等 Mesh，用于与 Instancing 共用的合批网格。
        /// </summary>
        /// <param name="shareGridMesh">共享格 Mesh</param>
        public void SetMesh(Mesh shareGridMesh)
        {
            m_Mesh = shareGridMesh;
        }

        /// <summary>
        /// 从已设置的 GameObject 上提取 Mesh/材质/缩放，并依 DB 启用了 GPU Instancing 与 Z 相关 Shader 属性。
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

            // 批绘需要时开启 GPU Instancing
            if (!m_Mat.enableInstancing)
            {
                m_Mat.enableInstancing = true;
            }

            m_Mat.SetFloat(SLGDefine.s_SLG_Shader_SceneObj_ZTestId, (float)UnityEngine.Rendering.CompareFunction.Always);
            m_Mat.SetFloat(SLGDefine.s_SLG_Shader_SceneObj_ZWriteId, m_ResDB.zWriteOn ? 1 : 0);

            m_Mat.renderQueue = m_ResDB.renderQueue;
        }
    }
}
