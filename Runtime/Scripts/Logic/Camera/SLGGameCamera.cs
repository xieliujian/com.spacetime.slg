using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG 运行时摄像机控制：WASD 平移、滚轮缩放、鼠标右键拖拽，固定俯仰角，高度限制。
    /// 参数默认值统一来自 <see cref="SLGDefine"/>，可在 Inspector 中覆盖。
    /// </summary>
    public class SLGGameCamera : MonoBehaviour
    {
        Camera m_Cam;

        float m_InputHorizontal;
        float m_InputVertical;
        float m_InputScroll;

        Vector3 m_DragLastMousePos;
        bool    m_IsDragging;

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

        /// <summary>WASD 平移灵敏度。</summary>
        [Header("摄像机平移速度")]
        public float camMoveSpeed = SLGDefine.s_SLGGameCamera_MoveSpeed;

        /// <summary>滚轮缩放灵敏度。</summary>
        [Header("摄像机拉近拉远速度")]
        public float camWheelSpeed = SLGDefine.s_SLGGameCamera_WheelSpeed;

        /// <summary>鼠标拖拽平移灵敏度。</summary>
        [Header("摄像机拖拽速度")]
        public float camDragSpeed = SLGDefine.s_SLGGameCamera_DragSpeed;

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
            RefreshDrag();
            RefreshCamera();
        }

        void RefreshInput()
        {
            m_InputHorizontal = Input.GetAxis(SLGDefine.s_SLGCamera_Input_Horizontal);
            m_InputVertical   = Input.GetAxis(SLGDefine.s_SLGCamera_Input_Vertical);
            m_InputScroll     = Input.GetAxis(SLGDefine.s_SLGCamera_Input_MouseScrollWheel);
        }

        void RefreshDrag()
        {
            int btn = SLGDefine.s_SLGGameCamera_DragMouseButton;

            if (Input.GetMouseButtonDown(btn))
            {
                m_IsDragging      = true;
                m_DragLastMousePos = Input.mousePosition;
                return;
            }

            if (Input.GetMouseButtonUp(btn))
            {
                m_IsDragging = false;
                return;
            }

            if (!m_IsDragging || !Input.GetMouseButton(btn))
                return;

            Vector3 delta      = Input.mousePosition - m_DragLastMousePos;
            m_DragLastMousePos = Input.mousePosition;

            if (delta.sqrMagnitude < 0.001f)
                return;

            // 沿摄像机水平右方向和前方向（投影到 XZ 平面）移动
            var camRight   = Vector3.Normalize(new Vector3(transform.right.x,   0f, transform.right.z));
            var camForward = Vector3.Normalize(new Vector3(transform.forward.x, 0f, transform.forward.z));

            float speed = camDragSpeed * Time.deltaTime * 100f;
            transform.position -= camRight   * delta.x * speed;
            transform.position -= camForward * delta.y * speed;
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
