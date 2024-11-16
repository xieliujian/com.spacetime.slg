using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaInfoLayer : SLGAreaLayer
    {
        /// <summary>
        /// 
        /// </summary>
        protected SLGAreaInfoLayerDB m_InfoLayerDB;

        /// <summary>
        /// 
        /// </summary>
        protected Mesh m_Mesh;

        /// <summary>
        /// 
        /// </summary>
        protected Material m_Mat;

        /// <summary>
        /// 
        /// </summary>
        protected Matrix4x4 m_InitScaleMatrix = Matrix4x4.identity;

        /// <summary>
        /// 
        /// </summary>
        protected MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        /// <summary>
        /// 
        /// </summary>
        protected List<Matrix4x4> m_MatrixList = new List<Matrix4x4>();

        /// <summary>
        /// 
        /// </summary>
        protected List<Vector4> m_ColorList = new List<Vector4>();

        /// <summary>
        /// 
        /// </summary>
        protected bool m_Dirty = false;

        /// <summary>
        /// 
        /// </summary>
        protected bool m_Render = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoLayerDB"></param>
        public void SetAreaInfoLayerDB(SLGAreaInfoLayerDB infoLayerDB)
        {
            m_InfoLayerDB = infoLayerDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="render"></param>
        public void SetRender(bool render)
        {
            m_Render = render;
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
        /// <param name="meshScale"></param>
        public void CalcInitScaleMatrix(Vector3 meshScale)
        {
            Vector3 scale = Vector3.one;
            scale.x = meshScale.x / SLGDefine.SLG_GRID_UNIT_SIZE;
            scale.z = meshScale.z / SLGDefine.SLG_GRID_UNIT_SIZE;

            m_InitScaleMatrix = Matrix4x4.Scale(scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetLayerIndex()
        {
            if (m_InfoLayerDB == null)
                return -1;

            return m_InfoLayerDB.layerID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public virtual void AddGridInfo(Vector2Int logicPos, Color color)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        public virtual void RemoveGridInfo(Vector2Int logicPos)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="tex"></param>
        public virtual void FillMiniMapTexture(int layerID, Texture2D tex)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Destroy()
        {
            m_MatPropBlock.Clear();
            m_MatrixList.Clear();
            m_ColorList.Clear();
            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SubmitGPU()
        {

        }
    }
}

