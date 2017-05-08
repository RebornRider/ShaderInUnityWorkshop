Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight MultipleLights - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	// common code for all passes of all subshaders
	CGINCLUDE 
		#include "UnityCG.cginc" 
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
			Tags {"LightMode"="ForwardBase"}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragBasePass	
			
			#pragma multi_compile_fwdbase			

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
				half NdotL = saturate(dot(worldNormal, UnityWorldSpaceLightDir(i.modelPos)));
				fixed3 lightCol = saturate((NdotL * _LightColor0)  + ShadeSH9(half4(worldNormal,1)));
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}

		Pass
		{
			// indicate that our pass is the "additive" pass in forward rendering pipeline. 
			// It handles any additive per-pixel lights; one invocation per light illuminating this object
			Tags { "LightMode" = "ForwardAdd" } 
			// Set the Blend Mode to One One - additive blending with base pass
			Blend One One 
			// no need to write to z buffer twice
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAddPass

			#include "AutoLight.cginc" 

			// compiles variants for ForwardAdd (forward rendering additive) pass type. 
			// This compiles variants to handle directional, spot or point light types, and their variants with cookie textures.
			#pragma multi_compile_fwdadd

			struct vertexOutput {		
				float3 worldPos : TEXCOORD0;
				float2 uv : TEXCOORD1;	
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				// lighting UVs in TEXCOORD2 & TEXCOORD3
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
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(normalize(i.normal), normalize(UnityWorldSpaceLightDir(i.worldPos))));
				// we multiply with attenuation to create falloff
				// we can skip the saturate, since we do not add ambient in this pass
				fixed3 lightCol = _LightColor0.xyz * NdotL *  LIGHT_ATTENUATION(i);
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}	
	}
}