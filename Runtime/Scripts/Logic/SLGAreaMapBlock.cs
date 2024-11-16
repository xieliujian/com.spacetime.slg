using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaMapBlock
    {
        /// <summary>
        /// 
        /// </summary>
        SLGAreaMapBlockDB m_RenderBlockDB;

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
        protected MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockDB"></param>
        public void SetRenderBlockDB(SLGAreaMapBlockDB blockDB)
        {
            m_RenderBlockDB = blockDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void SetMesh(Mesh mesh)
        {
            m_Mesh = mesh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public void SetMat(Material mat)
        {
            m_Mat = mat;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if (m_RenderBlockDB != null && m_RenderBlockDB.uvScaleOffsetList != null)
            {
                m_MatPropBlock.SetVectorArray(SLGDefine.SLG_SHADER_SCENEOBJ_UV_SCALE_OFFSET_ID, m_RenderBlockDB.uvScaleOffsetList);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Destroy()
        {
            m_MatPropBlock.Clear();
            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            if (m_RenderBlockDB == null || m_Mat == null || m_Mesh == null)
                return;

            int matrixNum = m_RenderBlockDB.matrixList.Count;
            if (matrixNum <= 0)
                return;

            Graphics.DrawMeshInstanced(m_Mesh, 0, m_Mat, m_RenderBlockDB.matrixList,
                    m_MatPropBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        }
    }
}

