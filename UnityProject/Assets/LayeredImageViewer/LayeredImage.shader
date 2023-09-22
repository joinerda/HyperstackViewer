// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/LayeredImage"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Cutoff", float) = 0.2
        _Alpha("Alpha", float) = 0.4
        _NormalCutoff("NormalCutoff", float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            float _NormalCutoff;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = v.vertex;
                o.worldPos =
                    mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half camera_normal = abs(dot(normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz), i.normal));
                clip (camera_normal - _NormalCutoff);
                fixed4 col = tex2D(_MainTex, i.uv);
                col.g = col.r;
                col.b = col.r;
                half value_squared = dot(col.rgb, col.rgb);
                clip(value_squared-_Cutoff*_Cutoff*0.5);
          
                col.a *= clamp(_Alpha*value_squared,0,1);
                //col.a *= _Alpha;

                return col;
            }
            ENDCG
        }
    }
}
