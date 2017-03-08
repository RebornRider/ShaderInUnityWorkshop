Shader "ShadersInUnityWorkshop/Reference/SurfaceShaders/Basics/PBR AllMaps - Reference" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_GlossinessMap ("Smoothness Map", 2D) = "black" {}
		_MetallicMap ("Metallic Map", 2D) = "black" {}
		_EmissionMap ("Emission Map", 2D) = "black" {}	
		_OcclusionMap ("Occlusion Map", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _GlossinessMap;
		sampler2D _MetallicMap;
		sampler2D _EmissionMap;
		sampler2D _OcclusionMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_GlossinessMap;
			float2 uv_MetallicMap;
			float2 uv_EmissionMap;
			float2 uv_OcclusionMap;
		};

		fixed4 _Color;	

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
			o.Metallic = tex2D (_MetallicMap, IN.uv_MetallicMap).r;
			o.Smoothness = tex2D (_GlossinessMap, IN.uv_GlossinessMap).r;			
			o.Emission = tex2D (_EmissionMap, IN.uv_EmissionMap);
			o.Occlusion = tex2D (_OcclusionMap, IN.uv_OcclusionMap).r;
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}