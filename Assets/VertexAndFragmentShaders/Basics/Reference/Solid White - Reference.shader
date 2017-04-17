Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/Basics/Solid White - Reference"
{
	Properties {}
	SubShader
	{
		Tags 
		{ 
		"RenderType"="Opaque" 
		"PreviewType" = "Plane"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 vert (float4 position : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(position);
			}
			
			fixed4 frag () : SV_Target
			{
				return float4(1,1,1,1);
			}
			ENDCG
		}
	}
}