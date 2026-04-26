using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// <see cref="SLGSceneMgr"/> 的 MonoBehaviour 包装：在 LateUpdate 中驱动场景更新，脚本执行顺序应晚于相机。
    /// </summary>
    public class SLGSceneMgrMono : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }

        void LateUpdate()
        {
            SLGSceneMgr.S.Update();
        }
    }
}
