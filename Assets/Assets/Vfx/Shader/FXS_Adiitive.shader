// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify_Shader/Vfx/Master_Add_Shader"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Main_Tex("Main_Tex", 2D) = "white" {}
		_Main_Ins("Main_Ins", Float) = 1
		_Main_Power("Main_Power", Float) = 1
		[Toggle(_USE_SUB_COLOR_ON)] _Use_Sub_Color("Use_Sub_Color", Float) = 0
		[HDR]_Sub_Color("Sub_Color", Color) = (1,1,1,1)
		[HDR]_Main_Color("Main_Color", Color) = (1,1,1,1)
		_Emmision_Ins("Emmision_Ins", Float) = 1
		[Toggle(_COLOR_UV_CUSTOM_ON)] _Color_UV_Custom("Color_UV_Custom", Float) = 0
		_Emission_Offset("Emission_Offset", Range( -1 , 1)) = -0.5
		[Toggle(_MAIN_PANNER_CUSTOM_ON)] _Main_Panner_Custom("Main_Panner_Custom", Float) = 0
		_Main_Upanner("Main_Upanner", Float) = 0
		_Main_Vpanner("Main_Vpanner", Float) = 0
		_Normal_Tex("Normal_Tex", 2D) = "white" {}
		_Normal_Strength("Normal_Strength", Range( -1 , 1)) = 0
		_Normal_Upanner("Normal_Upanner", Float) = 0
		_Normal_Vpanner("Normal_Vpanner", Float) = 0
		[Toggle(_DISTORTION_CUSTOM_ON)] _Distortion_Custom("Distortion_Custom", Float) = 0
		_Normal_Offset("Normal_Offset", Vector) = (0,0,0,0)
		_Normal_Speed("Normal_Speed", Vector) = (0,0,0,0)
		_Mask_Tex("Mask_Tex", 2D) = "white" {}
		[Toggle(_MASK_TEXCOORD_CUSTOM_ON_ON)] _Mask_Texcoord_Custom_on("Mask_Texcoord_Custom_on", Float) = 0
		_Mask_Upanner("Mask_Upanner", Float) = 0
		_Mask_Vpanner("Mask_Vpanner", Float) = 0
		_Dissolve_Tex("Dissolve_Tex", 2D) = "white" {}
		[Toggle(_DISSOLVE_PANNER_CUSTOM_ON)] _Dissolve_Panner_Custom("Dissolve_Panner_Custom", Float) = 0
		_Dissolve_Vpanner("Dissolve_Vpanner", Float) = 0
		_Dissolve_Upanner("Dissolve_Upanner", Float) = 0
		[Toggle(_STEP_CUSTOM_ON)] _Step_Custom("Step_Custom", Float) = 0
		_Step_Value("Step_Value", Float) = 0.1
		_VertexNormal_Offset("VertexNormal_Offset", Range( -1 , 1)) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull Back
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 3.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha One, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120110


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _USE_SUB_COLOR_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _COLOR_UV_CUSTOM_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Normal_Tex_ST;
			float4 _Dissolve_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Main_Color;
			float4 _Sub_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _Normal_Upanner;
			float _Dissolve_Upanner;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Ins;
			float _Main_Power;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Dissolve_Vpanner;
			float _Step_Value;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60306;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = v.ase_texcoord * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = v.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2Dlod( _Sampler60306, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch305, 0, 0.0) ), 1.0f );
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_color = v.ase_color;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode304 * ( v.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.positionCS = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = IN.ase_texcoord3.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = IN.ase_texcoord3.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2D( _Sampler60306, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch305 ), 1.0f );
				float3 temp_output_302_0 = ( (tex2DNode304).xyz * _Normal_Strength );
				float2 uv_Main_Tex = IN.ase_texcoord3.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult292 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner284 = ( 1.0 * _Time.y * appendResult292 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch283 = float3( panner284 ,  0.0 );
				#else
				float3 staticSwitch283 = ( temp_output_302_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode1 = tex2D( _Main_Tex, staticSwitch283.xy );
				float4 temp_cast_3 = (_Main_Power).xxxx;
				float4 temp_output_4_0 = ( ( pow( tex2DNode1 , temp_cast_3 ) * _Main_Ins ) * _Main_Color );
				float2 texCoord261 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord262 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _COLOR_UV_CUSTOM_ON
				float staticSwitch260 = texCoord262.y;
				#else
				float staticSwitch260 = texCoord261.x;
				#endif
				#ifdef _USE_SUB_COLOR_ON
				float4 staticSwitch435 = ( ( _Sub_Color * ( _Emmision_Ins * ( saturate( ( ( 1.0 - staticSwitch260 ) + _Emission_Offset ) ) * tex2DNode1.r ) ) ) + temp_output_4_0 );
				#else
				float4 staticSwitch435 = temp_output_4_0;
				#endif
				
				float2 appendResult345 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = IN.ase_texcoord3.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult345 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch339 = ( IN.ase_texcoord3.z + uv_Mask_Tex );
				#else
				float2 staticSwitch339 = panner343;
				#endif
				float2 uv_Dissolve_Tex = IN.ase_texcoord3.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult360 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner359 = ( 1.0 * _Time.y * appendResult360 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch357 = float3( panner359 ,  0.0 );
				#else
				float3 staticSwitch357 = ( temp_output_302_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_366_0 = ( ( tex2DNode1.r * tex2D( _Mask_Tex, staticSwitch339 ).r ) * ( IN.ase_texcoord3.w + tex2D( _Dissolve_Tex, staticSwitch357.xy ).r ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch379 = step( _Step_Value , temp_output_366_0 );
				#else
				float staticSwitch379 = temp_output_366_0;
				#endif
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( staticSwitch435 * IN.ase_color ).rgb;
				float Alpha = ( IN.ase_color.a * saturate( staticSwitch379 ) );
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.positionCS, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120110


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Normal_Tex_ST;
			float4 _Dissolve_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Main_Color;
			float4 _Sub_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _Normal_Upanner;
			float _Dissolve_Upanner;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Ins;
			float _Main_Power;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Dissolve_Vpanner;
			float _Step_Value;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60306;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = v.ase_texcoord * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = v.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2Dlod( _Sampler60306, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch305, 0, 0.0) ), 1.0f );
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode304 * ( v.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				o.positionCS = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = IN.ase_texcoord2.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = IN.ase_texcoord2.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2D( _Sampler60306, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch305 ), 1.0f );
				float3 temp_output_302_0 = ( (tex2DNode304).xyz * _Normal_Strength );
				float2 uv_Main_Tex = IN.ase_texcoord2.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult292 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner284 = ( 1.0 * _Time.y * appendResult292 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch283 = float3( panner284 ,  0.0 );
				#else
				float3 staticSwitch283 = ( temp_output_302_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode1 = tex2D( _Main_Tex, staticSwitch283.xy );
				float2 appendResult345 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = IN.ase_texcoord2.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult345 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch339 = ( IN.ase_texcoord2.z + uv_Mask_Tex );
				#else
				float2 staticSwitch339 = panner343;
				#endif
				float2 uv_Dissolve_Tex = IN.ase_texcoord2.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult360 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner359 = ( 1.0 * _Time.y * appendResult360 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch357 = float3( panner359 ,  0.0 );
				#else
				float3 staticSwitch357 = ( temp_output_302_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_366_0 = ( ( tex2DNode1.r * tex2D( _Mask_Tex, staticSwitch339 ).r ) * ( IN.ase_texcoord2.w + tex2D( _Dissolve_Tex, staticSwitch357.xy ).r ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch379 = step( _Step_Value , temp_output_366_0 );
				#else
				float staticSwitch379 = temp_output_366_0;
				#endif
				

				float Alpha = ( IN.ase_color.a * saturate( staticSwitch379 ) );
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120110


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Normal_Tex_ST;
			float4 _Dissolve_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Main_Color;
			float4 _Sub_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _Normal_Upanner;
			float _Dissolve_Upanner;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Ins;
			float _Main_Power;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Dissolve_Vpanner;
			float _Step_Value;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60306;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = v.ase_texcoord * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = v.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2Dlod( _Sampler60306, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch305, 0, 0.0) ), 1.0f );
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode304 * ( v.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = IN.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2D( _Sampler60306, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch305 ), 1.0f );
				float3 temp_output_302_0 = ( (tex2DNode304).xyz * _Normal_Strength );
				float2 uv_Main_Tex = IN.ase_texcoord.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult292 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner284 = ( 1.0 * _Time.y * appendResult292 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch283 = float3( panner284 ,  0.0 );
				#else
				float3 staticSwitch283 = ( temp_output_302_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode1 = tex2D( _Main_Tex, staticSwitch283.xy );
				float2 appendResult345 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = IN.ase_texcoord.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult345 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch339 = ( IN.ase_texcoord.z + uv_Mask_Tex );
				#else
				float2 staticSwitch339 = panner343;
				#endif
				float2 uv_Dissolve_Tex = IN.ase_texcoord.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult360 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner359 = ( 1.0 * _Time.y * appendResult360 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch357 = float3( panner359 ,  0.0 );
				#else
				float3 staticSwitch357 = ( temp_output_302_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_366_0 = ( ( tex2DNode1.r * tex2D( _Mask_Tex, staticSwitch339 ).r ) * ( IN.ase_texcoord.w + tex2D( _Dissolve_Tex, staticSwitch357.xy ).r ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch379 = step( _Step_Value , temp_output_366_0 );
				#else
				float staticSwitch379 = temp_output_366_0;
				#endif
				

				surfaceDescription.Alpha = ( IN.ase_color.a * saturate( staticSwitch379 ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120110


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Normal_Tex_ST;
			float4 _Dissolve_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Main_Color;
			float4 _Sub_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _Normal_Upanner;
			float _Dissolve_Upanner;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Ins;
			float _Main_Power;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Dissolve_Vpanner;
			float _Step_Value;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60306;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			float4 _SelectionID;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = v.ase_texcoord * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = v.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2Dlod( _Sampler60306, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch305, 0, 0.0) ), 1.0f );
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode304 * ( v.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				o.positionCS = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = IN.ase_texcoord.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = IN.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2D( _Sampler60306, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch305 ), 1.0f );
				float3 temp_output_302_0 = ( (tex2DNode304).xyz * _Normal_Strength );
				float2 uv_Main_Tex = IN.ase_texcoord.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult292 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner284 = ( 1.0 * _Time.y * appendResult292 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch283 = float3( panner284 ,  0.0 );
				#else
				float3 staticSwitch283 = ( temp_output_302_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode1 = tex2D( _Main_Tex, staticSwitch283.xy );
				float2 appendResult345 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = IN.ase_texcoord.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult345 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch339 = ( IN.ase_texcoord.z + uv_Mask_Tex );
				#else
				float2 staticSwitch339 = panner343;
				#endif
				float2 uv_Dissolve_Tex = IN.ase_texcoord.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult360 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner359 = ( 1.0 * _Time.y * appendResult360 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch357 = float3( panner359 ,  0.0 );
				#else
				float3 staticSwitch357 = ( temp_output_302_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_366_0 = ( ( tex2DNode1.r * tex2D( _Mask_Tex, staticSwitch339 ).r ) * ( IN.ase_texcoord.w + tex2D( _Dissolve_Tex, staticSwitch357.xy ).r ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch379 = step( _Step_Value , temp_output_366_0 );
				#else
				float staticSwitch379 = temp_output_366_0;
				#endif
				

				surfaceDescription.Alpha = ( IN.ase_color.a * saturate( staticSwitch379 ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120110


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _DISTORTION_CUSTOM_ON
			#pragma shader_feature_local _STEP_CUSTOM_ON
			#pragma shader_feature_local _MAIN_PANNER_CUSTOM_ON
			#pragma shader_feature_local _MASK_TEXCOORD_CUSTOM_ON_ON
			#pragma shader_feature_local _DISSOLVE_PANNER_CUSTOM_ON


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Normal_Tex_ST;
			float4 _Dissolve_Tex_ST;
			float4 _Mask_Tex_ST;
			float4 _Main_Tex_ST;
			float4 _Main_Color;
			float4 _Sub_Color;
			float2 _Normal_Offset;
			float2 _Normal_Speed;
			float _Normal_Upanner;
			float _Dissolve_Upanner;
			float _Mask_Vpanner;
			float _Mask_Upanner;
			float _Emission_Offset;
			float _Emmision_Ins;
			float _Main_Ins;
			float _Main_Power;
			float _Main_Vpanner;
			float _Main_Upanner;
			float _Normal_Strength;
			float _VertexNormal_Offset;
			float _Normal_Vpanner;
			float _Dissolve_Vpanner;
			float _Step_Value;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Normal_Tex;
			sampler2D _Sampler60306;
			sampler2D _Main_Tex;
			sampler2D _Mask_Tex;
			sampler2D _Dissolve_Tex;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = v.ase_texcoord * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = v.ase_texcoord.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2Dlod( _Sampler60306, float4( ( appendResult10_g1 + appendResult24_g1 ), 0, 0.0) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2Dlod( _Normal_Tex, float4( staticSwitch305, 0, 0.0) ), 1.0f );
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = ( tex2DNode304 * ( v.normalOS * _VertexNormal_Offset ) );

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

				o.positionCS = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 appendResult309 = (float2(_Normal_Upanner , _Normal_Vpanner));
				float2 uv_Normal_Tex = IN.ase_texcoord1.xy * _Normal_Tex_ST.xy + _Normal_Tex_ST.zw;
				float2 panner308 = ( 1.0 * _Time.y * appendResult309 + uv_Normal_Tex);
				float2 temp_output_1_0_g1 = float2( 1,1 );
				float2 texCoord80_g1 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * texCoord80_g1.x ) , ( texCoord80_g1.y * (temp_output_1_0_g1).y )));
				float2 temp_output_11_0_g1 = float2( 0,0 );
				float2 texCoord81_g1 = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + texCoord81_g1);
				float2 panner19_g1 = ( ( _TimeParameters.x * (temp_output_11_0_g1).y ) * float2( 0,1 ) + texCoord81_g1);
				float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
				float2 temp_output_47_0_g1 = _Normal_Speed;
				float2 texCoord78_g1 = IN.ase_texcoord1.xy * float2( 2,2 ) + float2( 0,0 );
				float2 temp_output_31_0_g1 = ( texCoord78_g1 - float2( 1,1 ) );
				float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / TWO_PI ) ) , length( temp_output_31_0_g1 )));
				float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _TimeParameters.x ) * float2( 1,0 ) + appendResult39_g1);
				float2 panner55_g1 = ( ( _TimeParameters.x * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
				float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
				#ifdef _DISTORTION_CUSTOM_ON
				float2 staticSwitch305 = ( ( (tex2D( _Sampler60306, ( appendResult10_g1 + appendResult24_g1 ) )).rg * 1.0 ) + ( _Normal_Offset * appendResult58_g1 ) );
				#else
				float2 staticSwitch305 = panner308;
				#endif
				float3 tex2DNode304 = UnpackNormalScale( tex2D( _Normal_Tex, staticSwitch305 ), 1.0f );
				float3 temp_output_302_0 = ( (tex2DNode304).xyz * _Normal_Strength );
				float2 uv_Main_Tex = IN.ase_texcoord1.xy * _Main_Tex_ST.xy + _Main_Tex_ST.zw;
				float2 appendResult292 = (float2(_Main_Upanner , _Main_Vpanner));
				float2 panner284 = ( 1.0 * _Time.y * appendResult292 + uv_Main_Tex);
				#ifdef _MAIN_PANNER_CUSTOM_ON
				float3 staticSwitch283 = float3( panner284 ,  0.0 );
				#else
				float3 staticSwitch283 = ( temp_output_302_0 + float3( uv_Main_Tex ,  0.0 ) );
				#endif
				float4 tex2DNode1 = tex2D( _Main_Tex, staticSwitch283.xy );
				float2 appendResult345 = (float2(_Mask_Upanner , _Mask_Vpanner));
				float2 uv_Mask_Tex = IN.ase_texcoord1.xy * _Mask_Tex_ST.xy + _Mask_Tex_ST.zw;
				float2 panner343 = ( 1.0 * _Time.y * appendResult345 + uv_Mask_Tex);
				#ifdef _MASK_TEXCOORD_CUSTOM_ON_ON
				float2 staticSwitch339 = ( IN.ase_texcoord1.z + uv_Mask_Tex );
				#else
				float2 staticSwitch339 = panner343;
				#endif
				float2 uv_Dissolve_Tex = IN.ase_texcoord1.xy * _Dissolve_Tex_ST.xy + _Dissolve_Tex_ST.zw;
				float2 appendResult360 = (float2(_Dissolve_Upanner , _Dissolve_Vpanner));
				float2 panner359 = ( 1.0 * _Time.y * appendResult360 + uv_Dissolve_Tex);
				#ifdef _DISSOLVE_PANNER_CUSTOM_ON
				float3 staticSwitch357 = float3( panner359 ,  0.0 );
				#else
				float3 staticSwitch357 = ( temp_output_302_0 + float3( uv_Dissolve_Tex ,  0.0 ) );
				#endif
				float temp_output_366_0 = ( ( tex2DNode1.r * tex2D( _Mask_Tex, staticSwitch339 ).r ) * ( IN.ase_texcoord1.w + tex2D( _Dissolve_Tex, staticSwitch357.xy ).r ) );
				#ifdef _STEP_CUSTOM_ON
				float staticSwitch379 = step( _Step_Value , temp_output_366_0 );
				#else
				float staticSwitch379 = temp_output_366_0;
				#endif
				

				surfaceDescription.Alpha = ( IN.ase_color.a * saturate( staticSwitch379 ) );
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.positionCS.xyz, unity_LODFade.x );
				#endif

				float3 normalWS = IN.normalWS;

				return half4(NormalizeNormalPerPixel(normalWS), 0.0);
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;418;-19.55264,825.2587;Inherit;False;747.755;354.0595;Comment;10;404;414;413;415;399;416;398;397;410;419;Vertex_Normal_Setting;0.6828581,0.6,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;392;-652.5783,748.5727;Inherit;False;620.5991;357.9984;Comment;10;380;382;383;384;381;385;379;386;387;432;Step_Setting;0.6,0.6777313,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;376;-2323.795,696.3425;Inherit;False;1668.875;640.5963;Comment;20;355;359;360;357;363;356;358;361;362;365;364;366;373;374;375;371;378;393;394;395;Dissolve_Setting;0.6,1,0.8287886,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;353;-2220.283,73.38719;Inherit;False;1311.392;610.1761;Comment;14;320;339;345;341;342;340;346;347;319;344;343;329;354;405;Mask_Setting;0.7785853,1,0.6,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;351;-3660.518,-464.4479;Inherit;False;1707.205;631.7745;Comment;20;304;303;318;308;309;310;311;305;307;306;349;348;302;301;367;368;369;370;372;402;Main_Distortion_Setting;1,0.7888081,0.6,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;312;-1964.894,-375.6116;Inherit;False;1229.296;443.3408;Comment;28;1;2;6;3;7;283;288;289;285;290;291;284;292;294;295;296;323;324;328;330;331;332;333;334;335;336;337;338;Main_Setting;1,0.9824723,0.6,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;282;-2081.469,-902.3749;Inherit;False;1400.328;452.6003;Comment;17;261;262;260;263;264;265;266;267;268;270;269;5;271;277;279;315;321;Color_Setting;1,0.6,0.6,1;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;250;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;252;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;253;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;254;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;255;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;256;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;257;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;258;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;259;193.9,145.2001;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.OneMinusNode;263;-1610.469,-662.7747;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;266;-1347.469,-661.7747;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;267;-1196.469,-663.7747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;268;-1037.469,-685.7747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;270;-880.4692,-689.7747;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;269;-1394.47,-757.9744;Inherit;False;Property;_Emmision_Ins;Emmision_Ins;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;277;-763.1413,-799.644;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;279;-763.1407,-761.2449;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;261;-2072.745,-744.0933;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;260;-1875.465,-667.4821;Inherit;False;Property;_Color_UV_Custom;Color_UV_Custom;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;262;-2079,-622.1251;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1396.844,-325.2131;Inherit;True;Property;_Main_Tex;Main_Tex;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;2;-1079.74,-323.821;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-913.5992,-325.6115;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;314;-766.8368,-263.4913;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-725.6896,-300.5523;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;315;-732.8368,-649.3187;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;317;-731.8368,-370.4913;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;316;-728.8368,-336.4913;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;272;-581.8695,-363.7473;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;-1888.955,-321.2707;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;265;-1773.469,-574.7747;Inherit;False;Property;_Emission_Offset;Emission_Offset;8;0;Create;True;0;0;0;False;0;False;-0.5;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;264;-1463.469,-661.7747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;321;-1228.533,-600.4339;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;323;-1137.533,-291.4339;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;324;-1136.533,-379.4339;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;326;-1231.533,-426.4339;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;325;-1227.533,-388.4339;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;330;-1109.374,-258.8538;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;334;-939.374,-265.8538;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;328;-1138.215,-246.3105;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;331;-1099,-242;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;333;-942,-187;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;336;-789,-243;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;332;-948,-241;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1096,-220;Inherit;False;Property;_Main_Power;Main_Power;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-906,-222;Inherit;False;Property;_Main_Ins;Main_Ins;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;335;-932,-243;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;337;-781,-198;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;283;-1669.76,-333.1324;Inherit;False;Property;_Main_Panner_Custom;Main_Panner_Custom;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;291;-1699.955,-274.2707;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;338;-1699.463,-250.7296;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;288;-1630.955,-252.2706;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;289;-1466.955,-247.2706;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;285;-1437.955,-245.2705;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;290;-1434.955,-183.2705;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;284;-1598.895,-211.2705;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;292;-1745.895,-100.2705;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;295;-1918.425,-18.27085;Inherit;False;Property;_Main_Vpanner;Main_Vpanner;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-1927.895,-98.27046;Inherit;False;Property;_Main_Upanner;Main_Upanner;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;304;-2784.264,-299.1856;Inherit;True;Property;_Normal_Tex;Normal_Tex;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;303;-2467.264,-294.1856;Inherit;False;True;True;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;308;-3230.628,-385.9777;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;309;-3367.628,-299.978;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;305;-3056.664,-279.6857;Inherit;False;Property;_Distortion_Custom;Distortion_Custom;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;307;-3444.627,-414.4479;Inherit;False;0;304;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;306;-3410.164,-155.6857;Inherit;False;RadialUVDistortion;-1;;1;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60306;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;349;-3609.469,6.326557;Inherit;False;Property;_Normal_Speed;Normal_Speed;18;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;348;-3610.518,-122.3235;Inherit;False;Property;_Normal_Offset;Normal_Offset;17;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-2181.314,-277.573;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-3580.081,-296.4497;Inherit;False;Property;_Normal_Upanner;Normal_Upanner;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;311;-3576.139,-223.6573;Inherit;False;Property;_Normal_Vpanner;Normal_Vpanner;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;320;-1050.891,176.8154;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;354;-1102.889,195.1676;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;329;-1105.113,147.6895;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;319;-1365.865,173.8797;Inherit;True;Property;_Mask_Tex;Mask_Tex;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;339;-1698.648,201.9319;Inherit;False;Property;_Mask_Texcoord_Custom_on;Mask_Texcoord_Custom_on;20;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;342;-2038.167,330.7637;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;340;-1821.731,404.2819;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;341;-2040.968,503.9634;Inherit;False;0;319;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;344;-2117.561,104.3412;Inherit;False;0;319;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;343;-1885.761,129.8574;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;345;-2033.162,226.3574;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;347;-2209.244,217.3872;Inherit;False;Property;_Mask_Upanner;Mask_Upanner;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;346;-2209.083,297.6585;Inherit;False;Property;_Mask_Vpanner;Mask_Vpanner;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;367;-2010.412,-218.4728;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;368;-2008.446,-37.5728;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;370;-2020.244,3.719566;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;369;-2264.066,1.753265;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;372;-2273.897,56.80975;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;371;-2283.626,750.5567;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;355;-1785.312,884.3773;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;359;-1891.904,1056.138;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;360;-2039.305,1152.638;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;357;-1664.001,925.6743;Inherit;False;Property;_Dissolve_Panner_Custom;Dissolve_Panner_Custom;24;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;363;-1353.578,915.3425;Inherit;True;Property;_Dissolve_Tex;Dissolve_Tex;23;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;356;-2001.999,901.6304;Inherit;False;0;363;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;358;-2123.704,1030.622;Inherit;False;0;363;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;365;-1280.578,746.3425;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;364;-1025.578,888.3425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;373;-2273.795,799.7143;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;374;-1828.478,802.8874;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;375;-1821.478,835.8874;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;377;-913.6261,215.148;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;378;-918.5943,889.1728;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;366;-880.9458,867.5699;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;362;-2272.226,1222.939;Inherit;False;Property;_Dissolve_Vpanner;Dissolve_Vpanner;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;361;-2273.387,1147.667;Inherit;False;Property;_Dissolve_Upanner;Dissolve_Upanner;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;389;-463.035,-312.9849;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;239;-314.2562,110.4778;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-126.2648,-16.1225;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;380;-568.5783,941.5711;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;382;-602.5783,979.5711;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;383;-592.5783,1018.571;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;384;-483.5782,1024.571;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;381;-443.5782,967.5711;Inherit;False;Property;_Step_Value;Step_Value;28;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;385;-286.5782,997.5711;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;386;-287.5782,1021.571;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;387;-209.9789,798.5727;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;394;-734.5885,986.9166;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;393;-730.7062,913.1475;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;395;-704.8223,992.0934;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;379;-446.8724,868.3943;Inherit;False;Property;_Step_Custom;Step_Custom;27;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;402;-2517.969,-239.1859;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;403;-2513.653,659.9432;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;401;-2471.368,666.0201;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;405;-1472.582,665.8759;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;407;-949.2831,671.8405;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;406;-662.7482,673.3979;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;388;-4.363985,553.827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;411;-226.679,672.7049;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;409;-18.98985,681.1707;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;412;25.52097,687.0051;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;404;30.44736,875.2587;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;414;78.72099,886.5049;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;413;384.2209,890.4054;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;415;501.2208,894.3053;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;399;550.2024,942.6407;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;416;507.7209,929.4048;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;398;379.0027,951.4409;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;397;137.9138,914.7816;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;410;95.94623,1066.318;Inherit;False;Property;_VertexNormal_Offset;VertexNormal_Offset;29;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;419;693.787,966.2619;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;423;154.6984,381.7374;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;421;154.6984,573.3046;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;422;162.0666,341.2135;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;420;688.8759,398.9294;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;424;687.6475,312.9699;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;425;29.44311,20.70753;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;427;331.5294,337.5298;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;426;38.03902,281.0422;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;428;41.72296,314.1978;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;430;-96.05298,285.7501;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;431;-99.19802,568.8162;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;429;-97.62565,255.8708;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;433;-74.08926,612.5781;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;434;-72.96255,715.1009;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;432;-68.45618,826.6364;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;318;-2432.219,-204.6729;Inherit;False;Property;_Normal_Strength;Normal_Strength;13;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;301;-2195.314,-161.5731;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;435;-394.8709,-330.4305;Inherit;False;Property;_Use_Sub_Color;Use_Sub_Color;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;251;742.7553,286.88;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;Amplify_Shader/Vfx/Master_Add_Shader;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;True;True;8;5;False;;1;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;22;Surface;1;638442149404295204;  Blend;2;638442149450675250;Two Sided;1;638574839673188383;Forward Only;0;0;Cast Shadows;0;638567330846107172;  Use Shadow Threshold;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;0;638567332177941917;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.ColorNode;5;-984.2297,-849.3442;Inherit;False;Property;_Main_Color;Main_Color;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;271;-1203.669,-852.3749;Inherit;False;Property;_Sub_Color;Sub_Color;4;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;263;0;260;0
WireConnection;266;0;264;0
WireConnection;267;0;266;0
WireConnection;267;1;321;0
WireConnection;268;0;269;0
WireConnection;268;1;267;0
WireConnection;270;0;271;0
WireConnection;270;1;268;0
WireConnection;277;0;5;0
WireConnection;279;0;277;0
WireConnection;260;1;261;1
WireConnection;260;0;262;2
WireConnection;1;1;283;0
WireConnection;2;0;1;0
WireConnection;2;1;330;0
WireConnection;3;0;2;0
WireConnection;3;1;334;0
WireConnection;314;0;279;0
WireConnection;4;0;3;0
WireConnection;4;1;314;0
WireConnection;315;0;270;0
WireConnection;317;0;315;0
WireConnection;316;0;317;0
WireConnection;272;0;316;0
WireConnection;272;1;4;0
WireConnection;296;0;302;0
WireConnection;296;1;301;0
WireConnection;264;0;263;0
WireConnection;264;1;265;0
WireConnection;321;0;326;0
WireConnection;323;0;1;1
WireConnection;324;0;323;0
WireConnection;326;0;325;0
WireConnection;325;0;324;0
WireConnection;330;0;331;0
WireConnection;334;0;335;0
WireConnection;328;0;1;1
WireConnection;331;0;332;0
WireConnection;333;0;6;0
WireConnection;336;0;337;0
WireConnection;332;0;333;0
WireConnection;335;0;336;0
WireConnection;337;0;7;0
WireConnection;283;1;296;0
WireConnection;283;0;291;0
WireConnection;291;0;338;0
WireConnection;338;0;288;0
WireConnection;288;0;289;0
WireConnection;289;0;285;0
WireConnection;285;0;290;0
WireConnection;290;0;284;0
WireConnection;284;0;301;0
WireConnection;284;2;292;0
WireConnection;292;0;294;0
WireConnection;292;1;295;0
WireConnection;304;1;305;0
WireConnection;303;0;304;0
WireConnection;308;0;307;0
WireConnection;308;2;309;0
WireConnection;309;0;310;0
WireConnection;309;1;311;0
WireConnection;305;1;308;0
WireConnection;305;0;306;0
WireConnection;306;68;348;0
WireConnection;306;47;349;0
WireConnection;302;0;303;0
WireConnection;302;1;318;0
WireConnection;320;0;354;0
WireConnection;320;1;319;1
WireConnection;354;0;329;0
WireConnection;329;0;328;0
WireConnection;319;1;339;0
WireConnection;339;1;343;0
WireConnection;339;0;340;0
WireConnection;340;0;342;3
WireConnection;340;1;341;0
WireConnection;343;0;344;0
WireConnection;343;2;345;0
WireConnection;345;0;347;0
WireConnection;345;1;346;0
WireConnection;367;0;302;0
WireConnection;368;0;367;0
WireConnection;370;0;368;0
WireConnection;369;0;370;0
WireConnection;372;0;369;0
WireConnection;371;0;372;0
WireConnection;355;0;375;0
WireConnection;355;1;356;0
WireConnection;359;0;358;0
WireConnection;359;2;360;0
WireConnection;360;0;361;0
WireConnection;360;1;362;0
WireConnection;357;1;355;0
WireConnection;357;0;359;0
WireConnection;363;1;357;0
WireConnection;364;0;365;4
WireConnection;364;1;363;1
WireConnection;373;0;371;0
WireConnection;374;0;373;0
WireConnection;375;0;374;0
WireConnection;377;0;320;0
WireConnection;378;0;377;0
WireConnection;366;0;378;0
WireConnection;366;1;364;0
WireConnection;389;0;272;0
WireConnection;238;0;435;0
WireConnection;238;1;239;0
WireConnection;380;0;382;0
WireConnection;380;1;395;0
WireConnection;382;0;383;0
WireConnection;383;0;384;0
WireConnection;384;0;386;0
WireConnection;385;0;381;0
WireConnection;386;0;385;0
WireConnection;387;0;379;0
WireConnection;394;0;393;0
WireConnection;393;0;366;0
WireConnection;395;0;394;0
WireConnection;379;1;366;0
WireConnection;379;0;380;0
WireConnection;402;0;304;0
WireConnection;403;0;402;0
WireConnection;401;0;403;0
WireConnection;405;0;401;0
WireConnection;407;0;405;0
WireConnection;406;0;407;0
WireConnection;388;0;431;0
WireConnection;388;1;433;0
WireConnection;411;0;406;0
WireConnection;409;0;411;0
WireConnection;412;0;409;0
WireConnection;404;0;412;0
WireConnection;414;0;404;0
WireConnection;413;0;414;0
WireConnection;415;0;413;0
WireConnection;399;0;416;0
WireConnection;399;1;398;0
WireConnection;416;0;415;0
WireConnection;398;0;397;0
WireConnection;398;1;410;0
WireConnection;419;0;399;0
WireConnection;423;0;421;0
WireConnection;421;0;388;0
WireConnection;422;0;423;0
WireConnection;420;0;419;0
WireConnection;424;0;428;0
WireConnection;425;0;238;0
WireConnection;427;0;422;0
WireConnection;426;0;425;0
WireConnection;428;0;426;0
WireConnection;430;0;429;0
WireConnection;431;0;430;0
WireConnection;429;0;239;4
WireConnection;433;0;434;0
WireConnection;434;0;432;0
WireConnection;432;0;387;0
WireConnection;435;1;4;0
WireConnection;435;0;389;0
WireConnection;251;2;424;0
WireConnection;251;3;427;0
WireConnection;251;5;420;0
ASEEND*/
//CHKSM=32C700DC07DC800E763CAA7FAA39129F8EF3CD82