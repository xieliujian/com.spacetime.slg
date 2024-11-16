using System.Collections;
using System.Collections.Generic;
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
        static string[] s_SLGSelPropPrefabArray =
        {
            "slg_selproperty_sel",
            "slg_selproperty_unsel",
            "slg_selproperty_dynamic"
        };

        /// <summary>
        /// 
        /// </summary>
        const string SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME = "SelPropertyLayer";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootGo"></param>
        /// <param name="layerIndex"></param>
        static void CreateSelPropertyLayer(GameObject rootGo, int layerIndex)
        {
            SLGEditUtils.CreateSLGLayer(rootGo, SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME, layerIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneDB"></param>
        /// <param name="obj"></param>
        /// <param name="propPos"></param>
        static void FillSceneSelPropertyInfo(SLGSceneDB sceneDB, GameObject obj, Vector2Int propPos)
        {
            var propertyDB = sceneDB.GetOrCreatePropertyGridDB(propPos);
            if (propertyDB == null)
                return;

            if (obj.name == s_SLGSelPropPrefabArray[0])
            {
                propertyDB.selType = (int)SLGSelGridProp.CanSel;
            }
            else if (obj.name == s_SLGSelPropPrefabArray[1])
            {
                propertyDB.selType = (int)SLGSelGridProp.UnSel;
            }
            else if (obj.name == s_SLGSelPropPrefabArray[2])
            {
                propertyDB.selType = (int)SLGSelGridProp.Dynamic;
            }
        }
    }
}

