Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight MultipleLights NormalMap ShadowCasting ShadowReceiving Fog - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Normal]
		_NormalMap ("Normalmap", 2D) = "bump" {}
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

		sampler2D _NormalMap;
		float4 _NormalMap_ST;

		half3 decodeNormal(half3 tspace0, half3 tspace1, half3 tspace2, float2 uv)
		{
			// sample the normal map, and decode from the Unity encoding
			half3 tnormal = UnpackNormal(tex2D(_NormalMap, uv));
			// transform normal from tangent to world space
			half3 worldNormal;
			worldNormal.x = dot(tspace0, tnormal);
			worldNormal.y = dot(tspace1, tnormal);
			worldNormal.z = dot(tspace2, tnormal);
			return worldNormal;
		}
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

			// shadow helper functions and macros
			#include "AutoLight.cginc"	
				
			// compile shader into multiple variants, with and without shadows
			// (we don't care about any lightmaps yet, so skip these variants)
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight		

			// Needed for fog variation to be compiled.
			#pragma multi_compile_fog				

			struct vertexOutput {		
				float4 modelPos : TEXCOORD0;
				// these three vectors will hold a 3x3 rotation matrix
				// that transforms from tangent to world space
				half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
				float2 uv : TEXCOORD4;	
				SHADOW_COORDS(5) // put shadows data into TEXCOORD6
				UNITY_FOG_COORDS(6) // put fog data into TEXCOORD7
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;	
				o.modelPos = v.vertex;			
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				// handle shadows
				TRANSFER_SHADOW(o)
				//Compute fog amount from clip space position.
				UNITY_TRANSFER_FOG(o,o.pos);

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				return o;
			}
			
			fixed3 fragBasePass (vertexOutput i) : SV_Target
			{
				half3 worldNormal = decodeNormal(i.tspace0, i.tspace1, i.tspace2, TRANSFORM_TEX(i.uv, _NormalMap));
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(worldNormal, UnityWorldSpaceLightDir(i.modelPos)));
				// factor in the light color 
				fixed3 lightCol = saturate(NdotL * _LightColor0 + ShadeSH9(half4(worldNormal,1)));
				
				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
				fixed shadowAtten = SHADOW_ATTENUATION(i);

				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				
				fixed4 finalColor = float4(diffuseColor * lightCol * shadowAtten, 1);
				//Apply fog (additive pass are automatically handled)
				UNITY_APPLY_FOG(i.fogCoord, finalColor); 
			
				return finalColor;
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

			#pragma multi_compile_fwdadd_fullshadows 

			struct vertexOutput {		
				float3 worldPos : TEXCOORD0;
				// these three vectors will hold a 3x3 rotation matrix
				// that transforms from tangent to world space
				half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
				float2 uv : TEXCOORD4;	
				float4 pos : SV_POSITION;
				LIGHTING_COORDS(5,6) 				
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;	
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				TRANSFER_VERTEX_TO_FRAGMENT(o); 

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				return o;
			}

			fixed3 fragAddPass (vertexOutput i) : SV_Target
			{
				half3 worldNormal = decodeNormal(i.tspace0, i.tspace1, i.tspace2, TRANSFORM_TEX(i.uv, _NormalMap));
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos)
				// dot product between normal and light direction for lambert lighting
				half NdotL = saturate(dot(worldNormal, normalize(UnityWorldSpaceLightDir(i.worldPos))));
				// we multiply with attenuation to create falloff
				// we can skip the saturate, since we do not add ambient in this pass
				fixed3 lightCol = _LightColor0.xyz * NdotL * attenuation;

				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}	
			ENDCG
		}	
		
		// shadow caster rendering pass
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

	}
}