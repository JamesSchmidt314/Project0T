Shader "Custom/WaterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Scale ("Scale", Range(0,10)) = 1
		_Speed ("Speed", Range(0,10)) = 1
		_Frequency ("Frequency", Range(0,10)) = 1

		_TexWarpXSpeed ("Texture Warp Speed (X Coord)", Range(0,10)) = 1
		_TexWarpYSpeed ("Texture Warp Speed (Y Coord)", Range(0,10)) = 1
	}
	SubShader {
		Tags{"RenderType"="Opaque"}
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert alpha vertex:vert

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten)
		{
			half NdotL = dot(s.Normal, lightDir);
			NdotL = smoothstep(0,0.025f,NdotL);
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		float _Scale, _Speed, _Frequency, _TexWarpXSpeed,_TexWarpYSpeed, _YVal;
		half4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float3 customValue;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			half offsetvert1 = v.vertex.x;
			half offsetvert2 = v.vertex.z;

			half value1 = _Scale * sin(_Time.w * _Speed + offsetvert1 * _Frequency);
			half value2 = _Scale * sin(_Time.w * _Speed + offsetvert2 * _Frequency);

			v.vertex.y += value1;
			v.vertex.y += value2/2;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			//half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half4 c = tex2D (_MainTex, IN.uv_MainTex + (float2(0.2f,0)*sin((_Time*(_TexWarpYSpeed*2)+IN.uv_MainTex.y))) + (float2(0,0.2f)*sin((_Time*_TexWarpXSpeed+IN.uv_MainTex.x)))) * _Color;
			//(float2(0.2f,0)*sin((_Time*5+IN.uv_MainTex.y)))
			//(float2(0.2f,0)*sin(_Time*5+IN.uv_MainTex.x))
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			//o.Normal.y += IN.customValue;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
