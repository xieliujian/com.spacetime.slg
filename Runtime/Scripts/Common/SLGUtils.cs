
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using ST.Core.Debugger;

namespace ST.SLG
{
    /// <summary>
    /// SLG 通用工具方法集合，提供坐标转换、格子计算、相机控制、包围盒等辅助功能
    /// </summary>
    public class SLGUtils
    {
        /// <summary>
        /// ����
        /// </summary>
        const string SPLIT_STR = ",";

        /// <summary>
        /// 视锥体平面数量（6 个面）
        /// </summary>
        public const int FRUSTUM_PLANE_NUM = 6;

        /// <summary>
        /// 不可见对象使用的矩阵（缩放为零，使对象不渲染）
        /// </summary>
        public static Matrix4x4 s_UnVisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);

        /// <summary>
        /// 默认 UV 缩放偏移值（缩放 1,1，偏移 0,0）
        /// </summary>
        public static Vector4 s_DefaultUVScaleOffset = new Vector4(1, 1, 0, 0);

        /// <summary>
        /// 场景全局根节点名称
        /// </summary>
        public const string GLOBAL_ROOT_NAME = "Global";

        /// <summary>
        /// ������̬�����
        /// </summary>
        public const string SLG_SCENE_DYNAMIC_OBJ_GROUP_ROOT_NAME = "SLGSceneDynamicObjGroupRoot";

        /// <summary>
        /// AABB 与视锥体平面测试结果
        /// </summary>
        public enum TestPlanesResults
        {
            /// <summary>
            /// The AABB is completely in the frustrum.
            /// </summary>
            Inside = 0,
            /// <summary>
            /// The AABB is partially in the frustrum.
            /// </summary>
            Intersect,
            /// <summary>
            /// The AABB is completely outside the frustrum.
            /// </summary>
            Outside
        }

        /// <summary>
        /// 将 SLG 三维世界坐标转换为逻辑格子坐标
        /// </summary>
        /// <param name="pos">三维世界坐标</param>
        /// <returns>对应的逻辑格子坐标</returns>
        public static Vector2Int ConvertSLG3DPosToLogicPos(Vector3 pos)
        {
            var propPos = new Vector2Int(Mathf.FloorToInt(pos.x / SLGDefine.s_SLG_Grid_UnitSize),
                Mathf.FloorToInt(pos.z / SLGDefine.s_SLG_Grid_UnitSize));
            propPos += new Vector2Int(SLGDefine.s_SLG_LogicGrid_HorizontalOffset, SLGDefine.s_SLG_LogicGrid_VerticalOffset);

            return propPos;
        }

        /// <summary>
        /// 将 SLG 逻辑格子坐标转换为三维世界坐标（Y 轴为 0，坐标居中于格子中心）
        /// </summary>
        /// <param name="logicGridPos">逻辑格子坐标</param>
        /// <returns>对应的三维世界坐标</returns>
        public static Vector3 ConvertSLGLogicPosTo3DPos(Vector2Int logicGridPos)
        {
            var propPos = logicGridPos - new Vector2Int(SLGDefine.s_SLG_LogicGrid_HorizontalOffset, SLGDefine.s_SLG_LogicGrid_VerticalOffset);
            Vector3 newPos = new Vector3();
            newPos.x = propPos.x * SLGDefine.s_SLG_Grid_UnitSize + SLGDefine.s_SLG_Grid_UnitSize * 0.5f;
            newPos.y = 0f;
            newPos.z = propPos.y * SLGDefine.s_SLG_Grid_UnitSize + SLGDefine.s_SLG_Grid_UnitSize * 0.5f;

            return newPos;
        }

