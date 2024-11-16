Shader "SpaceTime/Scene/SLG/SLGSceneLine"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend", Float) = 1.0      // One
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend", Float) = 0.0      // Zero

        [Enum(On, 1, Off, 0)] _UVMirror("UVMirror", Float) = 0

        [HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4   // LEqual
        [HideInInspector][Enum(On, 1, Off, 0)] _ZWrite("ZWrite", Float) = 1        // On
        [HideInInspector][Enum(On, 1, Off, 0)] _ZClip("ZClip", Float) = 1          // On
        
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_EnemyTex("EnemyTex", 2D) = "white" {}
        [HDR][MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        [HideInInspector]_UVScaleOffset("UVScaleOffset", Vector) = (1, 1, 0, 0)
        [Enum(On, 1, Off, 0)] _Enemy("Enemy", float) = 0

        _FlowSpeed("FlowSpeed", float) = 1

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

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "SLGSceneLineInclude.hlsl"
            #include "SLGSceneLineCommon.hlsl"
            #include "SLGSceneLineLighting.hlsl"
            #include "SLGSceneLineForwardPass.hlsl"

            #pragma vertex SLGSceneLineVert
            #pragma fragment SLGSceneLineFrag
			
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
