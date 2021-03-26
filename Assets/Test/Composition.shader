Shader "Hidden/MeetBarracuda/Composition"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _SourceTexture;
    sampler2D _MaskTexture;
    sampler2D _BGTexture;

    float4 Fragment(float4 position : SV_Position,
                    float2 uv : TEXCOORD0) : SV_Target
    {
        float3 src = tex2D(_SourceTexture, uv).rgb;
        float mask = tex2D(_MaskTexture, uv).r;
        float3 bg = tex2D(_BGTexture, uv).rgb;

        mask = smoothstep(0.2, 0.8, mask);

        return float4(lerp(bg, src, mask) , 1);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment Fragment
            ENDCG
        }
    }
}
