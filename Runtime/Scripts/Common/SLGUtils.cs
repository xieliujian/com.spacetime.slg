
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public class SLGUtils
    {
        /// <summary>
        /// 常量
        /// </summary>
        const string SPLIT_STR = ",";

        /// <summary>
        /// 
        /// </summary>
        public const int FRUSTUM_PLANE_NUM = 6;

        /// <summary>
        /// 不显示的矩阵
        /// </summary>
        public static Matrix4x4 s_UnVisMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);

        /// <summary>
        /// 
        /// </summary>
        public static Vector4 s_DefaultUVScaleOffset = new Vector4(1, 1, 0, 0);

        /// <summary>
        /// 
        /// </summary>
        public const string GLOBAL_ROOT_NAME = "Global";

        /// <summary>
        /// 场景动态物件组
        /// </summary>
        public const string SLG_SCENE_DYNAMIC_OBJ_GROUP_ROOT_NAME = "SLGSceneDynamicObjGroupRoot";

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2Int ConvertSLG3DPosToLogicPos(Vector3 pos)
        {
            var propPos = new Vector2Int(Mathf.FloorToInt(pos.x / SLGDefine.SLG_GRID_UNIT_SIZE),
                Mathf.FloorToInt(pos.z / SLGDefine.SLG_GRID_UNIT_SIZE));
            propPos += new Vector2Int(SLGDefine.SLG_LOGIC_GRID_HORIZONTAL_OFFSET, SLGDefine.SLG_LOGIC_GRID_VERTICAL_OFFSET);

            return propPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static Vector3 ConvertSLGLogicPosTo3DPos(Vector2Int logicGridPos)
        {
            var propPos = logicGridPos - new Vector2Int(SLGDefine.SLG_LOGIC_GRID_HORIZONTAL_OFFSET, SLGDefine.SLG_LOGIC_GRID_VERTICAL_OFFSET);
            Vector3 newPos = new Vector3();
            newPos.x = propPos.x * SLGDefine.SLG_GRID_UNIT_SIZE + SLGDefine.SLG_GRID_UNIT_SIZE * 0.5f;
            newPos.y = 0f;
            newPos.z = propPos.y * SLGDefine.SLG_GRID_UNIT_SIZE + SLGDefine.SLG_GRID_UNIT_SIZE * 0.5f;

            return newPos;
        }

        /// <summary>
        /// 根据逻辑坐标计算区域索引（用于快速获取区域）[0 ~ 25）
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static int CalcAreaIndexByLogicPos(Vector2Int logicPos)
        {
            int x = (logicPos.x - 1) / SLGDefine.SLG_AREA_HORIZONTAL_GRID_NUM;
            int y = (logicPos.y - 1) / SLGDefine.SLG_AREA_VERTICAL_GRID_NUM;
            int index = (y * SLGDefine.SLG_AREA_HORIZONTAL_NUM) + x;

            return index;
        }

        /// <summary>
        /// 计算区域的格子索引 [0 ~ 100）
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static int CalcAreaGridIndexByLogicPos(Vector2Int logicPos)
        {
            int x = (logicPos.x - 1) % SLGDefine.SLG_AREA_HORIZONTAL_GRID_NUM;
            int y = (logicPos.y - 1) % SLGDefine.SLG_AREA_VERTICAL_GRID_NUM;
            int index = (y * SLGDefine.SLG_AREA_HORIZONTAL_GRID_NUM) + x;

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public static int CalcPropertyGridIndex(Vector2Int logicPos)
        {
            int index = logicPos.x + (logicPos.y - 1) * SLGDefine.SLG_GRID_HORIZONTAL_NUM;
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="seqWidth"></param>
        /// <param name="seqHeight"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="logicGridPos"></param>
        /// <returns></returns>
        public static Matrix4x4 CalcSLGGridMatrix(Vector2Int logicGridPos)
        {
            Vector3 newPos = ConvertSLGLogicPosTo3DPos(logicGridPos);
            Vector3 scale = new Vector3(SLGDefine.SLG_GRID_UNIT_SIZE, 1f, SLGDefine.SLG_GRID_UNIT_SIZE);
            Matrix4x4 matrix = Matrix4x4.TRS(newPos, Quaternion.identity, scale);

            return matrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector3 ResetPosY(Vector3 pos)
        {
            return new Vector3(pos.x, 0, pos.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetTransfrom(Transform trans)
        {
            if (trans == null)
                return;

            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Vector4 CalcUVScaleOffsetByMesh(Mesh mesh)
        {
            var uvArray = mesh.uv;
            if (uvArray == null || uvArray.Length != 4)
            {
                Debug.Log($"[SLG][CalcUVScaleOffsetByMesh] 模型顶点数目不为4 {mesh.name}");
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static GameObject FindGlobalRoot()
        {
            var rootGo = GameObject.Find(GLOBAL_ROOT_NAME);
            if (rootGo == null)
                return null;

            return rootGo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Transform FindSLGSceneDynamicObjGroupRoot(GameObject globalRoot)
        {
            var root = globalRoot.transform.Find(SLG_SCENE_DYNAMIC_OBJ_GROUP_ROOT_NAME);
            if (root == null)
                return null;

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
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
        /// 
        /// </summary>
        /// <param name="camTrans"></param>
        /// <param name="camMinHeight"></param>
        /// <param name="camMaxHeight"></param>
        /// <param name="camMoveSpeed"></param>
        /// <param name="camWheelSpeed"></param>
        /// <param name="inputHorizontal"></param>
        /// <param name="inputVertical"></param>
        /// <param name="inputScroll"></param>
        /// <param name="camEulerAngle"></param>
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
        /// 
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="rootGo"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="resDB"></param>
        /// <param name="resId"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
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
        /// 
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
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
        /// 读取Csv表格
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
            Texture2D tex = new Texture2D(SLGDefine.SLG_MINIMAP_TEX_WIDTH,
                            SLGDefine.SLG_MINIMAP_TEX_HEIGHT,
                            TextureFormat.RGBA32, false, false);

            tex.filterMode = FilterMode.Point;

            return tex;
        }
    }
}

