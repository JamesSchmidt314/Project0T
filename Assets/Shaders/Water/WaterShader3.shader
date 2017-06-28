Shader "Custom/WaterShader3" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BlendTex("Blend (RGB)", 2D) = "white"
		_BlendAlpha("Blend Alpha", float) = 0

		_TexWarpXSpeed ("Texture Warp Speed (X Coord)", float) = 1.0
		_TexWarpYSpeed ("Texture Warp Speed (Y Coord)", float) = 10.0

	}
	SubShader {
		Tags { "Queue"="Geometry-9"
		 "IgnoreProjector"="True"
		 "RenderType"="Transparent"}
		 Lighting Off
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _BlendTex;
		float _BlendAlpha;

		float _TexWarpXSpeed,_TexWarpYSpeed;

		struct Input {
			float2 uv_MainTex;
		};

		//half _Glossiness;
		//half _Metallic;
		//fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//fixed4 c = ((1 - _BlendAlpha) * tex2D(_MainTex, IN.uv_MainTex) + _BlendAlpha * tex2D(_BlendTex, IN.uv_MainTex)) * _Color;

			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex + (float2(0.2f,0)*sin((_Time*(_TexWarpYSpeed*2)+IN.uv_MainTex.y))) + (float2(0,0.2f)*sin((_Time*_TexWarpXSpeed+IN.uv_MainTex.x)))) * _Color;

			fixed4 c = ((2 - _BlendAlpha) * tex2D(_MainTex, IN.uv_MainTex + (float2(0.2f,0)*sin((_Time*(_TexWarpYSpeed*2)+IN.uv_MainTex.y))) + (float2(0,0.2f)*sin((_Time*_TexWarpXSpeed+IN.uv_MainTex.x))))
			 + _BlendAlpha * tex2D(_BlendTex, IN.uv_MainTex + (float2(0.2f,0)*sin((_Time*(_TexWarpYSpeed*1.5)+IN.uv_MainTex.y))) + (float2(0,0.2f)*sin((_Time*(_TexWarpXSpeed * 0.5)+IN.uv_MainTex.x))))) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
