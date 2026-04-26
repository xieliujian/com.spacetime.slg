using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ST.SLG
{
    /// <summary>
    /// SLG场景数据库，作为ScriptableObject存储场景所有资源、属性和区域数据。
    /// </summary>
    [System.Serializable]
    public class SLGSceneDB : ScriptableObject
    {
        /// <summary>
        /// 场景资源数据库，管理所有渲染资源引用。
        /// </summary>
        [SerializeField]
        public SLGSceneResDB resDB = new SLGSceneResDB();

        /// <summary>
        /// 场景属性数据库，存储格子属性信息。
        /// </summary>
        [SerializeField]
        public SLGScenePropDB propDB = new SLGScenePropDB();

        /// <summary>
        /// 区域集合数据库，管理所有区域分块数据。
        /// </summary>
        [SerializeField]
        public SLGAreaSetDB areaSetDB = new SLGAreaSetDB();

        /// <summary>
        /// 初始化场景数据库，创建所有区域分块。
        /// </summary>
        public void Init()
        {
            areaSetDB.Init();
        }

        /// <summary>
        /// 填充区域信息层数据，将资源路径和层类型写入对应的信息层数据库。
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <param name="resPath">资源路径</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        /// <param name="infoLayerType">信息层类型</param>
        /// <param name="areaPropertyLayerType">区域格子属性层类型</param>
        /// <param name="propertyTexSeq">属性贴图序列尺寸</param>
        public void FillAreaInfoLayerDB(int layerID, string resPath, int renderQueue, bool isZWriteOn,
            SLGDefine.SLGInfoLayerType infoLayerType, SLGDefine.SLGAreaGridPropertyLayerType areaPropertyLayerType,
            Vector2Int propertyTexSeq)
        {
            areaSetDB.FillAreaInfoLayerDB(layerID, resPath, resDB, renderQueue, isZWriteOn, infoLayerType,
                areaPropertyLayerType, propertyTexSeq);
        }


        /// <summary>
        /// 填充区域地图渲染块数据，将对象的变换信息写入对应区域的地图层。
        /// </summary>
        /// <param name="layerID">层ID</param>
        /// <param name="obj">场景对象</param>
        /// <param name="prefabPath">预制体路径</param>
        /// <param name="matPath">材质路径</param>
        /// <param name="pos">世界坐标位置</param>
        /// <param name="rot">旋转角度</param>
        /// <param name="scale">缩放比例</param>
        /// <param name="uvScaleOffset">UV缩放偏移</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        public void FillAreaMapDB(int layerID, GameObject obj, string prefabPath, string matPath,
                        Vector3 pos, Vector3 rot, Vector3 scale, Vector4 uvScaleOffset,
                        int renderQueue, bool isZWriteOn)
        {
            var areaDB = areaSetDB.GetAreaDB(pos);
            if (areaDB == null)
                return;

            resDB.AddRes(prefabPath, matPath, renderQueue, isZWriteOn);
            var resId = resDB.FindResId(matPath);

            areaDB.mapLayerSet.FillRenderBlockDB(layerID, resDB, resId, pos, rot, scale, uvScaleOffset);
            areaDB.AddObj(obj);
        }

        /// <summary>
        /// 填充区域动态地图渲染块数据，将对象的变换信息写入对应区域的动态地图层。
        /// </summary>
        /// <param name="dynamicMapIndex">动态地图索引</param>
        /// <param name="layerID">层ID</param>
        /// <param name="obj">场景对象</param>
        /// <param name="prefabPath">预制体路径</param>
        /// <param name="matPath">材质路径</param>
        /// <param name="pos">世界坐标位置</param>
        /// <param name="rot">旋转角度</param>
        /// <param name="scale">缩放比例</param>
        /// <param name="uvScaleOffset">UV缩放偏移</param>
        /// <param name="renderQueue">渲染队列值</param>
        /// <param name="isZWriteOn">是否开启深度写入</param>
        public void FillAreaDynamicMapDB(int dynamicMapIndex, int layerID, GameObject obj, string prefabPath, string matPath,
                        Vector3 pos, Vector3 rot, Vector3 scale, Vector4 uvScaleOffset,
                        int renderQueue, bool isZWriteOn)
        {
            var areaDB = areaSetDB.GetAreaDB(pos);
            if (areaDB == null)
                return;

            resDB.AddRes(prefabPath, matPath, renderQueue, isZWriteOn);
            var resId = resDB.FindResId(prefabPath);

            areaDB.dynamicMapLayerSet.FillRenderBlockDB(dynamicMapIndex, layerID, resDB, resId, pos, rot, scale, uvScaleOffset);
            areaDB.AddObj(obj);
        }

        /// <summary>
        /// 获取或创建指定位置的属性格子数据库。
        /// </summary>
        /// <param name="propPos">属性格子的逻辑坐标</param>
        /// <returns>对应位置的属性格子数据库</returns>
        public SLGPropertyGridDB GetOrCreatePropertyGridDB(Vector2Int propPos)
        {
            return propDB.GetOrCreatePropertyGridDB(propPos);
        }

        /// <summary>
        /// 查找指定位置的属性格子数据库。
        /// </summary>
        /// <param name="propPos">属性格子的逻辑坐标</param>
        /// <returns>找到则返回对应数据库，否则返回null</returns>
        public SLGPropertyGridDB FindPropertyGridDB(Vector2Int propPos)
        {
            return propDB.FindProperty(propPos);
        }
    }
}

