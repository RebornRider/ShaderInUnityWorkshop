Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight MultipleLights NormalMap ShadowCasting - Reference"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Normal]
		_NormalMap ("Normalmap", 2D) = "bump" {}
	}

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

		sampler2D _NormalMap;
		float4 _NormalMap_ST;

		half3 decodeNormal(half3 tspace0, half3 tspace1, half3 tspace2, float2 uv)
		{
			half3 tnormal = UnpackNormal(tex2D(_NormalMap, uv));
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
			Tags {"LightMode"="ForwardBase"}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragBasePass	

			struct vertexOutput {		
				float4 modelPos : TEXCOORD0;
				half3 tspace0 : TEXCOORD1;
				half3 tspace1 : TEXCOORD2;
				half3 tspace2 : TEXCOORD3;
				float2 uv : TEXCOORD4;	
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;	
				o.modelPos = v.vertex;			
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;	
				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				return o;
			}	

			fixed3 fragBasePass (vertexOutput i) : SV_Target
			{
				half3 worldNormal = decodeNormal(i.tspace0, i.tspace1, i.tspace2, TRANSFORM_TEX(i.uv, _NormalMap));
				half NdotL = saturate(dot(worldNormal, UnityWorldSpaceLightDir(i.modelPos)));
				fixed3 lightCol = saturate((NdotL * _LightColor0) + ShadeSH9(half4(worldNormal,1)));
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" } 
			Blend One One 
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAddPass

			#include "AutoLight.cginc" 

			#pragma multi_compile_fwdadd

			struct vertexOutput {		
				float3 worldPos : TEXCOORD0;
				half3 tspace0 : TEXCOORD1; 
				half3 tspace1 : TEXCOORD2; 
				half3 tspace2 : TEXCOORD3; 
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
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				return o;
			}
		
			fixed3 fragAddPass (vertexOutput i) : SV_Target
			{
				half3 worldNormal = decodeNormal(i.tspace0, i.tspace1, i.tspace2, TRANSFORM_TEX(i.uv, _NormalMap));	
				half NdotL = saturate(dot(worldNormal, normalize(UnityWorldSpaceLightDir(i.worldPos))));
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos)
				fixed3 lightCol = _LightColor0.xyz * NdotL * attenuation;
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}	
		
		// shadow caster rendering pass, implemented manually using macros from UnityCG.cginc
		// eaquals to: UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		Pass
		{
			Tags {"LightMode"="ShadowCaster"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster

			struct v2f { 
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}