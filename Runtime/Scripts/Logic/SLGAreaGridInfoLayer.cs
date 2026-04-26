using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域网格信息图层，负责在地图格子上叠加颜色标记并支持小地图纹理填充。
    /// </summary>
    public class SLGAreaGridInfoLayer : SLGAreaInfoLayer
    {
        /// <summary>
        ///
        /// </summary>
        SLGAreaGridSetDB m_AreaGridSetDB;

        /// <summary>
        /// 记录当前已有数据的格子索引，用于快速判断是否有内容需要渲染。
        /// </summary>
        Dictionary<int, bool> m_DataExistDict = new Dictionary<int, bool>();

        /// <summary>
        /// 设置区域网格集合数据库，提供格子位置与矩阵信息。
        /// </summary>
        /// <param name="areaGridSetDB">区域网格集合数据库对象。</param>
        public void SetAreaGridSetDB(SLGAreaGridSetDB areaGridSetDB)
        {
            m_AreaGridSetDB = areaGridSetDB;
        }

        /// <summary>
        /// 初始化图层，清空数据并预填充矩阵列表与颜色列表。
        /// </summary>
        public override void Init()
        {
            m_DataExistDict.Clear();

            InitMatrixList();
            InitColorList();
        }

        /// <summary>
        /// 渲染当前帧有颜色数据的格子，使用 GPU 实例化绘制。
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
        /// 销毁图层，清空数据字典并调用基类销毁逻辑。
        /// </summary>
        public override void Destroy()
        {
            m_DataExistDict.Clear();
            base.Destroy();
        }

        /// <summary>
        /// 将脏颜色数据提交到 GPU 材质属性块。
        /// </summary>
        public override void SubmitGPU()
        {
            if (!m_Dirty)
                return;

            m_Dirty = false;
            m_MatPropBlock.SetVectorArray(SLGDefine.s_SLG_Shader_SceneObj_BaseColorId, m_ColorList);
        }

        /// <summary>
        /// 在指定逻辑坐标的格子上添加颜色标记。
        /// </summary>
        /// <param name="logicPos">逻辑格子坐标。</param>
        /// <param name="color">标记颜色。</param>
        public override void AddGridInfo(Vector2Int logicPos, Color color)
        {
            int index = SLGUtils.CalcAreaGridIndexByLogicPos(logicPos);
            if (index < 0 || index >= SLGDefine.s_SLG_Area_TotalGridNum)
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
        /// 移除指定逻辑坐标格子上的颜色标记。
        /// </summary>
        /// <param name="logicPos">逻辑格子坐标。</param>
        public override void RemoveGridInfo(Vector2Int logicPos)
        {
            int index = SLGUtils.CalcAreaGridIndexByLogicPos(logicPos);
            if (index < 0 || index >= SLGDefine.s_SLG_Area_TotalGridNum)
                return;

            m_MatrixList[index] = SLGUtils.s_UnVisMatrix;
            m_ColorList[index] = Color.clear;

            m_DataExistDict.Remove(index);

            m_Dirty = true;
        }

        /// <summary>
        /// 将当前图层所有格子的颜色数据写入小地图纹理。
        /// </summary>
        /// <param name="layerID">图层 ID。</param>
        /// <param name="tex">目标小地图纹理。</param>
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

                    if (index < 0 || index >= SLGDefine.s_SLG_Area_TotalGridNum)
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

        void InitColorList()
        {
            m_ColorList.Clear();

            int totalNum = SLGDefine.s_SLG_Area_TotalGridNum;
            for (int i = 0; i < totalNum; i++)
            {
                m_ColorList.Add(Color.clear);
            }
        }

        void InitMatrixList()
        {
            m_MatrixList.Clear();

            int totalNum = SLGDefine.s_SLG_Area_TotalGridNum;
            for (int i = 0; i < totalNum; i++)
            {
                m_MatrixList.Add(SLGUtils.s_UnVisMatrix);
            }
        }
    }
}

