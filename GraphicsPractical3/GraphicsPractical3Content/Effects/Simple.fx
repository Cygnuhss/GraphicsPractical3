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

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
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
	float4 Position : POSITION;
	float4 Color: COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
	float4 Normal : TEXCOORD1;
	// Storing the 3D position in TEXCOORD2, because the POSITION0 semantic cannot be used in the pixel shader.
	float4 Position3D : TEXCOORD2;
};

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct.
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform.
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// Relay the input normals.
	output.Normal = input.Normal;
	// Relay the texture coordinates.
	output.TextureCoordinate = input.TextureCoordinate;

	// Use this line for NormalColor and ProceduralColor. Leaving it will not cause harm, as the color
	// will later be overridden by the diffuse color.
	output.Color = input.Normal;
	// Relay the POSITION0 information to the TEXCOORD2 semantic, for use in the pixel shader.
	output.Position3D = input.Position;

	// Correctly handle the normals with non-uniform scaling.
	float4 normal = mul(input.Normal, WorldInverseTranspose);
	normal = normalize(normal);
	output.Normal = normal;
	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	float4 color = input.Color;

	// The ambient color is the same everywhere: a predefined color at a certain intensity.
	float4 ambient = AmbientColor * AmbientIntensity;

	for (int i = 0; i < MAX_LIGHTS; i++)
	{
		float direction = normalize(float4(LightPositions[i], 1) - input.Position3D);
		float attenuation = 1.0f - saturate((direction - 11.0f) / 17.0f);
		//if (LightPositions[i] != )
		//{
		// The color is proportional to the angle between the surface normal and direction to the light source.
		// Surfaces pointing away from the light do not receive any light.
		float lightIntensity = max(0, dot(input.Normal, direction));
		// Take the diffuse color and intensity into account.
		color += saturate(attenuation * LightColors[i] * DiffuseColor * DiffuseIntensity * lightIntensity);
		//}
	}

	/*for (int i = 0; i < MAX_LIGHTS; i++)
	{
		// The light vector l is the direction from the location to the light.
		float3 l = -LightPositions[i];
		// The normal vector n denotes the normal of the surface.
		float3 n = input.Normal;
		// The view vector v is the vector from the camera to the fragment.
		float3 v = normalize(EyePos - input.WorldPos);
		// Calculate the half vector, which is the bisector of the angle between the view vector v and light vector l.
		float3 h = normalize(v + l);
		float4 specular = SpecularColor * SpecularIntensity * pow(saturate(dot(n, h)), SpecularPower);

		// Add the ambient and specular light to the already calculated diffuse light and texture.
		color = saturate(color + ambient + specular);
	}*/

	return color;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 SimpleVertexShader();
		PixelShader = compile ps_3_0 SimplePixelShader();
	}
}
