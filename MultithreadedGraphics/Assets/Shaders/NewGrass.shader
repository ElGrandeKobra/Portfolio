// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/NewGrass" {
 
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
 
SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 700
    ZTest LEqual
CGPROGRAM
#pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
 
sampler2D _MainTex;
//fixed4 _Color;
static const fixed _ShakeTime = .175;
static const fixed _ShakeWindspeed = .05;   
static const fixed4 _waveXSize = fixed4(0.048, 0.06, 0.24, 0.096);
static const fixed4 _waveZSize = fixed4 (0.024, .08, 0.08, 0.2);
static const fixed4 waveSpeed = fixed4 (1.2, 2, 1.6, 4.8);

static const fixed4 _waveXmove = fixed4(0.024, 0.04, -0.12, 0.096);
static const fixed4 _waveZmove = fixed4 (0.006, .02, -0.02, 0.1);
static const fixed4 sin7 = fixed4(1, -0.16161616, 0.0083333, -0.00019841);
static const fixed4 cos8 = fixed4(-0.5, 0.041666666, -0.0013888889, 0.000024801587);

struct Input {
    float2 uv_MainTex;
};
 
void FastSinCos (fixed4 val, out fixed4 s, out fixed4 c) {
    val = val * 6.408849 - 3.1415927;
    fixed4 r5 = val * val;
    fixed4 r6 = r5 * r5;
    fixed4 r7 = r6 * r5;
    fixed4 r8 = r6 * r5;
    fixed4 r1 = r5 * val;
    fixed4 r2 = r1 * r5;
    fixed4 r3 = r2 * r5;
   // float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
    //float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587};
    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}
 
 
void vert (inout appdata_full v) {
     
    const float _WindSpeed  = (_ShakeWindspeed  +  v.color.g );    

   
    fixed4 waves;
    waves = v.vertex.x * _waveXSize;
    waves += v.vertex.z * _waveZSize;
 
    waves += _Time.x * (1 - _ShakeTime * 2 - v.color.b) * waveSpeed *_WindSpeed;
 
    fixed4 s, c;
    waves = frac (waves);
    FastSinCos (waves, s,c);
 
    fixed waveAmount = (v.texcoord.y) * (v.color.a + 0.1);
    s *= waveAmount;
 
    //s *= normalize (waveSpeed);
 
    s = s * s;
    //float fade = dot (s, 1.3);
    //s = s * s;
    fixed3 waveMove = fixed3 (0,0,0);
    waveMove.x = dot (s, _waveXmove);
    waveMove.z = dot (s, _waveZmove);
    v.vertex.xz -= mul ((fixed3x3)unity_WorldToObject, waveMove).xz;
   
}
 
void surf (Input IN, inout SurfaceOutputStandard o) {
    //fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
     fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 500
    ZTest LEqual
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff addshadow
 
sampler2D _MainTex;


struct Input {
    float2 uv_MainTex;
};
 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}
 
Fallback "Transparent/Cutout/VertexLit"
}