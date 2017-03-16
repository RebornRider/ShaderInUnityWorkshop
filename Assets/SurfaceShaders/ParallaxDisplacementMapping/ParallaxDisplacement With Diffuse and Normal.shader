Shader "Custom/ParallaxDisplacement With Diffuse and Normal" 
{
		Properties 
		{
			_Color ("Main Color", Color) = (1,1,1,1)
			_Parallax ("Height", Range (0.005, 0.08)) = 0.02
			_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
			_BumpMap ("Normalmap", 2D) = "bump" {}
			[Toggle(_FlipParallax_ON)]
			_FlipParallax("Flip Heightmap direction", Float) = 0
			_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
		}
		SubShader {
			Tags { "RenderType"="Opaque" }
			LOD 500
	
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		#pragma shader_feature _FlipParallax_ON

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _ParallaxMap;

		fixed4 _Color;
		fixed4 _ReflectColor;
		float _Parallax;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			#if _FlipParallax_ON
			_Parallax = 1 - _Parallax;
			#endif

			half h = tex2D (_ParallaxMap, IN.uv_BumpMap).r;
			float2 offset = ParallaxOffset (1-h, _Parallax, IN.viewDir);
			IN.uv_MainTex += offset;
			IN.uv_BumpMap += offset;
	
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
	
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
