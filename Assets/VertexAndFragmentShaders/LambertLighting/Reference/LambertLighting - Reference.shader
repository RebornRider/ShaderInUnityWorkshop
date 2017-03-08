Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			// indicate that our pass is the "base" pass in forward rendering pipeline. 
			// It gets ambient and main directional light data set up
			// light direction in _WorldSpaceLightPos0 (acessed by UnityWorldSpaceLightDir)
			// and color in _LightColor0
			Tags {"LightMode"="ForwardBase"}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// for UnityObjectToWorldNormal and UnityWorldSpaceLightDir
			#include "UnityCG.cginc" 
			// for _LightColor0
			#include "UnityLightingCommon.cginc"
			
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
				// dot product between normal and light direction for lambert lighting
				half nl = max(0, dot(worldNormal, UnityWorldSpaceLightDir(v.vertex)));
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