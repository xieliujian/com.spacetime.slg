

#ifndef _SLG_SCENE_OBJ_COMMON_H_
#define _SLG_SCENE_OBJ_COMMON_H_

// .
float2 CalcUVMirror(float2 srcUV, float uvMirror)
{
    return lerp(srcUV, 1 - srcUV, uvMirror);
}

// .
float2 CalcSeqFrameUV(float2 srcUV, float seqFrameWidth, float seqFrameHeight, float seqFrameSpeed, out float2 seqUVCenterOff)
{    
    uint iSeqFrameWidth = (uint)seqFrameWidth;
    uint iSeqFrameHeight = (uint)seqFrameHeight;
    uint iSeqTotalNum = iSeqFrameWidth * iSeqFrameHeight;
    
    int curFrame = floor(_Time.y * iSeqTotalNum * seqFrameSpeed) % iSeqTotalNum;
    //curFrame = 0;
    
    float row = curFrame / iSeqFrameWidth;
    float column = (curFrame - row * iSeqFrameWidth);
    row = (iSeqFrameHeight - 1) - row;
    
    float2 uv = srcUV + float2(column, row);
    uv.x /= iSeqFrameWidth;
    uv.y /= iSeqFrameHeight;
    
    seqUVCenterOff = 0;
#if _SLG_SCENE_OBJ_UV_ROT
    seqUVCenterOff = float2(column, row) + float2(0.5, 0.5);
    seqUVCenterOff.x /= iSeqFrameWidth;
    seqUVCenterOff.y /= iSeqFrameHeight;
#endif

    return uv;
}

// 只能用于逻辑材质和游戏材质，编辑器编辑地图材质UV不是标准（0 ~ 1）不适用
float2 CalcUVRot(float2 srcUV, float4 uvScaleOffset, float2 seqUVCenterOff, float rotAngle)
{
    // 中心点移动到(0, 0)然后旋转
    float2 uvOffset = uvScaleOffset.zw + uvScaleOffset.xy * 0.5;
#if _SLG_SCENE_OBJ_SEQ_FRAME_ANIM
    uvOffset = seqUVCenterOff;
#endif
    
    srcUV -= uvOffset;
    
    // rot
    float rad = radians(rotAngle);
    float cosValue = cos(rad);
    float sinValue = sin(rad);
    float2 newUV = float2(srcUV.x * cosValue - srcUV.y * sinValue, srcUV.x * sinValue + srcUV.y * cosValue);
    
    // 移动到原来的位置
    newUV += uvOffset;
    
    return newUV;
}

// .
float2 CalcGlobalMaskTexUV(float3 positionWS, float mapSize, float rotAngle)
{
    float uvX = (positionWS.x / mapSize) + 0.5;
    float uvY = (positionWS.z / mapSize) + 0.5;
    float2 uv = float2(uvX, uvY);
    
    // rot
    float rad = radians(rotAngle);
    float cosValue = cos(rad);
    float sinValue = sin(rad);
    uv = float2(uv.x * cosValue - uv.y * sinValue, uv.x * sinValue + uv.y * cosValue);
        
    return uv;
}

// .
float2 CalcGlobalBlendTexUV(float3 positionWS, float mapSize, float4 uvScaleOffset, float rotAngle)
{
    float2 uv = CalcGlobalMaskTexUV(positionWS, mapSize, rotAngle);
    uv = uv * uvScaleOffset.xy + uvScaleOffset.zw;   
    
    return uv;
}



#endif