using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicsPractical3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Often used XNA objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FrameRateCounter frameRateCounter;

        // Game objects and variables
        private Camera camera;

        // Model
        private Model model;
        private SimpleMaterial modelMaterial;

        // Quad
        private VertexPositionNormalTangentBinormalTexture[] quadVertices;
        private short[] quadIndices;
        private Matrix quadTransform;
        private Effect textureEffect;

        // Quad material
        private TextureMaterial quadMaterial;
        private Texture2D floorTexture;

        // Render target for post-processing
        private RenderTarget2D renderTarget;

        // Post-processing effect
        private Effect postprocessingEffect;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            // Create and add a frame rate counter
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }

        protected override void Initialize()
        {
            // Copy over the device's rasterizer state to change the current fillMode
            this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };
            // Set up the window
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            // Let the renderer draw and update as often as possible
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            // Flush the changes to the device parameters to the graphics card
            this.graphics.ApplyChanges();
            // Initialize the camera
            this.camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            this.IsMouseVisible = true;

            // Set the render target.
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            floorTexture = Content.Load<Texture2D>("Textures/CobblestonesDiffuse");
            // Create a SpriteBatch object.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // Load the "Simple" effect.
            Effect simpleEffect = this.Content.Load<Effect>("Effects/Simple");
            // Load the model and let it use the "Simple" effect.
            this.model = this.Content.Load<Model>("Models/femalehead");
            this.model.Meshes[0].MeshParts[0].Effect = simpleEffect;

            // Setup the quad.
            this.setupQuad();
            // Load the "Texture" effect.
            this.textureEffect = Content.Load<Effect>("Effects/Texture");

            // Setup the material.
            this.modelMaterial = new SimpleMaterial();
            // Set the ambient color.
            this.modelMaterial.AmbientColor = Color.Gray;
            // Set the ambient intensity.
            this.modelMaterial.AmbientIntensity = 0.2f;
            // Set the diffuse color.
            this.modelMaterial.DiffuseColor = Color.Gray;
            // Set the specular color.
            this.modelMaterial.SpecularColor = Color.White;
            // Set the specular intensity.
            this.modelMaterial.SpecularIntensity = 2.0f;
            // Set the specular power.
            this.modelMaterial.SpecularPower = 25.0f;

            // Load the "PostProcessing" effect.
            postprocessingEffect = Content.Load<Effect>("Effects/PostProcessing");
        }

        /// <summary>
        /// Sets up a 2 by 2 quad around the origin.
        /// </summary>
        private void setupQuad()
        {
            float scale = 500.0f;

            // Normal points up
            Vector3 quadNormal = new Vector3(0, 1, 0);

            this.quadVertices = new VertexPositionNormalTangentBinormalTexture[4];
            // Top left
            this.quadVertices[0].Position = new Vector3(-1, 0, -1);
            this.quadVertices[0].Normal = quadNormal;
            this.quadVertices[0].Tangent = new Vector3(-1, 1, -1);
            this.quadVertices[0].Binormal = new Vector3(1, 0, 1);
            this.quadVertices[0].TextureCoordinate = new Vector2(0.0f, 0.0f);
            // Top right
            this.quadVertices[1].Position = new Vector3(1, 0, -1);
            this.quadVertices[1].Normal = quadNormal;
            this.quadVertices[1].Tangent = new Vector3(1, 0, -1);
            this.quadVertices[1].Binormal = new Vector3(-1, 0, 1);
            this.quadVertices[1].TextureCoordinate = new Vector2(2.0f, 0.0f);
            // Bottom left
            this.quadVertices[2].Position = new Vector3(-1, 0, 1);
            this.quadVertices[2].Normal = quadNormal;
            this.quadVertices[2].Tangent = new Vector3(-1, 0, 1);
            this.quadVertices[2].Binormal = new Vector3(1, 0, -1);
            this.quadVertices[2].TextureCoordinate = new Vector2(0.0f, 2.0f);
            // Bottom right
            this.quadVertices[3].Position = new Vector3(1, 0, 1);
            this.quadVertices[3].Normal = quadNormal;
            this.quadVertices[3].Tangent = new Vector3(1, 0, 1);
            this.quadVertices[3].Binormal = new Vector3(-1, 0, -1);
            this.quadVertices[3].TextureCoordinate = new Vector2(2.0f, 2.0f);

            this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
            this.quadTransform = Matrix.CreateScale(scale);

            // Setup the material.
            this.quadMaterial = new TextureMaterial();
            // Set the ambient color.
            this.quadMaterial.AmbientColor = Color.White;
            // Set the ambient intensity.
            this.quadMaterial.AmbientIntensity = 0.0f;
            // Set the diffuse color.
            this.quadMaterial.DiffuseColor = Color.White;
            // Set the quad texture.
            this.quadMaterial.DiffuseTexture = this.Content.Load<Texture2D>("Textures/CobblestonesDiffuse");
            // Set the displacement factor.
            this.quadMaterial.DisplacementFactor = 1.0f;
            // Set the normal displacement texture.
            this.quadMaterial.NormalMap = Content.Load<Texture2D>("Normal Maps/CobblestonesNormal");
            // Set the specular color.
            this.quadMaterial.SpecularColor = Color.White;
            // Set the specular intensity.
            this.quadMaterial.SpecularIntensity = 0.0f;
            // Set the specular power.
            this.quadMaterial.SpecularPower = 0.0f;
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;

            // Update the window title
            this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget);

            GraphicsDevice.Clear(Color.Black);

            // Set the effect parameters.
            postprocessingEffect.CurrentTechnique = postprocessingEffect.Techniques["PostProcessing"];
            // Set the gamma value.
            // A value of 1.5 is used in the screenshot for gamma correction, 1.0 is used to apply
            // no correction.
            postprocessingEffect.Parameters["Gamma"].SetValue(1.0f);
            // Turn grayscale on or off.
            postprocessingEffect.Parameters["isGrayscale"].SetValue(false);
            // Turn Gaussian blur on or off.
            postprocessingEffect.Parameters["isGaussian"].SetValue(true);
            // Apply gamma correction.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, postprocessingEffect);

            spriteBatch.Draw(renderTarget, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawScene()
        {
            // Clear the screen in a predetermined color and clear the depth buffer.
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

            // Get the model's only mesh.
            ModelMesh mesh = this.model.Meshes[0];
            Effect simpleEffect = mesh.Effects[0];

            // Directional light directions.
            Vector3 directionalLight1 = new Vector3(-40.0f, 30.0f, 23.0f);
            Vector3 directionalLight2 = new Vector3(-27.0f, 37.0f, 32.0f);
            Vector3 directionalLight3 = new Vector3(49.0f, 27.0f, 19.0f);
            Vector3[] directionalLights = new Vector3[5];
            directionalLights[0] = directionalLight1;
            directionalLights[1] = directionalLight2;
            directionalLights[2] = directionalLight3;
            directionalLights[3] = directionalLight1;
            directionalLights[4] = directionalLight2;
            Vector4[] directionalColors = new Vector4[5];
            directionalColors[0] = Color.BlanchedAlmond.ToVector4();
            directionalColors[1] = Color.PeachPuff.ToVector4();
            directionalColors[2] = Color.Coral.ToVector4();
            directionalColors[3] = Color.CornflowerBlue.ToVector4();
            directionalColors[4] = Color.PapayaWhip.ToVector4();


            // Matrices for 3D perspective projection.
            Matrix world;
            Matrix worldInverseTransposeMatrix;

            // Set the effect parameters.
            simpleEffect.CurrentTechnique = simpleEffect.Techniques["Simple"];
            // Matrices for 3D perspective projection.
            this.camera.SetEffectParameters(simpleEffect);
            // Uniform scale.
            world = Matrix.CreateScale(0.5f);
            // Replace 'world' with 'scale' for the non-uniform scale demo.
            Matrix translate = world * Matrix.CreateTranslation(new Vector3(0, 8.5f, 0));
            simpleEffect.Parameters["World"].SetValue(translate);
            // Set world inverse transpose.
            worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
            simpleEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            // Set the light source.
            simpleEffect.Parameters["LightPositions"].SetValue(directionalLights);
            // Set the Color
            simpleEffect.Parameters["LightColors"].SetValue(directionalColors);
            // Set the view direction.
            simpleEffect.Parameters["EyePos"].SetValue(this.camera.Eye);
            // Set all the material parameters.
            this.modelMaterial.SetEffectParameters(simpleEffect);

            // Draw the model.
            mesh.Draw();

            // Set the effect parameters.
            textureEffect.CurrentTechnique = textureEffect.Techniques["Texture"];
            // Matrices for 3D perspective projection.
            this.camera.SetEffectParameters(textureEffect);
            // Uniform scale.
            world = Matrix.CreateScale(50);
            textureEffect.Parameters["World"].SetValue(world);
            // Set world inverse transpose.
            worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
            textureEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            // Set the light source.
            textureEffect.Parameters["LightPositions"].SetValue(directionalLights);
            // Set the view direction.
            textureEffect.Parameters["EyePos"].SetValue(this.camera.Eye);
            // Set all the quad material parameters.
            this.quadMaterial.SetEffectParameters(textureEffect);

            // Draw the ground texture.
            foreach (EffectPass pass in textureEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                    this.quadVertices, 0, this.quadVertices.Length,
                    this.quadIndices, 0, this.quadIndices.Length / 3,
                    VertexPositionNormalTangentBinormalTexture.VertexDeclaration);
            }
        }

        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target.
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene.
            DrawScene();

            // Drop the render target.
            GraphicsDevice.SetRenderTarget(null);
        }
    }
}
