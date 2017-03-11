Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/Blending/Basic Alpha Blend With Luminance - Reference"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[Toggle]
		_Good("Is Good?", Int) = 1
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
			int _Good;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float luminance = float(0.3 * color.r + 0.59 * color.g + 0.11 * color.b); 
				float average = (color.r + color.g + color.b) / 3;
				color = lerp(float4(average, average, average, color.a), float4(luminance, luminance, luminance, color.a), _Good);
				return color;
			}
			ENDCG
		}
	}
}
