#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// This attributes struct receives data about the mesh we're currently rendering
// Data is automatically placed in fields according to their semantic
struct Attributes
{
    float3 positionOS : POSITION; // Position in object space
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

// This struct is output by the vertex function and input to the fragment function.
// Note that fields will be transformed by the intermediary rasterization stage
struct Interpolators
{
	// This value should contain the position in clip space (which is similar to a position on screen)
	// when output from the vertex function. It will be transformed into pixel position of the current
	// fragment on the screen when read from the fragment function
    float4 positionCS : SV_POSITION;    
    float2 uv_cs : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float3 posWS : TEXCOORD2;
};

float _Scale;
// The vertex function. This runs for each vertex on the mesh.
// It must output the position on the screen each vertex should appear at,
// as well as any data the fragment function will need
    Interpolators Vertex(Attributes input)
    {
        Interpolators output;

	// These helper functions, found in URP/ShaderLib/ShaderVariablesFunctions.hlsl
	// transform object space values into world and clip space
    VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS * _Scale);
    VertexNormalInputs mormInputs = GetVertexNormalInputs(input.normal);

	// Pass position and orientation data to the fragment function
        output.positionCS = posnInputs.positionCS;
        output.uv_cs = input.uv;
        output.normalWS = mormInputs.normalWS;
        output.posWS = posnInputs.positionWS;
        return output;
    };

float4 _ColorTint;
Texture2D _MainTexture;
SamplerState sampler_MainTexture;
Texture2D _NoiseTexture;
SamplerState sampler__NoiseTexture;

float4 _uvOffset;
float _Specular;
float _Smoothness;

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input) : SV_TARGET
{
    float2 uv = input.uv_cs;
    uv.x += _uvOffset.x;
    uv.y += _uvOffset.y;
    float4 colorS = _MainTexture.Sample(sampler_MainTexture, uv);
    colorS += _NoiseTexture.Sample(sampler__NoiseTexture, uv);
    colorS *= _ColorTint;
    
    InputData lightingI = (InputData) 0;
    lightingI.normalWS = normalize(input.normalWS);
    lightingI.positionWS = input.posWS;
    lightingI.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.posWS);
    lightingI.shadowCoord = TransformWorldToShadowCoord(input.posWS);
    
    SurfaceData surfaceI = (SurfaceData) 0;
    surfaceI.albedo = colorS.rgb;
    surfaceI.alpha = colorS.a;
    surfaceI.specular = _Specular;
    surfaceI.smoothness = _Smoothness;
    
    return UniversalFragmentBlinnPhong(lightingI, surfaceI);
};