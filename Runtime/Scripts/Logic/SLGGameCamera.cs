using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 运行时摄像机控制：WASD 平移、滚轮缩放，固定俯仰角，高度限制。
    /// 参数默认值统一来自 <see cref="SLGDefine"/>，可在 Inspector 中覆盖。
    /// </summary>
    public class SLGGameCamera : MonoBehaviour
    {
        static readonly string s_Horizontal       = "Horizontal";
        static readonly string s_Vertical         = "Vertical";
        static readonly string s_MouseScrollWheel = "Mouse ScrollWheel";

        Camera m_Cam;

        float m_InputHorizontal;
        float m_InputVertical;
        float m_InputScroll;

        /// <summary>摄像机视野角（度）。</summary>
        [Header("摄像机 Fov")]
        public float camFov = SLGDefine.s_SLGGameCamera_Fov;

        /// <summary>摄像机固定欧拉角（世界空间）。</summary>
        [Header("摄像机固定欧拉角")]
        public Vector3 camEulerAngle = new Vector3(SLGDefine.s_SLGGameCamera_EulerX, SLGDefine.s_SLGGameCamera_EulerY, 0f);

        /// <summary>摄像机允许的最小世界高度。</summary>
        [Header("摄像机最小高度")]
        public float camMinHeight = SLGDefine.s_SLGGameCamera_MinHeight;

        /// <summary>摄像机允许的最大世界高度。</summary>
        [Header("摄像机最大高度")]
        public float camMaxHeight = SLGDefine.s_SLGGameCamera_MaxHeight;

        /// <summary>平移灵敏度。</summary>
        [Header("摄像机平移速度")]
        public float camMoveSpeed = SLGDefine.s_SLGGameCamera_MoveSpeed;

        /// <summary>滚轮缩放灵敏度。</summary>
        [Header("摄像机拉近拉远速度")]
        public float camWheelSpeed = SLGDefine.s_SLGGameCamera_WheelSpeed;

        void Start()
        {
            m_Cam = GetComponent<Camera>();
            if (m_Cam != null)
            {
                m_Cam.nearClipPlane = SLGDefine.s_SLGGameCamera_Near;
                m_Cam.farClipPlane  = SLGDefine.s_SLGGameCamera_Far;
            }
        }

        void LateUpdate()
        {
            RefreshInput();
            RefreshCamera();
        }

        void RefreshInput()
        {
            m_InputHorizontal = Input.GetAxis(s_Horizontal);
            m_InputVertical   = Input.GetAxis(s_Vertical);
            m_InputScroll     = Input.GetAxis(s_MouseScrollWheel);
        }

        void RefreshCamera()
        {
            if (m_Cam == null)
                return;

            m_Cam.fieldOfView = camFov;

            SLGUtils.CalcSLGCameraPos(transform, camMinHeight, camMaxHeight, camMoveSpeed, camWheelSpeed,
                m_InputHorizontal, m_InputVertical, m_InputScroll, camEulerAngle);
        }
    }
}
