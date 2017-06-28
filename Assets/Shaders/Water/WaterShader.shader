Shader "Custom/WaterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0


		_TexWarpXSpeed ("Texture Warp Speed (X Coord)", Range(0,10)) = 1
		_TexWarpYSpeed ("Texture Warp Speed (Y Coord)", Range(0,10)) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		ZWrite Off
		LOD 200

		//Blend SrcBlend DestBlend
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float _TexWarpXSpeed,_TexWarpYSpeed;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + (float2(0.2f,0)*sin((_Time*(_TexWarpYSpeed*2)+IN.uv_MainTex.y))) + (float2(0,0.2f)*sin((_Time*_TexWarpXSpeed+IN.uv_MainTex.x)))) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
