Shader "Hidden/SelfieBarracuda/Compositor"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _SourceTexture;
    sampler2D _MaskTexture;
    sampler2D _BGTexture;

    void Vertex(float4 position : POSITION,
                float2 uv : TEXCOORD0,
                out float4 outPosition : SV_Position,
                out float2 outUV : TEXCOORD0)
    {
        outPosition = UnityObjectToClipPos(position);
        outUV = uv;
    }

    float4 CompositeForeground(float2 uv, float3 bg)
    {
        float3 fg = tex2D(_SourceTexture, uv).rgb;
        float mask = tex2D(_MaskTexture, uv).r;

        // Overlay blend mode
        float3 bl = fg < 0.5 ? 2 * bg * fg : 1 - 2 * (1 - bg) * (1 - fg);

        // Interpolation
        float3 rgb = bg;
        rgb = lerp(rgb, bl, saturate((mask - 0.1) / 0.4));
        rgb = lerp(rgb, fg, saturate((mask - 0.5) / 0.4));
        return float4(rgb , 1);
    }

    float4 FragmentThru(float4 position : SV_Position,
                        float2 uv : TEXCOORD0) : SV_Target
    {
        return tex2D(_SourceTexture, uv);
    }

    float4 FragmentMask(float4 position : SV_Position,
                        float2 uv : TEXCOORD0) : SV_Target
    {
        float4 color = tex2D(_SourceTexture, uv);
        float mask = tex2D(_MaskTexture, uv).r;
        return lerp(color, float4(1, 0, 0, 1), mask * 0.8);
    }

    float4 FragmentStatic(float4 position : SV_Position,
                          float2 uv : TEXCOORD0) : SV_Target
    {
        return CompositeForeground(uv, tex2D(_BGTexture, uv).rgb);
    }

    float4 FragmentDynamic(float4 position : SV_Position,
                           float2 uv : TEXCOORD0) : SV_Target
    {
        // Polar coodinates
        float2 p = (uv - 0.5) * float2(_ScreenParams.x / _ScreenParams.y, 1);
        p = float2(atan2(p.x, p.y), length(p));

        // Wavy animation
        float hue = p.x * 2.0 / UNITY_PI - _Time.y * 0.5;
        hue += sin(p.y * 5 - _Time.y * 3.3) * 0.2;

        // Hue to RGB
        float h = frac(hue) * 6 - 2;
        float3 rgb = saturate(float3(abs(h - 1) - 1, 2 - abs(h), 2 - abs(h - 2)));

        return CompositeForeground(uv, rgb);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentThru
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentMask
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentStatic
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentDynamic
            ENDCG
        }
    }
}
