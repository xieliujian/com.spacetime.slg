using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        /// <summary>
        /// 
        /// </summary>
        const string SLG_RES_LV_PROPERTY_PREFAB_PREFIX = "slg_reslvproperty_";

        /// <summary>
        /// 
        /// </summary>
        const string SLG_EDITLAYER_RES_LV_PROPERTY_NODE_NAME = "ResLvPropertyLayer";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootGo"></param>
        /// <param name="layerIndex"></param>
        static void CreateResLvPropertyLayer(GameObject rootGo, int layerIndex)
        {
            SLGEditUtils.CreateSLGLayer(rootGo, SLG_EDITLAYER_RES_LV_PROPERTY_NODE_NAME, layerIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        /// <param name="obj"></param>
        /// <param name="propPos"></param>
        static void FillSceneResLvPropertyInfo(SLGSceneDB sceneDB, GameObject obj, Vector2Int propPos)
        {
            var propertyDB = sceneDB.GetOrCreatePropertyGridDB(propPos);
            if (propertyDB == null)
                return;

            var objName = obj.name;
            if (!objName.Contains(SLG_RES_LV_PROPERTY_PREFAB_PREFIX))
                return;

            objName = objName.Replace(SLG_RES_LV_PROPERTY_PREFAB_PREFIX, "");

            try
            {
                var val = int.Parse(objName);
                propertyDB.resLvType = (byte)val;
            }
            catch
            {
                Debug.LogError($"[SceneStreamerSLG][FillSceneResLvPropertyInfo] [propPos]({propPos}) [objName]({obj.name})");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaSetDB"></param>
        /// <param name="layer"></param>
        static void FillAreaResLvStateLayer(SLGSceneDB sceneDB, SLGAreaSetDB areaSetDB, SLGAreaPropertyInfoLayerDB layer)
        {
            var areaDBList = areaSetDB.areaDBList;
            if (areaDBList == null)
                return;

            var propertyTexSeqWidth = layer.propertyTexSeqWidth;
            var propertyTexSeqHeight = layer.propertyTexSeqHeight;

            foreach (var areaDB in areaDBList)
            {
                if (areaDB == null)
                    continue;

                var gridSet = areaDB.gridSet;
                if (gridSet == null)
                    continue;

                var startPos = gridSet.startPos;
                var endPos = gridSet.endPos;

                SLGAreaPropertyInfoBlockDB block = new SLGAreaPropertyInfoBlockDB();
                layer.blockList.Add(block);

                for (int y = startPos.y; y < endPos.y; y++)
                {
                    for (int x = startPos.x; x < endPos.x; x++)
                    {
                        var logicPos = new Vector2Int(x, y);
                        SLGPropertyGridDB propGridDB = sceneDB.FindPropertyGridDB(logicPos);

                        Matrix4x4 matrix = SLGUtils.s_UnVisMatrix;
                        Vector4 uvScaleOffset = SLGUtils.s_DefaultUVScaleOffset;

                        if (propGridDB.resLvType > 0 && propGridDB.resLvType <= propertyTexSeqWidth * propertyTexSeqHeight)
                        {
                            var grid = gridSet.FindAreaGridDB(logicPos);
                            if (grid != null)
                            {
                                matrix = grid.matrix;

                                var seqFrameIndex = propGridDB.resLvType - 1;
                                uvScaleOffset = SLGUtils.CalcUVScaleOffset(seqFrameIndex, 
                                    propertyTexSeqWidth, propertyTexSeqHeight);
                            }
                        }

                        block.matrixList.Add(matrix);
                        block.uvScaleOffsetList.Add(uvScaleOffset);
                    }
                }
            }
        }
    }
}

