

#ifndef _SLG_SCENE_LINE_FORWARD_PASS_H_
#define _SLG_SCENE_LINE_FORWARD_PASS_H_

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
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// .
void InitializeInputData(Varyings input, out InputData inputData)
{
    inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
}

// .
float2 CalcUV(float2 srcUV)
{        
    float2 newUV = CalcUVMirror(srcUV, _UVMirror);
    float4 uvScaleOffset = (float4) UNITY_ACCESS_INSTANCED_PROP(Props, _UVScaleOffset);
    newUV = newUV * uvScaleOffset.xy + uvScaleOffset.zw;
    
    float flowRevert = lerp(1, -1, _UVMirror);
    newUV += float2(flowRevert * _FlowSpeed * _Time.y, 0);
    
    return newUV;
}

// .
float4 CalcTex(float2 uv)
{
    float4 mainTexColor = tex2D(_MainTex, uv);
    float4 enemyTexColor = tex2D(_EnemyTex, uv);
    
    float isEnemy = (float) UNITY_ACCESS_INSTANCED_PROP(Props, _Enemy);
    float4 texColor = lerp(mainTexColor, enemyTexColor, isEnemy);
    
    return texColor;
}

// .
float4 CalcFinalColor(Varyings input)
{
    float4 texColor = CalcTex(input.uv);
    float4 baseColor = (float4) UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
    float3 color = texColor.rgb * baseColor.rgb;
    float alpha = texColor.a * baseColor.a;
    float4 finalColor = float4(color, alpha);
    
    return finalColor;
}

// .
Varyings SLGSceneLineVert(Attributes v)
{
    Varyings output;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, output);

    output.vertex = TransformObjectToHClip(v.vertex.xyz);
    output.uv = TRANSFORM_TEX(v.uv, _MainTex);
    output.uv = CalcUV(output.uv);
    
    float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
    output.positionWS = positionWS;
    
    return output;
}

// .
float4 SLGSceneLineFrag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

    float4 finalColor = CalcFinalColor(input);
    
    InputData inputData;
    InitializeInputData(input, inputData);
    
    finalColor = UniversalFragment_SLGScene(inputData, finalColor);
    
    return finalColor;
}


#endif