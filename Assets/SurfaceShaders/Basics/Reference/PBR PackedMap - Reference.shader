Shader "ShadersInUnityWorkshop/Reference/SurfaceShaders/Basics/PBR Packed Map - Reference" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_EmissionMap ("Emission Map", 2D) = "black" {}	
		_PackedMap ("Smothness (R) | Metallic (G) | Occlusion( B)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _EmissionMap;
		sampler2D _PackedMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_EmissionMap;
			float2 uv_PackedMap;
		};

		fixed4 _Color;	

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
			float3 GlossMetalOcclusion = tex2D (_PackedMap, IN.uv_PackedMap);		
			o.Metallic = GlossMetalOcclusion.r;
			o.Smoothness = GlossMetalOcclusion.g;		
			o.Emission = tex2D (_EmissionMap, IN.uv_EmissionMap);
			o.Occlusion = GlossMetalOcclusion.b;		
		}
		ENDCG
	}
	FallBack "Diffuse"
}