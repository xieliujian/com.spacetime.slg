using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LR.SLG
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class SLGSceneCamera : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        static string s_Horizontal = "Horizontal";

        /// <summary>
        /// 
        /// </summary>
        static string s_Vertical = "Vertical";

        /// <summary>
        /// 
        /// </summary>
        static string s_MouseScrollWheel = "Mouse ScrollWheel";

        /// <summary>
        /// 
        /// </summary>
        static Vector3 s_CamInitPos = new Vector3(-0f, 0f, -0f);

        /// <summary>
        /// 摄像机
        /// </summary>
        Camera m_Cam;

        /// <summary>
        /// 
        /// </summary>
        float m_InputHorizontal;

        /// <summary>
        /// 
        /// </summary>
        float m_InputVertical;

        /// <summary>
        /// 
        /// </summary>
        float m_InputScroll;

        /// <summary>
        /// 摄像机Fov
        /// </summary>
        [Header("摄像机Fov")]
        public float camFov = 5f;

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机固定欧拉角")]
        public Vector3 camEulerAngle = new Vector3(40f, 45f, 0f);

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机最小高度")]
        public float camMinHeight = 200f;

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机最大高度")]
        public float camMaxHeight = 350f;

        /// <summary>
        /// Movement speed.
        /// </summary>
        [Header("摄像机平移速度")]
        public float camMoveSpeed = 50f;

        /// <summary>
        /// 
        /// </summary>
        [Header("摄像机拉近拉远速度")]
        public float camWheelSpeed = 1f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetInputHorizontal(float val)
        {
            m_InputHorizontal = val;
            RefreshCamera();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetInputVertical(float val)
        {
            m_InputVertical = val;
            RefreshCamera();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetInputScroll(float val)
        {
            m_InputScroll = val;
            RefreshCamera();
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            m_Cam = GetComponent<Camera>();
            transform.position = new Vector3(s_CamInitPos.x, camMaxHeight, s_CamInitPos.z);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void LateUpdate()
        {
            RefreshInput();
            RefreshCamera();
        }

        /// <summary>
        /// 
        /// </summary>
        void RefreshCamera()
        {
            if (m_Cam == null)
                return;

            m_Cam.fieldOfView = camFov;
            m_Cam.farClipPlane = 2500f;

            SLGUtils.CalcSLGCameraPos(transform, camMinHeight, camMaxHeight, camMoveSpeed, camWheelSpeed,
                        m_InputHorizontal, m_InputVertical, m_InputScroll, camEulerAngle);
        }

        /// <summary>
        /// 
        /// </summary>
        void RefreshInput()
        {
            if (Application.isPlaying)
            {
                m_InputHorizontal = Input.GetAxis(s_Horizontal);
                m_InputVertical = Input.GetAxis(s_Vertical);
                m_InputScroll = Input.GetAxis(s_MouseScrollWheel);
            }
        }
    }
}

