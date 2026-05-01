using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ST.SLG
{
    /// <summary>
    /// <see cref="SLGSceneCamera"/> 的自定义 Inspector：非播放模式下提供 WASD/滚轮模拟按钮。
    /// </summary>
    [CustomEditor(typeof(SLGSceneCamera))]
    public class SLGSceneCameraInspector : Editor
    {
        /// <summary>
        /// 绘制运行提示、编辑器移动按钮并绘制默认序列化字段。
        /// </summary>
        public override void OnInspectorGUI()
        {
            var script = target as SLGSceneCamera;
            if (script == null)
                return;

            if (Application.isPlaying)
            {
                GUILayout.Label("_________________________注意_______________________");
                GUILayout.Label("       鼠标滚轮拉近拉远镜头，WADS平移摄像机            ");
                GUILayout.Label("____________________________________________________");
            }
            else
            {
                DrawWADS(script);
                GUILayout.Space(10);
                DrawMouseWheel(script);
            }

            base.OnInspectorGUI();

        }

        void DrawMouseWheel(SLGSceneCamera script)
        {
            bool isWheel = false;

            EditorGUILayout.BeginVertical();

            GUILayout.Label("滚轮移动");

            if (GUILayout.RepeatButton("滚轮向前"))
            {
                isWheel = true;
                script.SetInputScroll(SLGEditDefine.SLG_CAMERA_SCROLL_UNIT);
            }

            if (GUILayout.RepeatButton("滚轮向后"))
            {
                isWheel = true;
                script.SetInputScroll(-SLGEditDefine.SLG_CAMERA_SCROLL_UNIT);
            }

            if (!isWheel)
            {
                script.SetInputScroll(0f);
            }

            EditorGUILayout.EndVertical();
        }

        void DrawWADS(SLGSceneCamera script)
        {
            bool isHorizontal = false;
            bool isVertical = false;

            EditorGUILayout.BeginVertical();

            GUILayout.Label("摄像机移动");

            if (GUILayout.RepeatButton("W"))
            {
                script.SetInputVertical(SLGEditDefine.SLG_CAMERA_VERTICAL_UNIT);
                isVertical = true;
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.RepeatButton("A"))
            {
                script.SetInputHorizontal(-SLGEditDefine.SLG_CAMERA_HORIZONTAL_UNIT);
                isHorizontal = true;
            }

            if (GUILayout.RepeatButton("D"))
            {
                script.SetInputHorizontal(SLGEditDefine.SLG_CAMERA_HORIZONTAL_UNIT);
                isHorizontal = true;
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.RepeatButton("S"))
            {
                script.SetInputVertical(-SLGEditDefine.SLG_CAMERA_VERTICAL_UNIT);
                isVertical = true;
            }

            if (!isVertical)
            {
                script.SetInputVertical(0f);
            }

            if (!isHorizontal)
            {
                script.SetInputHorizontal(0f);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
