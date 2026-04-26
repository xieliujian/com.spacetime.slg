using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// SLG场景管理器的MonoBehaviour包装，脚本执行顺序在摄像机刷新之后。
    /// </summary>
    public class SLGSceneMgrMono : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        void LateUpdate()
        {
            SLGSceneMgr.S.Update();
        }
    }
}
