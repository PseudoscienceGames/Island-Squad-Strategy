Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2DArray) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.5

        UNITY_DECLARE_TEX2DARRAY(_MainTex);

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float4 color : Color;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float h = IN.color.r * 100;
            fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, h));
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
