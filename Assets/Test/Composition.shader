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
        float3 bg = tex2D(_BGTexture, uv).rgb;
        float3 fg = tex2D(_SourceTexture, uv).rgb;
        float mask = tex2D(_MaskTexture, uv).r;

        // Screen blend mode
        float3 bl = 1 - (1 - fg) * (1 - bg);

        // Interpolation
        float3 rgb = bg;
        rgb = lerp(rgb, bl, saturate((mask - 0.2) / 0.4));
        rgb = lerp(rgb, fg, saturate((mask - 0.6) / 0.4));
        return float4(rgb , 1);
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
