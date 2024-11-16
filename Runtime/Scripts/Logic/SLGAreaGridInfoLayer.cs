using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGAreaGridInfoLayer : SLGAreaInfoLayer
    {
        /// <summary>
        /// 
        /// </summary>
        SLGAreaGridSetDB m_AreaGridSetDB;

        /// <summary>
        /// ???????
        /// </summary>
        Dictionary<int, bool> m_DataExistDict = new Dictionary<int, bool>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaGridDB"></param>
        public void SetAreaGridSetDB(SLGAreaGridSetDB areaGridSetDB)
        {
            m_AreaGridSetDB = areaGridSetDB;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            m_DataExistDict.Clear();

            InitMatrixList();
            InitColorList();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render()
        {
            if (m_Mesh == null || m_Mat == null)
                return;

            if (m_DataExistDict.Count <= 0)
                return;

            SubmitGPU();

            Graphics.DrawMeshInstanced(m_Mesh, 0, m_Mat, m_MatrixList,
                    m_MatPropBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Destroy()
        {
            m_DataExistDict.Clear();
            base.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SubmitGPU()
        {
            if (!m_Dirty)
                return;

            m_Dirty = false;
            m_MatPropBlock.SetVectorArray(SLGDefine.SLG_SHADER_SCENEOBJ_BASECOLOR_ID, m_ColorList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        /// <param name="color"></param>
        public override void AddGridInfo(Vector2Int logicPos, Color color)
        {
            int index = SLGUtils.CalcAreaGridIndexByLogicPos(logicPos);
            if (index < 0 || index >= SLGDefine.SLG_AREA_TOTAL_GRID_NUM)
                return;

            SLGAreaGridDB gridDB = m_AreaGridSetDB.FindAreaGridDB(index);
            if (gridDB == null)
                return;

            Matrix4x4 gridMatrix = gridDB.matrix * m_InitScaleMatrix;

            m_MatrixList[index] = gridMatrix;
            m_ColorList[index] = color;

            if (!m_DataExistDict.ContainsKey(index))
            {
                m_DataExistDict.Add(index, true);
            }

            m_Dirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        public override void RemoveGridInfo(Vector2Int logicPos)
        {
            int index = SLGUtils.CalcAreaGridIndexByLogicPos(logicPos);
            if (index < 0 || index >= SLGDefine.SLG_AREA_TOTAL_GRID_NUM)
                return;

            m_MatrixList[index] = SLGUtils.s_UnVisMatrix;
            m_ColorList[index] = Color.clear;

            m_DataExistDict.Remove(index);

            m_Dirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="tex"></param>
        public override void FillMiniMapTexture(int layerID, Texture2D tex)
        {

            Vector2Int startPos =  m_AreaGridSetDB.startPos;
            Vector2Int endPos = m_AreaGridSetDB.endPos;

            for (int y = startPos.y; y < endPos.y; y++)
            {
                for (int x = startPos.x; x < endPos.x; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    int index = SLGUtils.CalcAreaGridIndexByLogicPos(gridPos);

                    if (index < 0 || index >= SLGDefine.SLG_AREA_TOTAL_GRID_NUM)
                    {
                        //Debugger.LogErrorF("[SLG][FillMiniMapTexture] [x] {0} [y] {1}", x, y);
                    }
                    else
                    {
                        int texX = x - 1;
                        int texY = y - 1;
                        tex.SetPixel(texX, texY, m_ColorList[index]);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitColorList()
        {
            m_ColorList.Clear();

            int totalNum = SLGDefine.SLG_AREA_TOTAL_GRID_NUM;
            for (int i = 0; i < totalNum; i++)
            {
                m_ColorList.Add(Color.clear);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitMatrixList()
        {
            m_MatrixList.Clear();

            int totalNum = SLGDefine.SLG_AREA_TOTAL_GRID_NUM;
            for (int i = 0; i < totalNum; i++)
            {
                m_MatrixList.Add(SLGUtils.s_UnVisMatrix);
            }
        }
    }
}

