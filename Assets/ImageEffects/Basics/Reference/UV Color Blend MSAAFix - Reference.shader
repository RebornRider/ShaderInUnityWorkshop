Shader "ShadersInUnityWorkshop/Reference/ImageEffects/Basics/UV Color Blend MSAAFix - Reference"
{
	Properties
	{
		[HideInInspector]
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"PreviewType" = "Plane"
		}
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
				float4 vertex : SV_POSITION;	
				float2 uv : TEXCOORD0;		
				float2 uv1 : TEXCOORD1;		
			};
			
			float4 _MainTex_TexelSize;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = o.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - v.uv.y;
				#endif
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * float4(i.uv1, 0, 1);
			}
			ENDCG
		}
	}
}