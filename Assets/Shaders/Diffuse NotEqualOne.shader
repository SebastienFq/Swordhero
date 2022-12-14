Shader "Custom/Stencil/Diffuse NotEqualOne"
{
 
Properties
{
    [IntRange] _StencilID ("Stencil ID", Range(0, 15)) = 0
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
 
SubShader
{
    Tags 
    {  
        "RenderType"="Opaque" 
        "Queue"="Geometry" 
        "RenderPipeline" = "UniversalRenderPipeline" 
    }
    LOD 200
 
    Stencil
    {
        Ref [_StencilID]
        Comp notequal
        Pass keep
    }
 
    CGPROGRAM
    #pragma surface surf Lambert
 
    sampler2D _MainTex;
    fixed4 _Color;
 
    struct Input
    {
        float2 uv_MainTex;
    };
 
    void surf (Input IN, inout SurfaceOutput o)
    {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
   
    ENDCG
}
 
Fallback "VertexLit"
}