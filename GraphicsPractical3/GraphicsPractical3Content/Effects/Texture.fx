//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define MAX_LIGHTS 5

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World;
// Matrix for inverse transpose of world. Used for correct normals calculations.
float4x4 WorldInverseTranspose;

// Variables for ambient lighting.
float4 AmbientColor;
float AmbientIntensity;

// Array of lights.
float3 LightPositions[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];

// Variables for diffuse (Lambertian) lighting.
float4 DiffuseColor;
// This intensity approaches the lighting as in the assignment.
float DiffuseIntensity = 0.5;

// Variables for specular (Blinn-Phong) lighting.
float4 SpecularColor;
float SpecularIntensity;
float SpecularPower;
float3 EyePos;

// Variables for texturing.
bool HasTexture;
texture DiffuseTexture;
sampler2D textureSampler = sampler_state
{
	Texture = (DiffuseTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

// Variables for normal maps.
bool HasNormalMap;
float DisplacementFactor;
texture NormalMap;
sampler2D displacementSampler = sampler_state {
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// The output of the vertex shader. After being passed through the interpolator/rasterizer it is also 
// the input of the pixel shader. 
// Note 1: The values that you pass into this struct in the vertex shader are not the same as what 
// you get as input for the pixel shader. A vertex shader has a single vertex as input, the pixel 
// shader has 3 vertices as input, and lets you determine the color of each pixel in the triangle 
// defined by these three vertices. Therefore, all the values in the struct that you get as input for 
// the pixel shaders have been linearly interpolated between there three vertices!
// Note 2: You cannot use the data with the POSITION0 semantic in the pixel shader.
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color: COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
	float4 Normal : TEXCOORD1;
	float3 Tangent : TEXCOORD2;
	float3 Binormal : TEXCOORD3;
	// Storing the 3D position in TEXCOORD4, because the POSITION0 semantic cannot be used in the pixel shader.
	float3 WorldPos : TEXCOORD4;
};

//--------------------------------------- Technique: Texture ---------------------------------------

VertexShaderOutput TextureVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct.
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform.
	float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

	// Relay the input normal, tangent and binormal.
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose));
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));
	// Relay the texture coordinates.
	output.TextureCoordinate = input.TextureCoordinate;

	// Use this line for NormalColor and ProceduralColor. Leaving it will not cause harm, as the color
	// will later be overridden by the diffuse color.
	output.Color = input.Normal;
	// Relay the POSITION0 information to the TEXCOORD2 semantic, for use in the pixel shader.
	output.WorldPos = input.Position;

	return output;
}

float4 TexturePixelShader(VertexShaderOutput input) : COLOR0
{
	// Calculate the normal, including the information in the displacement map.
	float3 displacement = DisplacementFactor * (tex2D(displacementSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 displacementNormal = input.Normal + (displacement.x * input.Tangent + displacement.y * input.Binormal);
	displacementNormal = normalize(displacementNormal);

	// Calculate the diffuse light component with the displacement map normal.
	float diffuseIntensity = dot(normalize(-LightPositions[0]), displacementNormal);
	if (diffuseIntensity < 0)
		diffuseIntensity = 0;

	// Calculate the specular light component with the displacement map normal.
	float3 light = normalize(LightPositions[0]);
		float3 r = normalize(2 * dot(light, displacementNormal) * displacementNormal - light);
		float3 v = normalize(mul(normalize(EyePos), World));
		float dotProduct = dot(r, v);

	float4 specular = SpecularIntensity * SpecularColor * max(pow(abs(dotProduct), SpecularPower), 0) * diffuseIntensity;

		// Calculate the texture color
		float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
		textureColor.a = 1;

	// Combine all of these values into one (including the ambient light).
	float4 color = saturate(textureColor * (diffuseIntensity)+AmbientColor * AmbientIntensity + specular);

		return color;
}

technique Texture
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TextureVertexShader();
		PixelShader = compile ps_2_0 TexturePixelShader();
	}
}
