using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 属性网格编辑器流式导出（分部类），将场景中的属性节点写入 <see cref="SLGSceneDB"/> 并同步区域属性层数据。
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        /// <summary>
        /// 流式导出：填充属性层场景数据、重算全图属性格，并将各区域属性层写入场景数据库。
        /// </summary>
        /// <param name="sceneDB">目标场景数据库。</param>
        public static void StreamerExport(SLGSceneDB sceneDB)
        {
            FillPropertyLayerSceneDB(sceneDB);
            CalcAllPropertyGrid(sceneDB);
            FillAllAreaPropertyLayer(sceneDB);
        }

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

                if (rootNode.name == SLGEditDefine.SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME)
                {
                    FillSceneSelPropertyInfo(sceneDB, obj, propPos);
                }
                else if (rootNode.name == SLGEditDefine.SLG_EDITLAYER_RES_LV_PROPERTY_NODE_NAME)
                {
                    FillSceneResLvPropertyInfo(sceneDB, obj, propPos);
                }
            }
        }

        static void CalcAllPropertyGrid(SLGSceneDB sceneDB)
        {
            for (int j = 1; j <= SLGDefine.s_SLG_Grid_VerticalNum; j++)
            {
                for (int i = 1; i <= SLGDefine.s_SLG_Grid_HorizontalNum; i++)
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
