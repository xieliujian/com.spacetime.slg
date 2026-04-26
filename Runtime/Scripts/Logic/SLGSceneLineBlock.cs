
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// 场景连线 GPU 实例块：单块内固定数量 <see cref="SLG_LINE_BLOCK_MATRIX_NUM"/> 条线段，使用 <see cref="Graphics.DrawMeshInstanced"/> 提交矩阵与材质属性。
    /// </summary>
    public class SLGSceneLineBlock
    {
        /// <summary>
        /// 单块可容纳的连线实例槽位数量。
        /// </summary>
        public const int SLG_LINE_BLOCK_MATRIX_NUM = 300;

        Mesh m_Mesh;

        Material m_Mat;

        float m_MeshLength;

        float m_MeshWidth;

        MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        List<Matrix4x4> m_MatrixList = new List<Matrix4x4>();

        List<float> m_EnemyPropList = new List<float>();

        List<Vector4> m_UVScaleOffsetPropList = new List<Vector4>();

        bool m_Dirty = false;

        /// <summary>
        /// 槽位索引到是否占用连线的映射。
        /// </summary>
        Dictionary<int, bool> m_DataExistDict = new Dictionary<int, bool>(SLG_LINE_BLOCK_MATRIX_NUM);

        Stack<int> m_EmptyIndexStack = new Stack<int>(SLG_LINE_BLOCK_MATRIX_NUM);

        /// <summary>
        /// 设置用于绘制的网格。
        /// </summary>
        /// <param name="mesh">线段 Mesh</param>
        public void SetMesh(Mesh mesh)
        {
            m_Mesh = mesh;
        }

        /// <summary>
        /// 设置用于绘制的材质。
        /// </summary>
        /// <param name="mat">线段材质</param>
        public void SetMat(Material mat)
        {
            m_Mat = mat;
        }

        /// <summary>
        /// 设置线段预制在 X 方向的参考长度（用于 UV 缩放）。
        /// </summary>
        /// <param name="meshLength">Mesh 长度</param>
        public void SetMeshLength(float meshLength)
        {
            m_MeshLength = meshLength;
        }

        /// <summary>
        /// 设置线段在 Z 方向的宽度缩放参考。
        /// </summary>
        /// <param name="meshWidth">Mesh 宽度</param>
        public void SetMeshWidth(float meshWidth)
        {
            m_MeshWidth = meshWidth;
        }

        /// <summary>
        /// 初始化矩阵列表、空闲栈与 Shader 属性数组。
        /// </summary>
        public void Init()
        {
            m_DataExistDict.Clear();

            InitMatrixList();
            InitEmptyIndexSet();
            InitAllShaderProperty();
        }

        /// <summary>
        /// 清空 GPU 侧缓存与引用。
        /// </summary>
        public void Destroy()
        {
            m_DataExistDict.Clear();
            m_EmptyIndexStack.Clear();

            m_MatPropBlock.Clear();
            m_MatrixList.Clear();
            m_EnemyPropList.Clear();
            m_UVScaleOffsetPropList.Clear();

            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 提交材质属性变更并绘制本块内所有有效实例。
        /// </summary>
        public void Render()
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
        /// 在指定槽位写入一条连线的变换与样式数据。
        /// </summary>
        /// <param name="index">槽位索引 0 .. <see cref="SLG_LINE_BLOCK_MATRIX_NUM"/>-1</param>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <param name="enemy">敌方标记</param>
        public void AddSceneLineInfo(int index, Vector3 startPos, Vector3 endPos, bool enemy)
        {
            if (index < 0 || index >= SLG_LINE_BLOCK_MATRIX_NUM)
                return;

            Matrix4x4 matrix = CalcLineMatrix(startPos, endPos);
            Vector4 uvScaleOffset = CalcLineUVScaleOffset(startPos, endPos);

            m_MatrixList[index] = matrix;
            m_EnemyPropList[index] = enemy ? 1 : 0;
            m_UVScaleOffsetPropList[index] = uvScaleOffset;

            if (!m_DataExistDict.ContainsKey(index))
            {
                m_DataExistDict.Add(index, true);
            }

            m_Dirty = true;
        }

        /// <summary>
        /// 清空指定槽位并将索引压回空闲栈。
        /// </summary>
        /// <param name="index">槽位索引</param>
        public void RemoveSceneLineInfo(int index)
        {
            if (index < 0 || index >= SLG_LINE_BLOCK_MATRIX_NUM)
                return;

            m_MatrixList[index] = SLGUtils.s_UnVisMatrix;
            m_EnemyPropList[index] = 0;
            m_UVScaleOffsetPropList[index] = SLGUtils.s_DefaultUVScaleOffset;

            m_DataExistDict.Remove(index);

            if (!m_EmptyIndexStack.Contains(index))
            {
                m_EmptyIndexStack.Push(index);
            }
            else
            {
                //Debugger.LogErrorF("[SLG][RemoveSceneLineInfo] {0}", index);
            }

            m_Dirty = true;
        }

        /// <summary>
        /// 从空闲栈弹出一个可用槽位索引。
        /// </summary>
        /// <returns>索引，栈空返回 -1</returns>
        public int GetEmptyIndex()
        {
            if (m_EmptyIndexStack.Count <= 0)
            {
                //Debugger.LogErrorF("[SLG][GetEmptyIndex] {Empty}");
                return -1;
            }

            int index = m_EmptyIndexStack.Pop();
            return index;
        }

        /// <summary>
        /// 当前块是否已无空闲槽位。
        /// </summary>
        /// <returns>已满返回 true</returns>
        public bool IsFull()
        {
            return m_DataExistDict.Count == SLG_LINE_BLOCK_MATRIX_NUM;
        }

        Vector4 CalcLineUVScaleOffset(Vector3 startPos, Vector3 endPos)
        {
            Vector4 uvScaleOffset = SLGUtils.s_DefaultUVScaleOffset;

            float lineLength = Vector3.Distance(startPos, endPos);
            float uvScaleX = lineLength / m_MeshLength;
            uvScaleOffset.x = uvScaleX;

            // 目标点 uv.X 始终为 0，防止起始点移动时的画面抖动
            uvScaleOffset.z = -uvScaleX;

            return uvScaleOffset;
        }

        Matrix4x4 CalcLineMatrix(Vector3 startPos, Vector3 endPos)
        {
            Vector3 pos = (endPos + startPos) / 2;
            Vector3 dir = (endPos - startPos).normalized;

            //Quaternion rot = Quaternion.LookRotation(dir);
            float angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.up);
            Quaternion rot = Quaternion.Euler(0, angle, 0);

            float length = Vector3.Distance(startPos, endPos);
            Vector3 scale = new Vector3(length, 1, m_MeshWidth);

            Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scale);

            return matrix;
        }

        void InitMatrixList()
        {
            m_MatrixList.Clear();

            int totalNum = SLG_LINE_BLOCK_MATRIX_NUM;
            for (int i = 0; i < totalNum; i++)
            {
                m_MatrixList.Add(SLGUtils.s_UnVisMatrix);
            }
        }

        void InitEmptyIndexSet()
        {
            m_EmptyIndexStack.Clear();

            int totalNum = SLG_LINE_BLOCK_MATRIX_NUM;
            for (int i = totalNum - 1; i >= 0; i--)
            {
                m_EmptyIndexStack.Push(i);
            }
        }

        void InitAllShaderProperty()
        {
            m_EnemyPropList.Clear();
            m_UVScaleOffsetPropList.Clear();

            int totalNum = SLG_LINE_BLOCK_MATRIX_NUM;
            for (int i = 0; i < totalNum; i++)
            {
                m_EnemyPropList.Add(0);
                m_UVScaleOffsetPropList.Add(Vector4.zero);
            }
        }

        void SubmitGPU()
        {
            if (!m_Dirty)
                return;

            m_Dirty = false;

            m_MatPropBlock.SetFloatArray(SLGDefine.s_SLG_Shader_SceneLine_EnemyId, m_EnemyPropList);
            m_MatPropBlock.SetVectorArray(SLGDefine.s_SLG_Shader_SceneLine_UvScaleOffsetId, m_UVScaleOffsetPropList);
        }
    }
}
