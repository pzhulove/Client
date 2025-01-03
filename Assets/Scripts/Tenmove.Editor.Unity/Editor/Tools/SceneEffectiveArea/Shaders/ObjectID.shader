Shader "Hidden/Debug/ObjectID"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            Texture2D _MainTex;
            float4 _MainTex_ST;
            SamplerState my_point_clamp_sampler;
            SamplerState sampler_MainTex;

            fixed4 _Color;
            fixed4 _RendererColor;

            float4 _ObjectID;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.vertex = UnityPixelSnap(o.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color * _RendererColor;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = _MainTex.Sample(my_point_clamp_sampler, i.uv) * i.color;

                int alpha = ceil(col.a);

                float4 id = _ObjectID * alpha;

                return float4(id.r, id.g, id.b, alpha);
            }
            ENDCG
        }
    }
}
