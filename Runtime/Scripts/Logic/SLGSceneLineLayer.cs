using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 场景连线渲染层：按唯一 ID 管理多条线段，将实例分块填入 <see cref="SLGSceneLineBlock"/> 并批量绘制。
    /// </summary>
    public class SLGSceneLineLayer
    {
        const int INIT_BLOCK_NUM = 5;

        SLGResMgr m_ResMgr;

        SLGAreaInfoLayerDB m_InfoLayerDB;

        Mesh m_Mesh;

        Material m_Mat;

        float m_MeshLength;

        float m_MeshWidth;

        Dictionary<uint, int> m_UniqueID2IndexDict = new Dictionary<uint, int>();

        List<SLGSceneLineBlock> m_BlockList = new List<SLGSceneLineBlock>();

        /// <summary>
        /// 注入资源管理器。
        /// </summary>
        /// <param name="resMgr">资源管理器</param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

        /// <summary>
        /// 绑定连线层使用的信息层 DB（取自定义资源路径等）。
        /// </summary>
        /// <param name="infoLayerDB">信息层 DB</param>
        public void SetAreaInfoLayerDB(SLGAreaInfoLayerDB infoLayerDB)
        {
            m_InfoLayerDB = infoLayerDB;
        }

        /// <summary>
        /// 加载 Mesh/材质并预分配若干 <see cref="SLGSceneLineBlock"/>。
        /// </summary>
        public void Init()
        {
            Destroy();

            SLGRes res = m_ResMgr.FindCustomRes(m_InfoLayerDB.resPath);
            if (res == null)
                return;

            m_Mesh = res.mesh;
            m_Mat = res.mat;
            m_MeshLength = res.meshScale.x;
            m_MeshWidth = res.meshScale.z;

            InitBlockList();
        }

        /// <summary>
        /// 销毁所有块并清空 ID 映射。
        /// </summary>
        public void Destroy()
        {
            DestroyBlockList();
            m_UniqueID2IndexDict.Clear();
            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 绘制所有连线块。
        /// </summary>
        public void Render()
        {
            foreach(var block in m_BlockList)
            {
                if (block == null)
                    continue;

                block.Render();
            }
        }

        /// <summary>
        /// 添加或更新一条连线（世界坐标）；已存在相同 <paramref name="uniqueID"/> 则更新矩阵。
        /// </summary>
        /// <param name="uniqueID">业务侧唯一 ID</param>
        /// <param name="startPos">起点世界坐标</param>
        /// <param name="endPos">终点世界坐标</param>
        /// <param name="enemy">是否为敌方样式（Shader 参数）</param>
        public void AddSceneLineInfo(uint uniqueID, Vector3 startPos, Vector3 endPos, bool enemy)
        {
            if (m_UniqueID2IndexDict.ContainsKey(uniqueID))
            {
                int index = m_UniqueID2IndexDict[uniqueID];

                SLGSceneLineBlock block = FindBlock(index, out int matrixIndex);
                if (block != null)
                {
                    if (matrixIndex < 0)
                    {
                        //Debugger.LogErrorF("[SLGSceneLineLayer][AddSceneLineInfo][Exist] {0} {1} {2}", uniqueID, startPos, endPos);
                    }
                    else
                    {
                        block.AddSceneLineInfo(matrixIndex, startPos, endPos, enemy);
                    }
                }
                else
                {
                    //Debugger.LogErrorF("[SLGSceneLineLayer][AddSceneLineInfo][Exist][Block == null] {0} {1} {2}", uniqueID, startPos, endPos);
                }
            }
            else
            {
                SLGSceneLineBlock block = AddBlock(out int globalIndex, out int matrixIndex);
                if (block != null)
                {
                    if (globalIndex == -1 || matrixIndex == -1)
                    {
                        //Debugger.LogErrorF("[SLGSceneLineLayer][AddSceneLineInfo][UnExist] {0} {1} {2}", uniqueID, startPos, endPos);
                    }
                    else
                    {
                        block.AddSceneLineInfo(matrixIndex, startPos, endPos, enemy);
                        m_UniqueID2IndexDict.Add(uniqueID, globalIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定 ID 的连线并回收实例槽位。
        /// </summary>
        /// <param name="uniqueID">业务侧唯一 ID</param>
        public void RemoveSceneLineInfo(uint uniqueID)
        {
            if (m_UniqueID2IndexDict.ContainsKey(uniqueID))
            {
                int index = m_UniqueID2IndexDict[uniqueID];

                SLGSceneLineBlock block = FindBlock(index, out int matrixIndex);
                if (block != null)
                {
                    block.RemoveSceneLineInfo(matrixIndex);
                }

                m_UniqueID2IndexDict.Remove(uniqueID);
            }
        }

        SLGSceneLineBlock AddBlock(out int globalIndex, out int matrixIndex)
        {
            globalIndex = -1;
            matrixIndex = -1;

            SLGSceneLineBlock findBlock = null;
            int blockIndex = -1;

            for (int i = 0; i < m_BlockList.Count; i++)
            {
                var block = m_BlockList[i];
                if (block == null)
                    continue;

                bool isFull = block.IsFull();
                if (isFull)
                    continue;

                findBlock = block;
                blockIndex = i;
                break;
            }

            if (findBlock != null)
            {
                matrixIndex = findBlock.GetEmptyIndex();
                globalIndex = SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM * blockIndex + matrixIndex;
            }
            else
            {
                SLGSceneLineBlock block = InitBlock();
                m_BlockList.Add(block);

                findBlock = block;
                matrixIndex = findBlock.GetEmptyIndex();
                globalIndex = SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM * (m_BlockList.Count - 1) + matrixIndex;
            }

            return findBlock;
        }

        SLGSceneLineBlock FindBlock(int globalIndex, out int matrixIndex)
        {
            int blockIndex = globalIndex / SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM;
            matrixIndex = globalIndex % SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM;

            if (blockIndex < 0 || blockIndex >= m_BlockList.Count)
                return null;

            return m_BlockList[blockIndex];
        }

        SLGSceneLineBlock InitBlock()
        {
            SLGSceneLineBlock block = new SLGSceneLineBlock();
            block.SetMesh(m_Mesh);
            block.SetMat(m_Mat);
            block.SetMeshLength(m_MeshLength);
            block.SetMeshWidth(m_MeshWidth);
            block.Init();
            return block;
        }

        void InitBlockList()
        {
            for (int i = 0; i < INIT_BLOCK_NUM; i++)
            {
                SLGSceneLineBlock block = InitBlock();
                m_BlockList.Add(block);
            }
        }

        void DestroyBlockList()
        {
            foreach(var block in m_BlockList)
            {
                if (block == null)
                    continue;

                block.Destroy();
            }

            m_BlockList.Clear();
        }
    }
}