        /// <summary>
        /// �����߼���������������������ڿ��ٻ�ȡ����[0 ~ 25��
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static int CalcAreaIndexByLogicPos(Vector2Int logicPos)
        {
            int x = (logicPos.x - 1) / SLGDefine.s_SLG_Area_HorizontalGridNum;
            int y = (logicPos.y - 1) / SLGDefine.s_SLG_Area_VerticalGridNum;
            int index = (y * SLGDefine.s_SLG_Area_HorizontalNum) + x;

            return index;
        }

        /// <summary>
        /// ��������ĸ������� [0 ~ 100��
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static int CalcAreaGridIndexByLogicPos(Vector2Int logicPos)
        {
            int x = (logicPos.x - 1) % SLGDefine.s_SLG_Area_HorizontalGridNum;
            int y = (logicPos.y - 1) % SLGDefine.s_SLG_Area_VerticalGridNum;
            int index = (y * SLGDefine.s_SLG_Area_HorizontalGridNum) + x;

            return index;
        }

        /// <summary>
        /// 根据逻辑坐标计算属性格子的线性索引
        /// </summary>
        /// <param name="logicPos">逻辑格子坐标</param>
        /// <returns>属性格子线性索引</returns>
        public static int CalcPropertyGridIndex(Vector2Int logicPos)
        {
            int index = logicPos.x + (logicPos.y - 1) * SLGDefine.s_SLG_Grid_HorizontalNum;
            return index;
        }

        /// <summary>
        /// 根据序列索引计算 UV 缩放与偏移参数
        /// </summary>
        /// <param name="val">序列中的元素索引</param>
        /// <param name="seqWidth">序列宽度（列数）</param>
        /// <param name="seqHeight">序列高度（行数）</param>
        /// <returns>UV 缩放偏移向量（x,y 为缩放，z,w 为偏移）</returns>
        public static Vector4 CalcUVScaleOffset(int val, int seqWidth, int seqHeight)
        {
            float column = (val % seqWidth);

            float row = val / seqWidth;
            row = (seqHeight - 1) - row;

            Vector4 uvScaleOffset = new Vector4(1.0f / seqWidth, 1.0f / seqHeight,
                                    column / seqWidth, row / seqHeight);

            return uvScaleOffset;
        }

        /// <summary>
        /// 根据逻辑格子坐标计算该格子的变换矩阵
        /// </summary>
        /// <param name="logicGridPos">逻辑格子坐标</param>
        /// <returns>对应格子的 TRS 变换矩阵</returns>
        public static Matrix4x4 CalcSLGGridMatrix(Vector2Int logicGridPos)
        {
            Vector3 newPos = ConvertSLGLogicPosTo3DPos(logicGridPos);
            Vector3 scale = new Vector3(SLGDefine.s_SLG_Grid_UnitSize, 1f, SLGDefine.s_SLG_Grid_UnitSize);
            Matrix4x4 matrix = Matrix4x4.TRS(newPos, Quaternion.identity, scale);

            return matrix;
        }

        /// <summary>
        /// 将坐标的 Y 分量重置为 0，保留 X 和 Z
        /// </summary>
        /// <param name="pos">原始坐标</param>
        /// <returns>Y 轴归零后的坐标</returns>
        public static Vector3 ResetPosY(Vector3 pos)
        {
            return new Vector3(pos.x, 0, pos.z);
        }

