﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ip3d_tpc1
{
    class CircleMesh : CustomMesh
    {

        // define some constants for the 
        // number of sides. These will be our constrains
        const int MAX_SIDES = 10;
        const int MIN_SIDES = 3;

        // the dimensions
        public float Radius;
        public int Sides;

        // the effect(shader) to apply when rendering
        // we will use Monogame built in BasicEffect for the purpose
        BasicEffect TextureShaderEffect;
        
        // the texture to render
        Texture2D Texture;
        
        public CircleMesh(CustomModel parentModel, Game game, string textureKey, float radius = 5f, int sides = 3) : base(parentModel, game)
        {

            // basic assign
            Radius = radius;
            Sides = sides;

            Texture = game.Content.Load<Texture2D>(textureKey);
            
            TextureShaderEffect = new BasicEffect(game.GraphicsDevice);
            TextureShaderEffect.LightingEnabled = false;
            TextureShaderEffect.VertexColorEnabled = false;
            TextureShaderEffect.TextureEnabled = true;
            TextureShaderEffect.Texture = Texture;

            CreateGeometry();

        }

        public override void Draw(GameTime gameTime)
        {

            Game.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Game.GraphicsDevice.Indices = IndexBuffer;

            TextureShaderEffect.CurrentTechnique.Passes[0].Apply();

            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexList.Length);

        }

        // updates the effect matrices
        public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
        {
            TextureShaderEffect.Projection = projectionTransform;
            TextureShaderEffect.View = viewTransform;
            TextureShaderEffect.World = WorldTransform;
        }

        public override void CreateGeometry()
        {
            base.CreateGeometry();

            // first, the array of vertices is created
            VertexList = Utils.CreateCircleVertices(Radius, Sides);  // see function source for full details

            IndicesList = new short[3 * Sides];  // three times slides because we will render this with an indexed triangle list.
                                                 // so we need 3 indices per vertex
            
            // generate the indices
            for(int i = 0; i < Sides; i++)
            {

                // making the triangle winding clockwise

                IndicesList[3 * i + 0] = 0;  // the middle;
                IndicesList[3 * i + 1] = (short)(i + 1);  // this one will never need to repeat
                IndicesList[3 * i + 2] = (short)(((i + 2) - 1) % Sides + 1); // this gives the next and repeats if necessary

            }

            // set the buffers
            VertexBuffer = new VertexBuffer(Game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, VertexList.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalTexture>(VertexList);

            IndexBuffer = new IndexBuffer(Game.GraphicsDevice, typeof(short), IndicesList.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData<short>(IndicesList);

        }

    }

}
