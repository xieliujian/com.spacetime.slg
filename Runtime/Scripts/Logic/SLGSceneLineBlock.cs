
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LR.SLG
{
    /// <summary>
    /// SceneLine????
    /// </summary>
    public class SLGSceneLineBlock
    {
        /// <summary>
        /// 
        /// </summary>
        public const int SLG_LINE_BLOCK_MATRIX_NUM = 300;

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
        float m_MeshLength;

        /// <summary>
        /// 
        /// </summary>
        float m_MeshWidth;

        /// <summary>
        /// 
        /// </summary>
        MaterialPropertyBlock m_MatPropBlock = new MaterialPropertyBlock();

        /// <summary>
        /// 
        /// </summary>
        List<Matrix4x4> m_MatrixList = new List<Matrix4x4>();

        /// <summary>
        /// 
        /// </summary>
        List<float> m_EnemyPropList = new List<float>();

        /// <summary>
        /// 
        /// </summary>
        List<Vector4> m_UVScaleOffsetPropList = new List<Vector4>();

        /// <summary>
        /// 
        /// </summary>
        bool m_Dirty = false;

        /// <summary>
        /// 判断是否有数据
        /// </summary>
        Dictionary<int, bool> m_DataExistDict = new Dictionary<int, bool>(SLG_LINE_BLOCK_MATRIX_NUM);

        /// <summary>
        /// 
        /// </summary>
        Stack<int> m_EmptyIndexStack = new Stack<int>(SLG_LINE_BLOCK_MATRIX_NUM);
        
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
        /// <param name="meshLength"></param>
        public void SetMeshLength(float meshLength)
        {
            m_MeshLength = meshLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshWidth"></param>
        public void SetMeshWidth(float meshWidth)
        {
            m_MeshWidth = meshWidth;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            m_DataExistDict.Clear();

            InitMatrixList();
            InitEmptyIndexSet();
            InitAllShaderProperty();
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="enemy"></param>
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
        /// 
        /// </summary>
        /// <param name="index"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return m_DataExistDict.Count == SLG_LINE_BLOCK_MATRIX_NUM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        Vector4 CalcLineUVScaleOffset(Vector3 startPos, Vector3 endPos)
        {
            Vector4 uvScaleOffset = SLGUtils.s_DefaultUVScaleOffset;

            float lineLength = Vector3.Distance(startPos, endPos);
            float uvScaleX = lineLength / m_MeshLength;
            uvScaleOffset.x = uvScaleX;

            // 目标点uv.X始终为0，防止起始点移动时候的画面抖动
            uvScaleOffset.z = -uvScaleX;

            return uvScaleOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        void InitMatrixList()
        {
            m_MatrixList.Clear();

            int totalNum = SLG_LINE_BLOCK_MATRIX_NUM;
            for (int i = 0; i < totalNum; i++)
            {
                m_MatrixList.Add(SLGUtils.s_UnVisMatrix);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitEmptyIndexSet()
        {
            m_EmptyIndexStack.Clear();

            int totalNum = SLG_LINE_BLOCK_MATRIX_NUM;
            for (int i = totalNum - 1; i >= 0; i--)
            {
                m_EmptyIndexStack.Push(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        void SubmitGPU()
        {
            if (!m_Dirty)
                return;

            m_Dirty = false;

            m_MatPropBlock.SetFloatArray(SLGDefine.SLG_SHADER_SCENELINE_ENEMY_ID, m_EnemyPropList);
            m_MatPropBlock.SetVectorArray(SLGDefine.SLG_SHADER_SCENELINE_UV_SCALE_OFFSET_ID, m_UVScaleOffsetPropList);
        }
    }
}




