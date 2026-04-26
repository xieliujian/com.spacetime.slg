using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 地图块实例数据：引用资源 ID 及每实例的变换矩阵与 UV 缩放偏移列表。
    /// </summary>
    [System.Serializable]
    public class SLGAreaMapBlockDB
    {
        /// <summary>
        /// 指向 <see cref="SLGSceneResDB.resDBList"/> 的资源下标。
        /// </summary>
        [SerializeField]
        public int resID;

        /// <summary>
        /// 每个放置实例的模型矩阵列表。
        /// </summary>
        [SerializeField]
        public List<Matrix4x4> matrixList = new List<Matrix4x4>();

        /// <summary>
        /// 与矩阵一一对应的 UV 缩放偏移（xy 缩放，zw 偏移）。
        /// </summary>
        [SerializeField]
        public List<Vector4> uvScaleOffsetList = new List<Vector4>();
    }
}
