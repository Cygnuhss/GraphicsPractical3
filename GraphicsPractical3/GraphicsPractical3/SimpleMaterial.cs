using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical3
{
    /// <summary>
    /// This struct can be used to make interaction with the effects easier.
    /// To use this, create a new material and set all the variables you want to share with the effect.
    /// Then call the SetEffectParameters to set the globals of the effect given using the parameter.
    /// Make sure to comment all the lines that set effect parameters that are currently not existing in your effect file.
    /// </summary>
    public struct SimpleMaterial
    {
        // Color of the ambient light
        public Color AmbientColor;
        // Intensity of the ambient light
        public float AmbientIntensity;
        // The color of the surface (can be ignored if texture is used, or not if you want to blend)
        public Color DiffuseColor;
        // Color of the specular highlight (mostly equal to the color of the light source)
        public Color SpecularColor;
        // The intensity factor of the specular highlight, controls its size
        public float SpecularIntensity;
        // The power term of the specular highlight, controls its smoothness
        public float SpecularPower;

        // Using this function requires all these elements to be present as top-level variables in the shader code. Comment out the ones that you don't use
        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["AmbientColor"].SetValue(this.AmbientColor.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(this.AmbientIntensity);
            effect.Parameters["DiffuseColor"].SetValue(this.DiffuseColor.ToVector4());
            effect.Parameters["SpecularColor"].SetValue(this.SpecularColor.ToVector4());
            effect.Parameters["SpecularIntensity"].SetValue(this.SpecularIntensity);
            effect.Parameters["SpecularPower"].SetValue(this.SpecularPower);
        }
    }
}
