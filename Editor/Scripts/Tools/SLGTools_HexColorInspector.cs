using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ST.SLG
{
    /// <summary>
    /// <see cref="SLGTools_HexColor"/> 的自定义检视面板，在默认检视之外提供一键打印颜色信息。
    /// </summary>
    [CustomEditor(typeof(SLGTools_HexColor))]
    public class SLGTools_HexColorInspector : Editor
    {
        /// <summary>
        /// 绘制默认检视并在底部增加“打印颜色信息”按钮。
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
