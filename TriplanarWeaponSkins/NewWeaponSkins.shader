// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NewWeaponSkins"
{
	Properties
	{
		_overlayDiffuse2("overlayDiffuse2", 2D) = "white" {}
		_AdditionalScratches("AdditionalScratches", 2D) = "white" {}
		_BaseScratches("BaseScratches", 2D) = "white" {}
		_AdditionalscratchesOpacity("AdditionalscratchesOpacity", Range( 0 , 1)) = 1
		_AdditionalScratchesCutoff("AdditionalScratchesCutoff", Range( 0 , 1)) = 0
		_BaseScratchesCutoff("BaseScratchesCutoff", Range( 0 , 1)) = 0
		_AdditionalScratchesTiling("AdditionalScratchesTiling", Float) = 1
		_Tiling("Tiling", Float) = 0
		_Tint("Tint", Color) = (0,0,0,0)
		_AmbientOcclusion("AmbientOcclusion", 2D) = "white" {}
		_MetalSmoothness("MetalSmoothness", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform float4 _Tint;
		uniform sampler2D _overlayDiffuse2;
		uniform float _Tiling;
		uniform sampler2D _BaseScratches;
		uniform float4 _BaseScratches_ST;
		uniform float _BaseScratchesCutoff;
		uniform sampler2D _AdditionalScratches;
		uniform float _AdditionalScratchesTiling;
		uniform float _AdditionalScratchesCutoff;
		uniform float _AdditionalscratchesOpacity;
		uniform float _MetalSmoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 localPos = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) );
			float3 localNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 triplanar50 = TriplanarSamplingSF( _overlayDiffuse2, localPos, localNormal, 1.0, _Tiling, 0 );
			float4 lerpResult53 = lerp( _Tint , triplanar50 , triplanar50.w);
			float2 uv_BaseScratches = i.uv_texcoord * _BaseScratches_ST.xy + _BaseScratches_ST.zw;
			float ifLocalVar34 = 0;
			if( Luminance(tex2D( _BaseScratches, uv_BaseScratches ).rgb) > _BaseScratchesCutoff )
				ifLocalVar34 = 1.0;
			float4 triplanar46 = TriplanarSamplingSF( _AdditionalScratches, localPos, localNormal, 1.0, _AdditionalScratchesTiling, 0 );
			float ifLocalVar40 = 0;
			if( Luminance(triplanar46.xyz) > _AdditionalScratchesCutoff )
				ifLocalVar40 = _AdditionalscratchesOpacity;
			float clampResult45 = clamp( ( ifLocalVar34 + ifLocalVar40 ) , 0.0 , 1.0 );
			float BaseToCoverAlpha37 = clampResult45;
			float4 lerpResult49 = lerp( lerpResult53 , float4( 1,1,1,0 ) , BaseToCoverAlpha37);
			o.Albedo = lerpResult49.xyz;
			float temp_output_48_0 = BaseToCoverAlpha37;
			o.Metallic = temp_output_48_0;
			o.Smoothness = ( BaseToCoverAlpha37 * _MetalSmoothness );
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
0;723;1350;270;1495.899;157.3585;1.535138;True;True
Node;AmplifyShaderEditor.RangedFloatNode;47;-1228.62,228.6642;Float;False;Property;_AdditionalScratchesTiling;AdditionalScratchesTiling;6;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;31;-857.5529,-147.928;Float;True;Property;_BaseScratches;BaseScratches;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;46;-922.3497,202.0492;Float;True;Spherical;Object;False;AdditionalScratches;_AdditionalScratches;white;1;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;AdditionalScratches;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCGrayscale;32;-553.2697,-152.1273;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;35;-557.8955,-66.4472;Float;False;Property;_BaseScratchesCutoff;BaseScratchesCutoff;5;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;38;-505.0938,282.5452;Float;False;Property;_AdditionalScratchesCutoff;AdditionalScratchesCutoff;4;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-465.838,379.3113;Float;False;Property;_AdditionalscratchesOpacity;AdditionalscratchesOpacity;3;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCGrayscale;41;-500.4679,196.8651;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-549.0568,15.11046;Float;False;Constant;_Float0;Float 0;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;40;-244.4186,193.7971;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;34;-333.3404,-155.1953;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;-119.6774,-346.6268;Float;False;Property;_Tiling;Tiling;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-63.06543,67.07303;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;50;109.2668,-289.8613;Float;True;Spherical;Object;False;overlayDiffuse2;_overlayDiffuse2;white;0;None;Mid Texture 4;_MidTexture4;white;-1;None;Bot Texture 4;_BotTexture4;white;-1;None;OverlayDiffuse;False;8;0;SAMPLER2D;;False;5;FLOAT;1.0;False;1;SAMPLER2D;;False;6;FLOAT;0.0;False;2;SAMPLER2D;;False;7;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;45;85.2182,-5.167717;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;52;511.8148,-393.6763;Float;False;Property;_Tint;Tint;8;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;63;395.3915,95.93929;Float;False;Property;_MetalSmoothness;MetalSmoothness;10;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;53;590.2307,-222.5873;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;239.2046,22.2462;Float;False;BaseToCoverAlpha;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;48;136.8876,184.0178;Float;False;37;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;423.0239,183.4422;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;49;538.9725,-48.63144;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,1,1,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;54;352.1324,294.9559;Float;True;Property;_AmbientOcclusion;AmbientOcclusion;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;764.105,43.95625;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NewWeaponSkins;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;3;47;0
WireConnection;32;0;31;0
WireConnection;41;0;46;0
WireConnection;40;0;41;0
WireConnection;40;1;38;0
WireConnection;40;2;39;0
WireConnection;34;0;32;0
WireConnection;34;1;35;0
WireConnection;34;2;36;0
WireConnection;44;0;34;0
WireConnection;44;1;40;0
WireConnection;50;3;51;0
WireConnection;45;0;44;0
WireConnection;53;0;52;0
WireConnection;53;1;50;0
WireConnection;53;2;50;4
WireConnection;37;0;45;0
WireConnection;62;0;48;0
WireConnection;62;1;63;0
WireConnection;49;0;53;0
WireConnection;49;2;37;0
WireConnection;0;0;49;0
WireConnection;0;3;48;0
WireConnection;0;4;62;0
WireConnection;0;5;54;0
ASEEND*/
//CHKSM=8594BB0B3490953C4FFCAE86B0647D1A1044ABB3