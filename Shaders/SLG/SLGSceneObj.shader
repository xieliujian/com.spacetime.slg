Shader "SpaceTime/Scene/SLG/SLGSceneObj"
{
    Properties
    {
        [Header(AlphaBlendParam)]
        [Space(5)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend", Float) = 1.0      // One
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend", Float) = 0.0      // Zero

        [Header(KeyWord)]
        [Space(5)]
        [Toggle(_SLG_SCENE_OBJ_CLOSE_SHADOW)]_CloseRealShadow("CloseRealShadow", float) = 0
        [Toggle(_SLG_SCENE_OBJ_SEQ_FRAME_ANIM)]_OpenSeqFrameAnim("OpenSeqFrameAnim", float) = 0
        [Toggle(_SLG_SCENE_OBJ_GLOBAL_MASKBLEND)]_GlobalMaskBlend("GlobalMaskBlend", float) = 0
        [Toggle(_SLG_SCENE_OBJ_UV_ROT)]_OpenUVRot("OpenUVRot", float) = 0

        [HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4   // LEqual
        [HideInInspector][Enum(On, 1, Off, 0)] _ZWrite("ZWrite", Float) = 1        // On
        [HideInInspector][Enum(On, 1, Off, 0)] _ZClip("ZClip", Float) = 1          // On
        
        [Header(BaseTex)]
        [Space(5)]
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        [HDR][MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_UVScaleOffset("UVScaleOffset", Vector) = (1, 1, 0, 0)
        [Enum(On, 1, Off, 0)] _UVMirror("UVMirror", Float) = 0
        _UVRotAngle("UVRotAngle", float) = 0

        [Header(SeqFrame)]
        [Space(5)]
        _SeqFrameSpeed("SeqFrameSpeed", float) = 1
        _SeqFrameWidth("SeqFrameWidth", float) = 2
        _SeqFrameHeight("SeqFrameHeight", float) = 2

        [Header(GlobalBlendTex)]
        [Space(5)]
        _GlobalBlendTex("GlobalBlendTex", 2D) = "black" {}
        [NoScaleOffset]_GlobalMaskTex ("GlobalMaskTex", 2D) = "black" {}
        _GlobalTexRotAngle("GlobalTexRotAngle", float) = 0
        _MapSize("MapSize", float) = 200


        [HideInInspector]_SLGShaderFlag("SLGShaderFlag", float) = 1
    }

    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
	    
        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            Blend [_SrcBlend][_DstBlend]
            ZClip [_ZClip]
            ZTest [_ZTest]
            ZWrite [_ZWrite]

            HLSLPROGRAM

            #pragma target 4.5
            #pragma multi_compile_instancing

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma shader_feature_local _SLG_SCENE_OBJ_CLOSE_SHADOW
            #pragma shader_feature_local _SLG_SCENE_OBJ_SEQ_FRAME_ANIM
            #pragma shader_feature_local _SLG_SCENE_OBJ_GLOBAL_MASKBLEND
            #pragma shader_feature_local _SLG_SCENE_OBJ_UV_ROT

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "SLGSceneObj/SLGSceneObjInclude.hlsl"
            #include "SLGSceneObj/SLGSceneObjCommon.hlsl"
            #include "SLGSceneObj/SLGSceneObjShadow.hlsl"
            #include "SLGSceneObj/SLGSceneObjLighting.hlsl"
            #include "SLGSceneObj/SLGSceneObjForwardPass.hlsl"

            #pragma vertex SLGSceneObjVert
            #pragma fragment SLGSceneObjFrag
			
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
