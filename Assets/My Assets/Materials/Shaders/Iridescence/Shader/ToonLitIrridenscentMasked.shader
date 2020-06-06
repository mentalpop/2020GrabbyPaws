Shader "Toon/Iridescent Masked" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		
		_MainTex("Base (RGB)", 2D) = "white" {}

	_Noise("Noise (RGB)", 2D) = "white" {} // noise texture
	_Mask("Iri Mask (RGB)", 2D) = "white" {} // mask texture
	_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	_IrTex("Iridescence Ramp (RGB)", 2D) = "white" {} // color ramp
	_IrColor("Ir Color", Color) = (0.5,0.5,0.5,1)// extra iridescence tinting
	_Offset ("Iridescence Ramp Offset", Range (0, 1)) = 1 // offset of the color ramp
	_Brightness ("Iridescence Opacity", Range (0, 1)) = 1 // opacity of iridescence
	_WorldScale ("Noise Worldscale", Range (.002, 5)) = 1 // noise scale
	[Toggle(LM)] _LM("Use Lightmap UVS?", Float) = 0 // use lightmap uvs instead of normal ones
		   _EmissionLM("Emission (Lightmapper)", Float) = 0
   [Toggle] _DynamicEmissionLM("Dynamic Emission (Lightmapper)", Int) = 0 // emission for the unemissioned


	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf ToonRamp
#pragma shader_feature LM
		sampler2D _Ramp;

	// custom lighting function that uses a texture ramp based
	// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
	inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
#endif

		half d = dot(s.Normal, lightDir)*0.5 + 0.5;
		half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
		c.a = 0;
		return c;
	}


	sampler2D _MainTex, _Mask;
	sampler2D _Noise; // noise
	sampler2D _IrTex; // color ramp
	float4 _Color;
	float4 _IrColor; // extra tinting
	float _Offset; // color ramp offset
	float _Brightness; // Iridescence opacity
	float _WorldScale; // noise scale
	float _EmissionLM; // Emission


	struct Input {
		float2 uv_MainTex : TEXCOORD0;
		float2 uv2_Noise : TEXCOORD1; // lightmap uvs
		float3 viewDir; // view direction
	};

	void surf(Input IN, inout SurfaceOutput o) {


		half f = 1 -dot(o.Normal, IN.viewDir) + _Offset; // fresnel
		half4 m = tex2D(_Mask, IN.uv_MainTex); // mask
		half4 n = tex2D(_Noise, IN.uv_MainTex * _WorldScale); // noise based on the first uv set
#if LM
		n = tex2D(_Noise, IN.uv2_Noise * _WorldScale); // noise based on the lightmap uv set
#endif

		half4 c = tex2D(_MainTex, IN.uv_MainTex ) * _Color; 

		
		half3 i = tex2D(_IrTex, float2(f,f)+ n).rgb * _IrColor; // iridescence effect
		
		o.Albedo = (c.rgb) + ((i * n) * _Brightness * m); // multiplied by original texture, with an opacity float
		
		o.Alpha = c.a;


	}
	ENDCG

	}

		Fallback "Diffuse"
}