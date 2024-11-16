using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// The script execution order is after the camera refresh.
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
