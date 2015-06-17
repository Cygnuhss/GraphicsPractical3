using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsPractical3
{
    public struct VertexPositionNormalTangentBinormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 Tangent;
        public Vector3 Binormal;

        public static readonly VertexElement[] VertexElements = new VertexElement[] 
        { 
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), 
            new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0), 
            new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0) 
        };

        public static readonly int SizeInBytes = sizeof(float) * (3 + 3 + 2 + 3 + 3);

        public static readonly VertexDeclaration VertexDeclaration =
            new VertexDeclaration(VertexPositionNormalTangentBinormalTexture.VertexElements);

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormalTangentBinormalTexture.VertexDeclaration; }
        }
    }
}
