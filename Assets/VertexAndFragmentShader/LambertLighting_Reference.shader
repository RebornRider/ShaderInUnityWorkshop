Shader "Unlit/LambertLightingReference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			// indicate that our pass is the "base" pass in forward rendering pipeline. 
			// It gets ambient and main directional light data set up
			// light direction in _WorldSpaceLightPos0 and color in _LightColor0
			Tags {"LightMode"="ForwardBase"}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc" // for _LightColor0
			
			struct vertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;			
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;	
				float4 lightColor : COLOR;
			};

			vertexOutput vert (vertexInput v)
			{
				vertexOutput o;

				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.uv;

				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);

				// dot product between normal and light direction for standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));

				// factor in the light color
				o.lightColor = nl * _LightColor0;

				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (vertexOutput i) : SV_Target
			{
				// sample texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// multiply by lighting
				col *= i.lightColor;
				return col;
			}
			ENDCG
		}
	}
}
