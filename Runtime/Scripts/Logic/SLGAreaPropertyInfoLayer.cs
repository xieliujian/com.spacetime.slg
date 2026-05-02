using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域属性信息图层，从属性数据库中解析当前区域的数据块，生成 UV 与实例矩阵并参与 GPU 实例化渲染（如资源等级状态层）。
    /// </summary>
    public class SLGAreaPropertyInfoLayer : SLGAreaInfoLayer
    {
        int m_AreaIndex;

        SLGAreaPropertyInfoLayerDB m_PropertyInfoLayerDB;

        SLGAreaPropertyInfoBlockDB m_PropertyInfoBlockDB;

        /// <summary>
        /// 设置当前区域在区域集内的索引，用于从属性层数据库的块列表中选取本区域数据。
        /// </summary>
        /// <param name="index">区域索引（0 起）。</param>
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 设置区域属性信息图层的数据库配置。
        /// </summary>
        /// <param name="propertyLayerDB">区域属性信息图层数据库对象。</param>
        public void SetAreaPropertyInfoLayerDB(SLGAreaPropertyInfoLayerDB propertyLayerDB)
        {
            m_PropertyInfoLayerDB = propertyLayerDB;
        }

        /// <summary>
        /// 初始化图层，解析本区域数据块并写入材质属性块所需的 UV 数组。
        /// </summary>
        public override void Init()
        {
            m_Render = true;

            InitPropertyInfoBlockDB();
            InitMatPropBlock();
        }

        /// <summary>
        /// 销毁图层，释放资源并调用基类逻辑。
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
        }

        /// <summary>
        /// 在需要显示时，使用本区域矩阵列表与材质属性块进行 GPU 实例化绘制。
        /// </summary>
        public override void Render()
        {
            if (!m_Render)
                return;

            if (m_Mesh == null || m_Mat == null)
                return;

            if (m_PropertyInfoBlockDB == null)
                return;

            Graphics.DrawMeshInstanced(m_Mesh, 0, m_Mat, m_PropertyInfoBlockDB.matrixList,
                    m_MatPropBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        }

        void InitMatPropBlock()
        {
            if (m_PropertyInfoBlockDB == null)
                return;

            m_MatPropBlock.SetVectorArray(SLGDefine.s_SLG_Shader_SceneObj_UvScaleOffsetId,
                    m_PropertyInfoBlockDB.uvScaleOffsetList);
        }

        void InitPropertyInfoBlockDB()
        {
            if (m_PropertyInfoLayerDB == null)
                return;

            var blockList = m_PropertyInfoLayerDB.blockList;
            if (blockList == null || blockList.Count <= 0)
                return;

            if (m_AreaIndex < 0 || m_AreaIndex >= blockList.Count)
                return;

            m_PropertyInfoBlockDB = blockList[m_AreaIndex];
        }
    }
}
