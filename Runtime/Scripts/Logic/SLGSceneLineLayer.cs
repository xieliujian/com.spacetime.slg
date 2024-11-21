using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGSceneLineLayer
    {
        /// <summary>
        /// 
        /// </summary>
        const int INIT_BLOCK_NUM = 5;

        /// <summary>
        /// 
        /// </summary>
        SLGResMgr m_ResMgr;

        /// <summary>
        /// 
        /// </summary>
        SLGAreaInfoLayerDB m_InfoLayerDB;

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
        Dictionary<uint, int> m_UniqueID2IndexDict = new Dictionary<uint, int>();

        /// <summary>
        /// 
        /// </summary>
        List<SLGSceneLineBlock> m_BlockList = new List<SLGSceneLineBlock>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resMgr"></param>
        public void SetResMgr(SLGResMgr resMgr)
        {
            m_ResMgr = resMgr;
        }

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
        /// 
        /// </summary>
        public void Destroy()
        {
            DestroyBlockList();
            m_UniqueID2IndexDict.Clear();
            m_Mesh = null;
            m_Mat = null;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="enemy"></param>
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
        /// 
        /// </summary>
        /// <param name="uniqueID"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <param name="blockIndex"></param>
        /// <returns></returns>
        SLGSceneLineBlock FindBlock(int globalIndex, out int matrixIndex)
        {
            int blockIndex = globalIndex / SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM;
            matrixIndex = globalIndex % SLGSceneLineBlock.SLG_LINE_BLOCK_MATRIX_NUM;

            if (blockIndex < 0 || blockIndex >= m_BlockList.Count)
                return null;

            return m_BlockList[blockIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        void InitBlockList()
        {
            for (int i = 0; i < INIT_BLOCK_NUM; i++)
            {
                SLGSceneLineBlock block = InitBlock();
                m_BlockList.Add(block);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

