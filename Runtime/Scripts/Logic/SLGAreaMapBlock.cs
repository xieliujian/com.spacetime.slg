using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 区域地图渲染块，负责单个地图块的 GPU 实例化渲染。
    /// </summary>
    public class SLGAreaMapBlock
    {
        SLGAreaMapBlockDB m_RenderBlockDB;

        Mesh m_Mesh;

        Material m_Mat;

        protected MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        /// <summary>
        /// 设置渲染块的数据库配置。
        /// </summary>
        /// <param name="blockDB">地图块数据库对象。</param>
        public void SetRenderBlockDB(SLGAreaMapBlockDB blockDB)
        {
            m_RenderBlockDB = blockDB;
        }

        /// <summary>
        /// 设置渲染使用的网格。
        /// </summary>
        /// <param name="mesh">目标网格。</param>
        public void SetMesh(Mesh mesh)
        {
            m_Mesh = mesh;
        }

        /// <summary>
        /// 设置渲染使用的材质。
        /// </summary>
        /// <param name="mat">目标材质。</param>
        public void SetMat(Material mat)
        {
            m_Mat = mat;
        }

        /// <summary>
        /// 初始化渲染块，将 UV 缩放偏移数据写入材质属性块。
        /// </summary>
        public void Init()
        {
            if (m_RenderBlockDB != null && m_RenderBlockDB.uvScaleOffsetList != null)
            {
                m_MatPropBlock.SetVectorArray(SLGDefine.s_SLG_Shader_SceneObj_UvScaleOffsetId, m_RenderBlockDB.uvScaleOffsetList);
            }
        }

        /// <summary>
        /// 销毁渲染块，释放材质属性块及引用资源。
        /// </summary>
        public void Destroy()
        {
            m_MatPropBlock.Clear();
            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 执行当前帧的 GPU 实例化渲染。
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
