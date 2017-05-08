Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/LambertLighting/LambertLighting AmbientLight - Reference"
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
				// factor in the light color and ambience
				fixed3 lightCol = saturate((NdotL * _LightColor0)  + ShadeSH9(half4(worldNormal,1)));
				fixed3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				return diffuseColor * lightCol;
			}			
			ENDCG
		}
	}
}