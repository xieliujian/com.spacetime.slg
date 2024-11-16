using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class SLGSceneDB : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGSceneResDB resDB = new SLGSceneResDB();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGScenePropDB propDB = new SLGScenePropDB();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public SLGAreaSetDB areaSetDB = new SLGAreaSetDB();

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            areaSetDB.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="resName"></param>
        /// <param name="yAxisOffset"></param>
        public void FillAreaInfoLayerDB(int layerID, string resPath, int renderQueue, bool isZWriteOn, 
            SLGDefine.SLGInfoLayerType infoLayerType, SLGDefine.SLGAreaGridPropertyLayerType areaPropertyLayerType,
            Vector2Int propertyTexSeq)
        {
            areaSetDB.FillAreaInfoLayerDB(layerID, resPath, resDB, renderQueue, isZWriteOn, infoLayerType,
                areaPropertyLayerType, propertyTexSeq);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
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
        /// 
        /// </summary>
        /// <param name="dynamicMapIndex"></param>
        /// <param name="obj"></param>
        /// <param name="areaDB"></param>
        /// <param name="prefabPath"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
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
        /// 
        /// </summary>
        /// <param name="propPos"></param>
        /// <returns></returns>
        public SLGPropertyGridDB GetOrCreatePropertyGridDB(Vector2Int propPos)
        {
            return propDB.GetOrCreatePropertyGridDB(propPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propPos"></param>
        /// <returns></returns>
        public SLGPropertyGridDB FindPropertyGridDB(Vector2Int propPos)
        {
            return propDB.FindProperty(propPos);
        }
    }
}

