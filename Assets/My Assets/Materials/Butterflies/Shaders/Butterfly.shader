//This shader is created in Amplify, but then modificated in notepad!

Shader "ERB/Particles/Butterfly"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Emission("Emission", Float) = 1.5
		_Speed("Speed", Float) = 20
		_WingPower("Wing Power", Float) = 0.2
		_Wingmaxdown("Wing max down", Range( 0 , 1)) = 0.5
	}


	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				//#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float3 ase_normal : NORMAL;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float _Speed;
				uniform float _Wingmaxdown;
				uniform float _WingPower;
				uniform float4 _Color;
				uniform float _Emission;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float mulTime5 = _Time.y * _Speed;
					float3 uv03 = v.texcoord.xyz;
					uv03.xy = v.texcoord.xyz.xy * float2( 1,1 ) + float2( 0,0 );
					float clampResult13 = clamp( ( pow( frac( ( uv03.x + -0.2 ) ) , 20.0 ) * 2000.0 ) , 0.0 , 1.0 );	
					v.vertex.xyz += ( (-1.0 + (sin( ( mulTime5 + uv03.z ) ) - -1.0) * (_Wingmaxdown - -1.0) / (1.0 - -1.0)) * clampResult13 * v.ase_normal * _WingPower );
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
					float4 appendResult57 = (float4((( tex2DNode2 * _Color * i.color * _Emission )).rgb , ( tex2DNode2.a * _Color.a * i.color.a )));				
					fixed4 col = appendResult57;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
/*ASEBEGIN
Version=16700
7;87;1906;946;1251.595;494.6689;1.473649;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1749.501,634.3;Float;True;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1394.113,434.94;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-1474.827,826.22;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;8;-1296.268,823.9592;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1239.217,436.506;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-743.6832,-346.296;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;14b845f5bfa59e74da3a0a3030e0fe2c;14b845f5bfa59e74da3a0a3030e0fe2c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-609.2553,13.61012;Float;False;Property;_Emission;Emission;2;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-656.1685,-151.2166;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1027.294,503.9936;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;10;-1090.523,825.7833;Float;True;2;0;FLOAT;0;False;1;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;20;-565.4923,211.1402;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-235.5615,10.1788;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-848.9823,686.2422;Float;False;Property;_Wingmaxdown;Wing max down;5;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;4;-833.6704,461.3597;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-810.4903,837.5204;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;2000;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-229.6995,158.598;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;56;-99.6045,3.62858;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-573.6266,1170.955;Float;False;Property;_WingPower;Wing Power;4;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;13;-574.9629,841.1405;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;42;-584.1919,495.214;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;6;-601.5726,1000.028;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;57;118.7591,8.402247;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-338.4967,699.123;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;55;352.2021,319.7822;Float;False;True;2;Float;;0;11;EGA/Particles/Butterfly;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;12;0;3;1
WireConnection;8;0;12;0
WireConnection;5;0;7;0
WireConnection;38;0;5;0
WireConnection;38;1;3;3
WireConnection;10;0;8;0
WireConnection;39;0;2;0
WireConnection;39;1;40;0
WireConnection;39;2;20;0
WireConnection;39;3;41;0
WireConnection;4;0;38;0
WireConnection;11;0;10;0
WireConnection;21;0;2;4
WireConnection;21;1;40;4
WireConnection;21;2;20;4
WireConnection;56;0;39;0
WireConnection;13;0;11;0
WireConnection;42;0;4;0
WireConnection;42;4;44;0
WireConnection;57;0;56;0
WireConnection;57;3;21;0
WireConnection;14;0;42;0
WireConnection;14;1;13;0
WireConnection;14;2;6;0
WireConnection;14;3;15;0
WireConnection;55;0;57;0
WireConnection;55;1;14;0
ASEEND*/
//CHKSM=C3B8389BF80C73F537DC220D8834C7DD40FB72BE