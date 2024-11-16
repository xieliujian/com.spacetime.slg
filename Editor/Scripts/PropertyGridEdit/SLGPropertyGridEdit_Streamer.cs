using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        public static void StreamerExport(SLGSceneDB sceneDB)
        {
            FillPropertyLayerSceneDB(sceneDB);
            CalcAllPropertyGrid(sceneDB);
            FillAllAreaPropertyLayer(sceneDB);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void FillAllAreaPropertyLayer(SLGSceneDB sceneDB)
        {
            var areaSetDB = sceneDB.areaSetDB;
            if (areaSetDB == null)
                return;

            var infoLayerSet = areaSetDB.infoLayerSet;
            if (infoLayerSet == null)
                return;

            var propertyLayerList = infoLayerSet.propertyLayerList;
            if (propertyLayerList == null)
                return;

            foreach(var layer in propertyLayerList)
            {
                if (layer == null)
                    continue;

                var isResLvLayer = layer.IsAreaResLvStateLayer();

                if (isResLvLayer)
                {
                    FillAreaResLvStateLayer(sceneDB, areaSetDB, layer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        static void FillPropertyLayerSceneDB(SLGSceneDB sceneDB)
        {
            var gridObj = GetScenePropertyRootNode();
            if (gridObj == null)
                return;

            var isPrefab = SLGEditUtils.IsPrefabObject(gridObj.gameObject);
            if (isPrefab)
            {
                PrefabUtility.UnpackPrefabInstance(gridObj.gameObject,
                        PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.AutomatedAction);
            }

            var childCount = gridObj.transform.childCount;
            if (childCount <= 0)
                return;

            for (int i = 0; i < childCount; i++)
            {
                var child = gridObj.transform.GetChild(i);
                if (child == null)
                    continue;

                var childName = child.name;
                FillScenePropertyInfo(sceneDB, child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        /// <param name="rootNode"></param>
        static void FillScenePropertyInfo(SLGSceneDB sceneDB, Transform rootNode)
        {
            var objList = SLGEditUtils.CollectAllPrefabByRootNode(rootNode.gameObject);
            if (objList == null || objList.Count <= 0)
                return;

            foreach (var obj in objList)
            {
                if (obj == null)
                    continue;

                var pos = obj.transform.position;
                var propPos = SLGUtils.ConvertSLG3DPosToLogicPos(pos);

                if (rootNode.name == SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME)
                {
                    FillSceneSelPropertyInfo(sceneDB, obj, propPos);
                }
                else if (rootNode.name == SLG_EDITLAYER_RES_LV_PROPERTY_NODE_NAME)
                {
                    FillSceneResLvPropertyInfo(sceneDB, obj, propPos);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static void CalcAllPropertyGrid(SLGSceneDB sceneDB)
        {
            for (int j = 1; j <= SLGDefine.SLG_GRID_VERTICAL_NUM; j++)
            {
                for (int i = 1; i <= SLGDefine.SLG_GRID_HORIZONTAL_NUM; i++)
                {
                    Vector2Int propPos = new Vector2Int(i, j);

                    var propertyDB = sceneDB.GetOrCreatePropertyGridDB(propPos);
                    if (propertyDB == null)
                        continue;

                    propertyDB.Null();
                }
            }
        }
    }
}

