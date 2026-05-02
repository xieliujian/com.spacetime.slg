using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 场景预览用相机控制：编辑模式下可用 Inspector 按钮模拟输入，运行时用 WASD 与滚轮驱动 <see cref="SLGUtils.CalcSLGCameraPos"/>。
    /// 参数默认值统一来自 <see cref="SLGDefine"/>，可在 Inspector 中覆盖。
    /// </summary>
    [ExecuteInEditMode]
    public class SLGSceneCamera : MonoBehaviour
    {
        static readonly Vector3 s_CamInitPos = new Vector3(SLGDefine.s_SLGCamera_InitPosX, 0f, SLGDefine.s_SLGCamera_InitPosZ);

        Camera m_Cam;

        float m_InputHorizontal;
        float m_InputVertical;
        float m_InputScroll;

        /// <summary>相机视野角（度）。</summary>
        [Header("摄像机Fov")]
        public float camFov = SLGDefine.s_SLGSceneCamera_Fov;

        /// <summary>相机固定欧拉角（相对世界）。</summary>
        [Header("摄像机固定欧拉角")]
        public Vector3 camEulerAngle = new Vector3(SLGDefine.s_SLGSceneCamera_EulerX, SLGDefine.s_SLGSceneCamera_EulerY, 0f);

        /// <summary>相机允许的最小世界高度。</summary>
        [Header("摄像机最小高度")]
        public float camMinHeight = SLGDefine.s_SLGSceneCamera_MinHeight;

        /// <summary>相机允许的最大世界高度。</summary>
        [Header("摄像机最大高度")]
        public float camMaxHeight = SLGDefine.s_SLGSceneCamera_MaxHeight;

        /// <summary>平移灵敏度。</summary>
        [Header("摄像机平移速度")]
        public float camMoveSpeed = SLGDefine.s_SLGSceneCamera_MoveSpeed;

        /// <summary>滚轮缩放灵敏度。</summary>
        [Header("摄像机拉近拉远速度")]
        public float camWheelSpeed = SLGDefine.s_SLGSceneCamera_WheelSpeed;

        /// <summary>
        /// 由外部或编辑器注入水平轴输入（-1..1）。
        /// </summary>
        /// <param name="val">水平输入</param>
        public void SetInputHorizontal(float val)
        {
            m_InputHorizontal = val;
            RefreshCamera();
        }

        /// <summary>
        /// 由外部或编辑器注入垂直轴输入（-1..1）。
        /// </summary>
        /// <param name="val">垂直输入</param>
        public void SetInputVertical(float val)
        {
            m_InputVertical = val;
            RefreshCamera();
        }

        /// <summary>
        /// 由外部或编辑器注入滚轮增量。
        /// </summary>
        /// <param name="val">滚轮输入</param>
        public void SetInputScroll(float val)
        {
            m_InputScroll = val;
            RefreshCamera();
        }

        void Start()
        {
            m_Cam = GetComponent<Camera>();
            transform.position = new Vector3(s_CamInitPos.x, camMaxHeight, s_CamInitPos.z);
        }

        void LateUpdate()
        {
            RefreshInput();
            RefreshCamera();
        }

        void RefreshCamera()
        {
            if (m_Cam == null)
                return;

            m_Cam.fieldOfView  = camFov;
            m_Cam.farClipPlane = SLGDefine.s_SLGSceneCamera_Far;

            SLGUtils.CalcSLGCameraPos(transform, camMinHeight, camMaxHeight, camMoveSpeed, camWheelSpeed,
                m_InputHorizontal, m_InputVertical, m_InputScroll, camEulerAngle);
        }

        void RefreshInput()
        {
            if (Application.isPlaying)
            {
                m_InputHorizontal = Input.GetAxis(SLGDefine.s_SLGCamera_Input_Horizontal);
                m_InputVertical   = Input.GetAxis(SLGDefine.s_SLGCamera_Input_Vertical);
                m_InputScroll     = Input.GetAxis(SLGDefine.s_SLGCamera_Input_MouseScrollWheel);
            }
        }
    }
}
