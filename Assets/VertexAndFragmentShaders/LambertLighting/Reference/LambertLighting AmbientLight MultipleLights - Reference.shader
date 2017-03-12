Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight MultipleLights - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE // common code for all passes of all subshaders
		// for UnityObjectToWorldNormal and UnityWorldSpaceLightDir
		#include "UnityCG.cginc" 
		// for _LightColor0
		#include "UnityLightingCommon.cginc"

		struct vertexInput {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;			
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};
			
		sampler2D _MainTex;
		float4 _MainTex_ST;

	ENDCG

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
			#pragma fragment fragBasePass	

			struct vertexOutput {		
				float4 modelPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float3 normal : NORMAL;	
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;	
				o.modelPos = v.vertex;			
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}	

			fixed3 fragBasePass (vertexOutput i) : SV_Target
			{
				float3 worldNormal = normalize(i.normal);
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(worldNormal, UnityWorldSpaceLightDir(i.modelPos)));
				// factor in the light color and ambience
				fixed3 lightCol = saturate((NdotL * _LightColor0)  + ShadeSH9(half4(worldNormal,1)));

				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
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
			#pragma fragment fragAddPass

			#include "AutoLight.cginc" 

			#pragma multi_compile_fwdadd

			struct vertexOutput {		
				float3 worldPos : TEXCOORD0;
				// these three vectors will hold a 3x3 rotation matrix
				// that transforms from tangent to world space
				float2 uv : TEXCOORD1;	
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				LIGHTING_COORDS(2,3) 				
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;	
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				o.normal = UnityObjectToWorldNormal(v.normal);
				TRANSFER_VERTEX_TO_FRAGMENT(o); 
				return o;
			}
		
			fixed3 fragAddPass (vertexOutput i) : SV_Target
			{
				half3 vertexToLightSource = UnityWorldSpaceLightDir(i.worldPos);
				// linear falloff for point lights , no falloff for directional lights
				float distance = length(vertexToLightSource);	
				half attenuation = LIGHT_ATTENUATION(i);
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(normalize(i.normal), normalize(vertexToLightSource)));
				// we multiply with attenuation to create falloff
				// we can skip the saturate, since we do not add ambient in this pass
				fixed3 lightCol = _LightColor0.xyz * NdotL * attenuation;

				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}	
	}
}