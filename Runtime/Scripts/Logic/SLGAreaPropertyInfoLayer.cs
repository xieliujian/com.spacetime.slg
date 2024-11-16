using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaPropertyInfoLayer : SLGAreaInfoLayer
    {
        /// <summary>
        /// 
        /// </summary>
        int m_AreaIndex;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaPropertyInfoLayerDB m_PropertyInfoLayerDB;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaPropertyInfoBlockDB m_PropertyInfoBlockDB;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetAreaIndex(int index)
        {
            m_AreaIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoLayerDB"></param>
        public void SetAreaPropertyInfoLayerDB(SLGAreaPropertyInfoLayerDB propertyLayerDB)
        {
            m_PropertyInfoLayerDB = propertyLayerDB;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            m_Render = false;

            InitPropertyInfoBlockDB();
            InitMatPropBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        void InitMatPropBlock()
        {
            if (m_PropertyInfoBlockDB == null)
                return;

            m_MatPropBlock.SetVectorArray(SLGDefine.SLG_SHADER_SCENEOBJ_UV_SCALE_OFFSET_ID,
                    m_PropertyInfoBlockDB.uvScaleOffsetList);
        }

        /// <summary>
        /// 
        /// </summary>
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
