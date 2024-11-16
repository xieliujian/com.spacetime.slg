

#ifndef _SLG_SCENE_OBJ_LIGHTING_H_
#define _SLG_SCENE_OBJ_LIGHTING_H_

#include "SLGSceneObjShadow.hlsl"

// Abstraction over Light shading data.
struct Light
{
    half3 direction;
    half3 color;
    half distanceAttenuation;
    half shadowAttenuation;
};

// .
Light GetMainLight()
{
    Light light;

    light.direction = _MainLightPosition.xyz;
    light.distanceAttenuation = unity_LightData.z;  // unity_LightData.z is 1 when not culled by the culling mask, otherwise 0.
    light.color = _MainLightColor.rgb;
    light.shadowAttenuation = 1.0;

    return light;
}

// .
Light GetMainLight(float4 shadowCoord, float3 positionWS, half4 shadowMask)
{
    Light light = GetMainLight(); 
    
#if !_SLG_SCENE_OBJ_CLOSE_SHADOW
    light.shadowAttenuation = MainLightShadow(shadowCoord, positionWS, shadowMask, _MainLightOcclusionProbes);
#endif
    
    return light;
}

// .
half3 CalcMainLightMixed(float3 finalColor, Light mainLight, InputData inputData)
{    
    half clamplightAttenuation = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
    half NdotL = saturate(dot(inputData.normalWS, mainLight.direction));
    clamplightAttenuation = clamp(clamplightAttenuation + _MainLightShaderParam.z, _MainLightShaderParam.x, 1);
    
    //half3 radiance = mainLight.color * (clamplightAttenuation * NdotL); // +(1 - clamplightAttenuation * NdotL) * _MainLightShadowColor.rgb;
    half3 radiance = clamplightAttenuation;
    
    return radiance;
}

// .
half4 UniversalFragment_SLGScene(InputData inputData, float4 finalColor)
{
    half3 mainLightMixed = 1;
    
    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);   
    mainLightMixed = CalcMainLightMixed(finalColor.rgb, mainLight, inputData);
    
    finalColor.rgb *= mainLightMixed;  
    return finalColor;
}

#endif