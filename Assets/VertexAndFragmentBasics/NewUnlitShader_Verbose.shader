Shader "Unlit/NewUnlitShader_Verbose"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vertexShaderFunction
			#pragma fragment fragmentShaderFunction
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 modelSpacePosition : POSITION;
				float2 uvChannel0Coordinate : TEXCOORD0;
			};

			struct vertexShaderOutputToFragmentShaderInput
			{
				float2 uvChannel0Coordinate : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 projectionSpacePosition : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			vertexShaderOutputToFragmentShaderInput vertexShaderFunction (appdata inputVertex)
			{
				vertexShaderOutputToFragmentShaderInput output;

				output.projectionSpacePosition = UnityObjectToClipPos(inputVertex.modelSpacePosition);
				//output.projectionSpacePosition = mul(UNITY_MATRIX_MVP, inputVertex.modelSpacePosition);
				//output.projectionSpacePosition = inputVertex.modelSpacePosition;
				
				output.uvChannel0Coordinate = TRANSFORM_TEX(inputVertex.uvChannel0Coordinate, _MainTex);
				//output.uvChannel0Coordinate = inputVertex.uvChannel0Coordinate * _MainTex_ST;

				UNITY_TRANSFER_FOG(output,output.projectionSpacePosition);
				return output;
			}
			
			fixed4 fragmentShaderFunction (vertexShaderOutputToFragmentShaderInput input) : SV_Target
			{
				// sample the texture
				fixed4 outputColor = tex2D(_MainTex, input.uvChannel0Coordinate);
				// apply fog
				UNITY_APPLY_FOG(input.fogCoord, outputColor);
				return outputColor;
			}
			ENDCG
		}
	}
}
