

#ifndef _SLG_SCENE_OBJ_INCLUDE_H_
#define _SLG_SCENE_OBJ_INCLUDE_H_

float _SLGShaderFlag;

sampler2D _MainTex;
float4 _MainTex_ST;

sampler2D _GlobalBlendTex;
float4 _GlobalBlendTex_ST;

sampler2D _GlobalMaskTex;
float4 _GlobalMaskTex_ST;

float _UVRotAngle;

float _UVMirror;
float _SeqFrameSpeed;
float _SeqFrameWidth;
float _SeqFrameHeight;

float _MapSize;
float _GlobalTexRotAngle;

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_DEFINE_INSTANCED_PROP(float4, _UVScaleOffset)
UNITY_INSTANCING_BUFFER_END(Props)

#endif


