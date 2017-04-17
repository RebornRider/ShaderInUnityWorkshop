Shader "ShadersInUnityWorkshop/Reference/VertexAndFragmentShaders/Basics/Solid Color - Reference"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
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
			
			float4 _Color;
			fixed4 frag () : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}