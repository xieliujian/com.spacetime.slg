// SLG 场景线段着色公共头：UV 镜像等小函数，供 SLGSceneLine 相关 Shader 包含。

#ifndef _SLG_SCENE_LINE_COMMON_H_
#define _SLG_SCENE_LINE_COMMON_H_

// 按 uvMirror（0/1）在原始 UV 与 1-UV 间插值，用于左右或上下翻转采样。
float2 CalcUVMirror(float2 srcUV, float uvMirror)
{
    return lerp(srcUV, 1 - srcUV, uvMirror);
}

#endif