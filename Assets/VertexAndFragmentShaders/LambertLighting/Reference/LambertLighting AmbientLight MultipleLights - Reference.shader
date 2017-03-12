Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight MultipleLights - Reference"
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
			// It gets ambient and main directional light data set up; 
			// light direction in _WorldSpaceLightPos0 and color in _LightColor0
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
				fixed3 lightCol : COLOR;
			};

			vertexOutput vert (vertexInput v)
			{
				vertexOutput o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.uv;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(worldNormal, UnityWorldSpaceLightDir(v.vertex)));
				// factor in the light color and ambience
				o.lightCol = saturate((NdotL * _LightColor0) + ShadeSH9(half4(worldNormal,1)));
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed3 frag (vertexOutput i) : SV_Target
			{
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * i.lightCol;
			}
			ENDCG
		}

		//the second pass for additional lights
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" } 
			// Set the Blend Mode to One One - additive blending
			Blend One One 
			// no need to write to z buffer twice
			ZWrite Off
			
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
				fixed3 lightCol : COLOR;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;					
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				half3 vertexToLightSource = WorldSpaceLightDir(v.vertex);
				// linear falloff for point lights , no falloff for directional lights
				float distance = length(vertexToLightSource);
				fixed attenuation = lerp(1.0,1/(distance*distance),_WorldSpaceLightPos0.w);
				//attenuation = 1/(distance*distance);
				//attenuation = 1/distance;	
				fixed NdotL = saturate(dot(UnityObjectToWorldNormal(v.normal), normalize(vertexToLightSource)));				
				// we multiply with attenuation to create falloff
				// we can skip the saturate, since we do not add ambient in this pass
				o.lightCol = _LightColor0.xyz * NdotL * attenuation;;				
				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed3 frag(vertexOutput i) : SV_Target
			{
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * i.lightCol;
			}
			
			ENDCG
		}	
	}
}