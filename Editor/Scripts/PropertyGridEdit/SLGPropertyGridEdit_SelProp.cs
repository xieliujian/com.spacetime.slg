using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ST.SLG
{
    /// <summary>
    /// SLG 属性网格编辑器：选择类属性层与场景数据填充（分部类）。
    /// </summary>
    public partial class SLGPropertyGridEdit
    {
        static void CreateSelPropertyLayer(GameObject rootGo, int layerIndex)
        {
            SLGEditUtils.CreateSLGLayer(rootGo, SLGEditDefine.SLG_EDIT_LAYER_SEL_PROPERTY_NODE_NAME, layerIndex);
        }

        static void FillSceneSelPropertyInfo(SLGSceneDB sceneDB, GameObject obj, Vector2Int propPos)
        {
            var propertyDB = sceneDB.GetOrCreatePropertyGridDB(propPos);
            if (propertyDB == null)
                return;

            if (obj.name == SLGEditDefine.s_SLGSelPropPrefabArray[0])
            {
                propertyDB.selType = (int)SLGSelGridProp.CanSel;
            }
            else if (obj.name == SLGEditDefine.s_SLGSelPropPrefabArray[1])
            {
                propertyDB.selType = (int)SLGSelGridProp.UnSel;
            }
            else if (obj.name == SLGEditDefine.s_SLGSelPropPrefabArray[2])
            {
                propertyDB.selType = (int)SLGSelGridProp.Dynamic;
            }
        }
    }
}
