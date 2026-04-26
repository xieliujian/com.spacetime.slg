using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ST.SLG
{
    /// <summary>
    /// 区域图层抽象基类，定义图层的基本接口。
    /// </summary>
    public abstract class SLGAreaLayer
    {
        /// <summary>
        /// 获取当前图层的索引 ID。
        /// </summary>
        /// <returns>图层 ID，若未设置则返回 -1。</returns>
        public abstract int GetLayerIndex();

        /// <summary>
        /// 执行当前帧的渲染逻辑。
        /// </summary>
        public virtual void Render()
        {

        }
    }
}

