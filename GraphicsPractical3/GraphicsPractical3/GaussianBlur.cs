using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical3
{
    public class GaussianBlur
    {
        private Game game;
        private Effect effect;

        private int radius;
        private float amount;
        private float sigma;
        private float[] kernel;

        private Vector2[] offsetsHorizontal;
        private Vector2[] offsetsVertical;

        public GaussianBlur()
        {
        }

        public GaussianBlur(Game game)
        {
            this.game = game;
            effect = game.Content.Load<Effect>("Effects/PostProcessing");
        }

        public void ComputeKernel(int blurRadius, float blurAmount)
        {
            radius = blurRadius;
            amount = blurAmount;

            kernel = new float[radius * 2 + 1];
            sigma = radius / amount;

            // Variables used in the calculation of the kernel values.
            float sigmaSquare = 2.0f * sigma * sigma;
            float sigmaRoot = (float)Math.Sqrt(sigmaSquare * Math.PI);
            float total = 0.0f;
            float distance = 0.0f;
            int index = 0;

            // Calculate the kernel values.
            for (int i = -radius; i <= radius; ++i)
            {
                distance = i * i;
                index = i + radius;
                kernel[index] = (float)Math.Exp(-distance / sigmaSquare) / sigmaRoot;
                total += kernel[index];
            }

            // Normalize the kernel.
            for (int i = 0; i < kernel.Length; ++i)
                kernel[i] /= total;
        }

        public void ComputeOffsets(float textureWidth, float textureHeight)
        {
            offsetsHorizontal = new Vector2[radius * 2 + 1];
            offsetsVertical = new Vector2[radius * 2 + 1];

            int index = 0;
            float xOffset = 1.0f / textureWidth;
            float yOffset = 1.0f / textureHeight;

            // Set the horizontal and vertical offsets.
            for (int i = -radius; i <= radius; ++i)
            {
                index = i + radius;
                offsetsHorizontal[index] = new Vector2(i * xOffset, 0.0f);
                offsetsVertical[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        public Texture2D PerformGaussianBlur(Texture2D sourceTexture,
                                             RenderTarget2D renderTarget,
                                             SpriteBatch spriteBatch)
        {
            Texture2D outputTexture = null;
            Rectangle sourceRect = new Rectangle(0, 0, sourceTexture.Width, sourceTexture.Height);
            Rectangle destinationRect = new Rectangle(0, 0, renderTarget.Width, renderTarget.Height);

            // Apply the horizontal Gaussian blur pass.
            game.GraphicsDevice.SetRenderTarget(renderTarget);

            effect.CurrentTechnique = effect.Techniques["PostProcessing"];
            effect.Parameters["weights"].SetValue(kernel);
            effect.Parameters["colorMapTexture"].SetValue(sourceTexture);
            effect.Parameters["offsets"].SetValue(offsetsHorizontal);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(sourceTexture, destinationRect, Color.White);
            spriteBatch.End();

            // Apply the vertical Gaussian blur pass.
            game.GraphicsDevice.SetRenderTarget(renderTarget);
            outputTexture = (Texture2D)renderTarget;

            effect.Parameters["colorMapTexture"].SetValue(outputTexture);
            effect.Parameters["offsets"].SetValue(offsetsVertical);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(outputTexture, destinationRect, Color.White);
            spriteBatch.End();

            // Return the blurred texture.
            game.GraphicsDevice.SetRenderTarget(null);
            outputTexture = (Texture2D)renderTarget;

            return outputTexture;
        }

        public int Radius
        {
            // Return the radius of the kernel.
            get { return radius; }
        }

        public float Amount
        {
            // Return the blur amount.
            get { return amount; }
        }

        public float Sigma
        {
            // Return the standard deviation of the Gaussian blur.
            get { return sigma; }
        }

        public float[] Kernel
        {
            // Return the Gaussian blur filter kernel.
            get { return kernel; }
        }

        public Vector2[] TextureOffsetsX
        {
            // Return the offsets for the horizontal blur pass.
            get { return offsetsHorizontal; }
        }

        public Vector2[] TextureOffsetsY
        {
            // Return the offsets for the vertical blur pass.
            get { return offsetsVertical; }
        }
    }
}
