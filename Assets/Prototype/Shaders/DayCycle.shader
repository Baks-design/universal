Shader "Skyvbox/DayCycle"
{
    Properties
    {
        _Texture1("Texture1", 2D) = "white" {}
        _Texture2("Texture2", 2D) = "white" {}
        _Blend("Blend", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Texture1;
            sampler2D _Texture2;
            float _Blend;

            v2f vert (appdata v)
            {
                v2f o;
                o.texcoord = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float2 ToRadialCoords(float3 coords)
            {
                float3 normalizeCoords = normalize(coords);
                float latitude = acos(normalizeCoords.y);
                float longitude = atan2(normalizeCoords.z, normalizeCoords.x);
                const float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1 / UNITY_PI);
                return float2(0.5, 1) - sphereCoords;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 to = ToRadialCoords(i.texcoord);
                fixed4 tex1 = tex2D(_Texture1, to);
                fixed4 tex2 = tex2D(_Texture2, to);
                return lerp(tex1, tex2, _Blend);
            }
            ENDCG
        }
    }
}
