

#ifndef _SLG_SCENE_OBJ_FORWARD_PASS_H_
#define _SLG_SCENE_OBJ_FORWARD_PASS_H_

// .
struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// .
struct Varyings
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float4 shadowCoord : TEXCOORD2;
#if _SLG_SCENE_OBJ_GLOBAL_MASKBLEND
    float4 globalMaskBlendMapUV : TEXCOORD3;
#else
    
#endif
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// .
void InitializeInputData(Varyings input, out InputData inputData)
{
    inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
    inputData.shadowCoord = input.shadowCoord;
    inputData.shadowMask = unity_ProbesOcclusion;
    inputData.normalWS = float3(0, 1, 0);
}

// .
float2 CalcUV(float2 srcUV)
{
    float2 newUV = CalcUVMirror(srcUV, _UVMirror);
    
    float4 uvScaleOffset = (float4)UNITY_ACCESS_INSTANCED_PROP(Props, _UVScaleOffset);
    newUV = newUV * uvScaleOffset.xy + uvScaleOffset.zw;
    
    float2 seqUVCenterOff = 0;
#if _SLG_SCENE_OBJ_SEQ_FRAME_ANIM         
    newUV = CalcSeqFrameUV(newUV, _SeqFrameWidth, _SeqFrameHeight, _SeqFrameSpeed, seqUVCenterOff); 
#endif
    
#if _SLG_SCENE_OBJ_UV_ROT
    newUV = CalcUVRot(newUV, uvScaleOffset, seqUVCenterOff, _UVRotAngle);
#endif
    
    return newUV;
}

// .
float4 CalcBaseColor(Varyings input)
{
    float4 texColor = tex2D(_MainTex, input.uv);
    float4 baseColor = (float4) UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
    float3 color = texColor.rgb * baseColor.rgb;
    float alpha = texColor.a * baseColor.a;
    
    return float4(color, alpha);
}

// .
float4 MixBaseColorGlobalBlendColor(Varyings input, float4 baseColor)
{
    float4 finalColor = baseColor;
    
#if _SLG_SCENE_OBJ_GLOBAL_MASKBLEND 
    
    float2 maskTexUV = input.globalMaskBlendMapUV.zw;
    float4 maskTexColor = tex2D(_GlobalMaskTex, maskTexUV);
    
    float2 blendTexUV = input.globalMaskBlendMapUV.xy;
    float4 blendTexColor = tex2D(_GlobalBlendTex, blendTexUV);
    
    finalColor = lerp(baseColor, blendTexColor, maskTexColor.a);
#endif
    
    return finalColor;
}

// .
float4 CalcFinalColor(Varyings input)
{    
    float4 baseColor = CalcBaseColor(input);    
    float4 finalColor = MixBaseColorGlobalBlendColor(input, baseColor);
    
    return finalColor;
}

// .
Varyings SLGSceneObjVert(Attributes v)
{
    Varyings output;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, output);

    output.vertex = TransformObjectToHClip(v.vertex.xyz);
    output.uv = TRANSFORM_TEX(v.uv, _MainTex);
    output.uv = CalcUV(output.uv);
    
    float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
    output.positionWS = positionWS;
    output.shadowCoord = TransformWorldToShadowCoord(positionWS);
    
#if _SLG_SCENE_OBJ_GLOBAL_MASKBLEND
    float2 globalBlendTexUV = CalcGlobalBlendTexUV(positionWS, _MapSize, _GlobalBlendTex_ST, _GlobalTexRotAngle);
    float2 globalMaskTexUV = CalcGlobalMaskTexUV(positionWS, _MapSize, _GlobalTexRotAngle);
    output.globalMaskBlendMapUV = float4(globalBlendTexUV, globalMaskTexUV);
#endif    

    return output;
}

// .
float4 SLGSceneObjFrag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    
    float4 finalColor = CalcFinalColor(input);
    
    InputData inputData;
    InitializeInputData(input, inputData);
    
    finalColor = UniversalFragment_SLGScene(inputData, finalColor);
    
    return finalColor;
}


#endif