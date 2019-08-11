Shader "Hidden/LightCamera"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _Lights[20];
			uniform float4 _LightsColor[20];
			uniform int _LightCount = 0;
			uniform float _MaxLightDistance = 20;
			uniform float _Ambiant = 0;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

				float lightValue = 0;
				float2 pos = i.vertex.xy;
				float4 color;
				for (int i = 0; i < _LightCount; i++)
				{
					float2 lPos = _Lights[i].xy;
					float dist = length(pos - lPos);

					if (dist > _Lights[i].w)
						continue;

					dist = _Lights[i].w - dist;

					float l = dist > _MaxLightDistance ? 1 : dist / _MaxLightDistance;
					lightValue += l;
					color += l * _LightsColor[i];
				}
				color /= lightValue;

				if (lightValue > 1.5)
					lightValue = 1.5;

				lightValue = floor(lightValue * 3 + 0.5) / 3;
				if (lightValue < _Ambiant)
					lightValue = _Ambiant;

				return col * lightValue * color;
            }
            ENDCG
        }
    }
}
