using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 单个 SLG 渲染资源配置：预制路径、渲染队列、深度写入及运行时材质路径。
    /// </summary>
    [System.Serializable]
    public class SLGResDB
    {
        /// <summary>
        /// 资源预制在工程中的路径（由 <see cref="SLGWarp"/> 加载）。
        /// </summary>
        [SerializeField]
        public string resPath;

        /// <summary>
        /// 材质渲染队列值。
        /// </summary>
        [SerializeField]
        public int renderQueue;

        /// <summary>
        /// 是否写入深度缓冲。
        /// </summary>
        [SerializeField]
        public bool zWriteOn;

        /// <summary>
        /// 材质资源路径（编辑器或运行时解析用，非序列化）。
        /// </summary>
        [NonSerialized]
        public string matPath;
    }
}
