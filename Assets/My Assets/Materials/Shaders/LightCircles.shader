Shader "Custom/StencilLight"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,0.5)

		_StencilRef("Stencil Reference Value", Float) = 20
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent-10"}
			LOD 100
			Pass
			{
			//This pass only draws under opaque objects
			Ztest Greater
			//Does not affect the zbuffer, we will still need surface depth information
			//not only for this shader but for the other transparent stuff that might get drawn after
			Zwrite off
			//Draw the insides of the primitive
			Cull Front
			//Do not output any color information
			Colormask 0
			Name "Stencil Greater"
			//As long as the Ztest passes, 20 shall be written to the stencil buffer
			Stencil
			{
				comp always
				ref[_StencilRef]
				pass replace
			}
		}
			/*
			This pass is now redundant
			Pass
			{
				Zwrite off
				Ztest lequal
				Cull Back
				Colormask 0
				Name "Stencil LEqual"
				Stencil
				{
					comp equal
					ref 20
					pass keep
					fail zero
					zfail zero
				}
			}*/

			Pass
			{
				Name "Color"
				//Once more we don't want to touch the zbuffer
				Zwrite off
			//Standard Ztest and backface culling, left these here just for clarity
			Ztest Lequal
			Cull Back
			//Additive, but taking alpha output into account for blending.
			Blend SrcAlpha One

			//Will only draw if intersecting with the stencil value from the previous pass
			//Even if the Ztest fails, it will still clear the stencil value
			//so that any following lights also render correctly without reading previous lights' stencil values.
			Stencil
			{
				comp equal
				ref[_StencilRef]
				pass zero
				fail zero
				zfail zero
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) *_Color;
				return col;
			}
			ENDCG
		}
		}
}