Shader "Effects/EffectBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Brightness ("Brightness", Range(0, 2)) = 1
		_SamplingRange("Sampling Range", Range(0, 1)) = 0.1
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Brightness;
			float _SamplingRange;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				col += tex2D(_MainTex, i.uv + float2(_SamplingRange, _SamplingRange));
				col += tex2D(_MainTex, i.uv + float2(-_SamplingRange, _SamplingRange));
				col += tex2D(_MainTex, i.uv + float2(-_SamplingRange, -_SamplingRange));
				col += tex2D(_MainTex, i.uv + float2(_SamplingRange, -_SamplingRange));
				col /= 5.0;
				col *= _Brightness;
				return col;
			}
			ENDCG
		}
	}
}
