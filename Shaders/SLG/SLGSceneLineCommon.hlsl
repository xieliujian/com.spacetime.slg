

#ifndef _SLG_SCENE_LINE_COMMON_H_
#define _SLG_SCENE_LINE_COMMON_H_

// .
float2 CalcUVMirror(float2 srcUV, float uvMirror)
{
    return lerp(srcUV, 1 - srcUV, uvMirror);
}

#endif