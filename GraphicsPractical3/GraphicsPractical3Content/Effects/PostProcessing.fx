//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define RADIUS  7
#define KERNEL_SIZE (RADIUS * 2 + 1)

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// SpriteBatch will set this texture. It is the screen that has to be post-processed.
texture ScreenTexture;

// Sampler for the texture.
sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

// The value for gamma correction.
float Gamma;

// Turn grayscale on or off.
bool IsGrayscale;
// Turn gaussian blur on or off.
bool IsGaussianBlurred;

// Gaussian blur variables.
float weights[KERNEL_SIZE];
float2 offsets[KERNEL_SIZE];

//------------------------------------------ Functions ------------------------------------------

float4 CalculateGamma(float4 color)
{
	// Correct by the given gamma value. The equations are simplified, because the colors range from 0-1.
	float4 outputColor = color;
		outputColor.r = pow(abs(color.r), 1.0 / Gamma);
	outputColor.g = pow(abs(color.g), 1.0 / Gamma);
	outputColor.b = pow(abs(color.b), 1.0 / Gamma);

	return outputColor;
}

//----------------------------------------- Pixel shader ----------------------------------------

float4 PostProcessingPixelShader(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	// Look up the texture color.
	float4 color = tex2D(TextureSampler, TextureCoordinate);

	// Apply the gamma correction.
	float4 outputColor = CalculateGamma(color);

	if (IsGrayscale)
	{
		// Apply the grayscale.
		float Y = outputColor.r * 0.3 + outputColor.g * 0.59 + outputColor.b * 0.11;
		outputColor.r = Y;
		outputColor.g = Y;
		outputColor.b = Y;
	}

	if (IsGaussianBlurred)
	{
		for (int i = 0; i < KERNEL_SIZE; ++i)
			color += tex2D(TextureSampler, TextureCoordinate + offsets[i]) * weights[i];
	}

	return outputColor;
}

//--------------------------------- Technique: PostProcessing ---------------------------------

technique PostProcessing
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 PostProcessingPixelShader();
	}
}
