using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 区域信息图层基类，提供网格信息叠加渲染的通用接口与数据结构。
    /// </summary>
    public class SLGAreaInfoLayer : SLGAreaLayer
    {
        protected SLGAreaInfoLayerDB m_InfoLayerDB;

        protected Mesh m_Mesh;

        protected Material m_Mat;

        protected Matrix4x4 m_InitScaleMatrix = Matrix4x4.identity;

        protected MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        protected List<Matrix4x4> m_MatrixList = new List<Matrix4x4>();

        protected List<Vector4> m_ColorList = new List<Vector4>();

        protected bool m_Dirty = false;

        protected bool m_Render = true;

        /// <summary>
        /// 设置信息图层的数据库配置。
        /// </summary>
        /// <param name="infoLayerDB">信息图层数据库对象。</param>
        public void SetAreaInfoLayerDB(SLGAreaInfoLayerDB infoLayerDB)
        {
            m_InfoLayerDB = infoLayerDB;
        }

        /// <summary>
        /// 设置图层是否参与渲染。
        /// </summary>
        /// <param name="render">true 表示渲染，false 表示隐藏。</param>
        public void SetRender(bool render)
        {
            m_Render = render;
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
        /// 根据网格尺寸和旋转计算初始变换矩阵，使网格与格子单位尺寸对齐并保留 prefab 自带旋转。
        /// </summary>
        /// <param name="meshScale">网格的原始尺寸。</param>
        /// <param name="meshRotation">网格的本地旋转（prefab 自带朝向）。</param>
        public void CalcInitScaleMatrix(Vector3 meshScale, Quaternion meshRotation)
        {
            Vector3 scale = Vector3.one;
            scale.x = meshScale.x / SLGDefine.s_SLG_Grid_UnitSize;
            scale.z = meshScale.z / SLGDefine.s_SLG_Grid_UnitSize;

            m_InitScaleMatrix = Matrix4x4.TRS(Vector3.zero, meshRotation, scale);
        }

        /// <summary>
        /// 根据网格尺寸计算初始缩放矩阵（无旋转版本，向后兼容）。
        /// </summary>
        /// <param name="meshScale">网格的原始尺寸。</param>
        public void CalcInitScaleMatrix(Vector3 meshScale)
        {
            CalcInitScaleMatrix(meshScale, Quaternion.identity);
        }

        /// <summary>
        /// 获取当前信息图层的索引 ID。
        /// </summary>
        /// <returns>图层 ID，若未设置则返回 -1。</returns>
        public override int GetLayerIndex()
        {
            if (m_InfoLayerDB == null)
                return -1;

            return m_InfoLayerDB.layerID;
        }

        /// <summary>
        /// 在指定逻辑坐标处添加网格信息（颜色标记）。
        /// </summary>
        /// <param name="logicPos">逻辑格子坐标。</param>
        /// <param name="color">标记颜色。</param>
        public virtual void AddGridInfo(Vector2Int logicPos, Color color)
        {

        }

        /// <summary>
        /// 移除指定逻辑坐标处的网格信息。
        /// </summary>
        /// <param name="logicPos">逻辑格子坐标。</param>
        public virtual void RemoveGridInfo(Vector2Int logicPos)
        {

        }

        /// <summary>
        /// 将当前图层的颜色数据填充到小地图纹理中。
        /// </summary>
        /// <param name="layerID">图层 ID。</param>
        /// <param name="tex">目标小地图纹理。</param>
        public virtual void FillMiniMapTexture(int layerID, Texture2D tex)
        {

        }

        /// <summary>
        /// 初始化图层数据。
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 销毁图层，清理材质属性块及所有列表数据。
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
        /// 将脏数据提交到 GPU 材质属性块。
        /// </summary>
        public virtual void SubmitGPU()
        {

        }
    }
}

