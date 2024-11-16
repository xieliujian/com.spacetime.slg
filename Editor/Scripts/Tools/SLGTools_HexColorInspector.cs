using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ST.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(SLGTools_HexColor))]
    public class SLGTools_HexColorInspector : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            var script = (SLGTools_HexColor)target;
            if (script == null)
                return;

            base.OnInspectorGUI();

            if (GUILayout.Button("打印颜色信息"))
            {
                script.DumpColorInfo();
            }
        }
    }
}

