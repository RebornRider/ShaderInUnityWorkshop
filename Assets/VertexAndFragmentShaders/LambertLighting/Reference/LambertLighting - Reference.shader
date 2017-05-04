Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType"="Opaque" 
		}

		Pass
		{
			// indicate that our pass is the "base" pass in forward rendering pipeline. 
			// It handles ambient light, lightmaps, main directional light and not important (vertex/SH) lights
			// light direction in UnityWorldSpaceLightDir and color in _LightColor0
			Tags 
			{
				"LightMode"="ForwardBase"
			}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			// compiles all variants needed by ForwardBase (forward rendering base) pass type.
			// The variants deal with different lightmap types and main directional light having shadows on or off
			// realtime GI being on or off etc.
			#pragma multi_compile_fwdbase			

			// for UnityObjectToWorldNormal and UnityWorldSpaceLightDir
			#include "UnityCG.cginc" 
			// for _LightColor0
			#include "UnityLightingCommon.cginc"
			
			struct vertexInput 
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;			
				float3 normal : NORMAL;
			};

			struct vertexOutput 
			{
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
				half nl = saturate(dot(worldNormal, UnityWorldSpaceLightDir(v.vertex)));
				// factor in the light color
				o.lightColor = nl * _LightColor0;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (vertexOutput i) : SV_Target
			{
				// sample texture and multiply by 
				fixed4 col = tex2D(_MainTex, i.uv);
				// multiply by lighting
				col *= i.lightColor;
				return col;
			}
			ENDCG
		}
	}
}