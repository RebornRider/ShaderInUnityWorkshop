Shader "ShadersInUnityWorkshop/Reference/SurfaceShaders/RampShading/RampShading - Reference" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]
		_MainTex ("Texture", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "white" {}
		_AttenMod ("Attennuation", Range(1.0, 2.0)) = 1.0
	}
	SubShader {

		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Ramp

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
		}

		sampler2D _Ramp;
		half4 _Ramp_ST;
		float _AttenMod;

		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) 
		{
			
			half NdotL = dot (s.Normal, lightDir);			
			half diff = NdotL * 0.5 + 0.5;
			half2 rampUV = TRANSFORM_TEX(float2(diff, 0.5), _Ramp);
			half3 ramp = (tex2D (_Ramp, rampUV)).rgb;
			
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb  * (ramp * atten * _AttenMod);
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	}
}
