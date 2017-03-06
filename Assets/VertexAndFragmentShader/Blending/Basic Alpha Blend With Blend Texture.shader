Shader "Shaders101/Basic Alpha Blend With Blend Texture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BlendTex("Blend Text", 2D) = "white" {}
		_BlendAmount("Blend Amount", Range(0, 1)) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _BlendTex;
			float _BlendAmount;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = (1 - _BlendAmount) * tex2D(_MainTex, i.uv) + _BlendAmount * tex2D(_BlendTex, i.uv);
				return color;
			}
			ENDCG
		}
	}
}