        /// <summary>
        /// 将 Transform 的本地位置、旋转、缩放重置为默认值
        /// </summary>
        /// <param name="trans">要重置的 Transform</param>
        public static void ResetTransfrom(Transform trans)
        {
            if (trans == null)
                return;

            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        /// <summary>
        /// 根据 Mesh 的 UV 坐标范围计算 UV 缩放与偏移参数（要求 Mesh 恰好有 4 个顶点）
        /// </summary>
        /// <param name="mesh">目标网格</param>
        /// <returns>UV 缩放偏移向量（x,y 为缩放，z,w 为偏移），失败时返回 Vector4.zero</returns>
        public static Vector4 CalcUVScaleOffsetByMesh(Mesh mesh)
        {
            var uvArray = mesh.uv;
            if (uvArray == null || uvArray.Length != 4)
            {
                Debugger.LogDebugF("[SLG][CalcUVScaleOffsetByMesh] ģ�Ͷ�����Ŀ��Ϊ4 {0}", mesh.name);
                return Vector4.zero;
            }

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach(var uv in uvArray)
            {
                if (uv.x < min.x)
                {
                    min.x = uv.x;
                }

                if (uv.y < min.y)
                {
                    min.y = uv.y;
                }

                if (uv.x > max.x)
                {
                    max.x = uv.x;
                }

                if (uv.y > max.y)
                {
                    max.y = uv.y;
                }
            }

            var size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
            var offset = new Vector2(min.x, min.y);
            var uvScaleOffset = new Vector4(size.x, size.y, offset.x, offset.y);

            return uvScaleOffset;
        }

        /// <summary>
        /// 在场景中查找全局根节点 GameObject
        /// </summary>
        /// <returns>全局根节点，未找到时返回 null</returns>
        public static GameObject FindGlobalRoot()
        {
            var rootGo = GameObject.Find(GLOBAL_ROOT_NAME);
            if (rootGo == null)
                return null;

            return rootGo;
        }

        /// <summary>
        /// 在全局根节点下查找 SLG 场景动态对象组根节点
        /// </summary>
        /// <param name="globalRoot">全局根节点 GameObject</param>
        /// <returns>动态对象组根节点 Transform，未找到时返回 null</returns>
        public static Transform FindSLGSceneDynamicObjGroupRoot(GameObject globalRoot)
        {
            var root = globalRoot.transform.Find(SLG_SCENE_DYNAMIC_OBJ_GROUP_ROOT_NAME);
            if (root == null)
                return null;

            return root;
        }

        /// <summary>
        /// 根据组索引设置 SLG 场景动态对象组的可见性，仅指定索引的组可见
        /// </summary>
        /// <param name="index">要显示的动态对象组索引</param>
        public static void SetSLGSceneDynamicObjGroup(int index)
        {
            var globalGo = FindGlobalRoot();
            if (globalGo == null)
                return;

            var root = FindSLGSceneDynamicObjGroupRoot(globalGo);
            if (root == null)
                return;

            var groupArray = root.GetComponentsInChildren<SLGSceneDynamicObjGroup>(true);
            if (groupArray == null || groupArray.Length <= 0)
                return;

            foreach(var group in groupArray)
            {
                if (group == null)
                    continue;

                var isVisible = (group.groupIndex == index) ? true : false;
                group.SetVisible(isVisible);
            }
        }

        /// <summary>
        /// 根据输入与高度限制计算并更新 SLG 相机位置
        /// </summary>
        /// <param name="camTrans">相机 Transform</param>
        /// <param name="camMinHeight">相机最小高度</param>
        /// <param name="camMaxHeight">相机最大高度</param>
        /// <param name="camMoveSpeed">相机平移速度</param>
        /// <param name="camWheelSpeed">滚轮缩放速度</param>
        /// <param name="inputHorizontal">水平输入值</param>
        /// <param name="inputVertical">垂直输入值</param>
        /// <param name="inputScroll">滚轮输入值</param>
        /// <param name="camEulerAngle">相机欧拉角</param>
        public static void CalcSLGCameraPos(Transform camTrans, float camMinHeight, float camMaxHeight, float camMoveSpeed, float camWheelSpeed,
                    float inputHorizontal, float inputVertical, float inputScroll, Vector3 camEulerAngle)
        {
            camTrans.localRotation = Quaternion.Euler(camEulerAngle);

            float moveSpeed = Time.deltaTime * camMoveSpeed;
            var camRight = Vector3.Normalize(new Vector3(camTrans.right.x, 0f, camTrans.right.z));
            var camUp = Vector3.Normalize(Quaternion.Euler(0, -90f, 0) * camRight);

            camTrans.position += camRight * moveSpeed * inputHorizontal;
            camTrans.position += camUp * moveSpeed * inputVertical;

            float wheelSpeed = Time.deltaTime * camWheelSpeed * 10000f;
            var camPos = camTrans.position;
            camPos += camTrans.forward * wheelSpeed * inputScroll;
            if (camPos.y < camMinHeight)
            {
                float revertPer = (camMinHeight - camPos.y) / camTrans.forward.y;
                camPos += camTrans.forward * revertPer;
            }
            else if (camPos.y > camMaxHeight)
            {
                float revertPer = (camMaxHeight - camPos.y) / camTrans.forward.y;
                camPos += camTrans.forward * revertPer;
            }

            camTrans.position = camPos;
        }

        /// <summary>
        /// 计算 GameObject 列表中所有 Renderer 的合并包围盒
        /// </summary>
        /// <param name="objList">GameObject 列表</param>
        /// <returns>合并后的包围盒</returns>
        public static Bounds CalcObjListBounds(List<GameObject> objList)
        {
            // https://www.xuanyusong.com/archives/3461

            Vector3 center = Vector3.zero;
            Bounds bounds = new Bounds(center, Vector3.zero);

            List<Renderer> renderList = new List<Renderer>();
            foreach(var obj in objList)
            {
                if (obj == null)
                    continue;

                var renderArray = obj.GetComponentsInChildren<Renderer>();
                if (renderArray == null)
                    continue;

                foreach(var render in renderArray)
                {
                    if (render == null)
                        continue;

                    renderList.Add(render);
                }
            }

            if (renderList.Count <= 0)
                return bounds;

            foreach (Renderer child in renderList)
            {
                center += child.bounds.center;
            }

            center /= renderList.Count;

            bounds = new Bounds(center, Vector3.zero);
            foreach (Renderer child in renderList)
            {
                bounds.Encapsulate(child.bounds);
            }

            return bounds;
        }

        /// <summary>
        /// 在指定 GameObject 的直接子节点中按名称查找子对象
        /// </summary>
        /// <param name="rootGo">父节点 GameObject</param>
        /// <param name="childName">要查找的子节点名称</param>
        /// <returns>匹配的子节点 GameObject，未找到时返回 null</returns>
        public static GameObject GetChildByName(GameObject rootGo, string childName)
        {
            if (rootGo == null)
                return null;

            var rootTrans = rootGo.transform;
            var childCount = rootTrans.childCount;
            if (childCount <= 0)
                return null;

            for (int i = 0; i < childCount; i++)
            {
                var child = rootTrans.GetChild(i);
                if (child == null)
                    continue;

                if (child.name == childName)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 将渲染数据填充到区域地图块数据库，按资源 ID 分组存储变换矩阵与 UV 信息
        /// </summary>
        /// <param name="blockList">区域地图块数据库列表</param>
        /// <param name="resDB">场景资源数据库</param>
        /// <param name="resId">资源 ID</param>
        /// <param name="pos">位置</param>
        /// <param name="rot">旋转（欧拉角）</param>
        /// <param name="scale">缩放</param>
        /// <param name="uvScaleOffset">UV 缩放偏移</param>
        public static void FillRenderBlockDB(List<SLGAreaMapBlockDB> blockList, SLGSceneResDB resDB, int resId,
            Vector3 pos, Vector3 rot, Vector3 scale, Vector4 uvScaleOffset)
        {
            if (resId < 0)
                return;

            var block = GetOrCreateRenderBlockDB(blockList, resId);
            if (block == null)
                return;

            var res = resDB.FindResByResID(resId);
            if (res == null)
                return;

            var quat = Quaternion.Euler(rot);
            var matrix = Matrix4x4.TRS(pos, quat, scale);

            block.matrixList.Add(matrix);
            block.uvScaleOffsetList.Add(uvScaleOffset);
        }

        /// <summary>
        /// 从列表中获取指定资源 ID 的区域地图块数据库，不存在时自动创建并添加
        /// </summary>
        /// <param name="blockList">区域地图块数据库列表</param>
        /// <param name="resId">资源 ID</param>
        /// <returns>对应的区域地图块数据库对象</returns>
        public static SLGAreaMapBlockDB GetOrCreateRenderBlockDB(List<SLGAreaMapBlockDB> blockList, int resId)
        {
            SLGAreaMapBlockDB findBlockDB = null;

            foreach (var block in blockList)
            {
                if (block == null)
                    continue;

                if (block.resID == resId)
                {
                    findBlockDB = block;
                    break;
                }
            }

            if (findBlockDB == null)
            {
                findBlockDB = new SLGAreaMapBlockDB();
                findBlockDB.resID = resId;
                blockList.Add(findBlockDB);
            }

            return findBlockDB;
        }

        /// <summary>
        /// This is a faster AABB cull than brute force that also gives additional info on intersections.
        /// Calling Bounds.Min/Max is actually quite expensive so as an optimization you can precalculate these.
        /// http://www.lighthouse3d.com/tutorials/view-frustum-culling/geometric-approach-testing-boxes-ii/
        /// </summary>
        /// <param name="planes"></param>
        /// <param name="boundsMin"></param>
        /// <param name="boundsMax"></param>
        /// <returns></returns>
        public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Vector3 boundsMin, ref Vector3 boundsMax, bool testIntersection = false)
        {
            Vector3 vmin, vmax;
            var testResult = TestPlanesResults.Inside;

            for (int planeIndex = 0; planeIndex < planes.Length; planeIndex++)
            {
                var normal = planes[planeIndex].normal;
                var planeDistance = planes[planeIndex].distance;

                // X axis
                if (normal.x < 0)
                {
                    vmin.x = boundsMin.x;
                    vmax.x = boundsMax.x;
                }
                else
                {
                    vmin.x = boundsMax.x;
                    vmax.x = boundsMin.x;
                }

                // Y axis
                if (normal.y < 0)
                {
                    vmin.y = boundsMin.y;
                    vmax.y = boundsMax.y;
                }
                else
                {
                    vmin.y = boundsMax.y;
                    vmax.y = boundsMin.y;
                }

                // Z axis
                if (normal.z < 0)
                {
                    vmin.z = boundsMin.z;
                    vmax.z = boundsMax.z;
                }
                else
                {
                    vmin.z = boundsMax.z;
                    vmax.z = boundsMin.z;
                }

                var dot1 = normal.x * vmin.x + normal.y * vmin.y + normal.z * vmin.z;
                if (dot1 + planeDistance < 0)
                    return TestPlanesResults.Outside;

                if (testIntersection)
                {
                    var dot2 = normal.x * vmax.x + normal.y * vmax.y + normal.z * vmax.z;
                    if (dot2 + planeDistance <= 0)
                        testResult = TestPlanesResults.Intersect;
                }
            }

            return testResult;
        }

        /// <summary>
        /// ��ȡCsv����
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string[]> ReadCsv(string path)
        {
            string[] splitarray = { SPLIT_STR };
            List<string[]> list = new List<string[]>();
            string line;

            StreamReader stream = new StreamReader(path);
            if (stream != null)
            {
                while ((line = stream.ReadLine()) != null)
                {
                    list.Add(line.Split(splitarray, StringSplitOptions.None));
                }

                stream.Close();
                stream.Dispose();
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Texture2D CreateSLGMiniMapTexture()
        {
            Texture2D tex = new Texture2D(SLGDefine.s_SLG_MiniMap_TexWidth,
                            SLGDefine.s_SLG_MiniMap_TexHeight,
                            TextureFormat.RGBA32, false, false);

            tex.filterMode = FilterMode.Point;

            return tex;
        }
    }
}

