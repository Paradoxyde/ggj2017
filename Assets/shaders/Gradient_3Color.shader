Shader "Custom/Gradient_3Color"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Top Color", Color) = (1,1,1,1)
        _Color2("Mid Color", Color) = (1,1,1,1)
        _Color3("Bot Color", Color) = (1,1,1,1)
        _Scale("Middle", Range(0, 1)) = 1
    }
        
    SubShader
    {
        Tags{ "Queue" = "Background"  "IgnoreProjector" = "True" }
        LOD 100

        ZWrite On
        Cull Off

        BindChannels
        {
            Bind "vertex", vertex
            Bind "color", color
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _Color2;
            fixed4 _Color3;
            fixed  _Scale;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed4 col : COLOR;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.col = step(_Scale, v.texcoord.y) * lerp(_Color2, _Color, v.texcoord.y) + step(v.texcoord.y, _Scale) * lerp(_Color3, _Color2, v.texcoord.y);
                return o;
            }


            float4 frag(v2f i) : COLOR
            {
                float4 c = i.col;
                c.a = 1;
                return c;
            }

            ENDCG
        }
    }
}
