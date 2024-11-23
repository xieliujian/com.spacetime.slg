

#ifndef _SLG_SCENE_LINE_INCLUDE_H_
#define _SLG_SCENE_LINE_INCLUDE_H_

float _SLGShaderFlag;

sampler2D _MainTex;
float4 _MainTex_ST;

sampler2D _EnemyTex;
float4 _EnemyTex_ST;

float _UVMirror;
float _FlowSpeed;

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_DEFINE_INSTANCED_PROP(float4, _UVScaleOffset)
UNITY_DEFINE_INSTANCED_PROP(float, _Enemy)
UNITY_INSTANCING_BUFFER_END(Props)

#endif