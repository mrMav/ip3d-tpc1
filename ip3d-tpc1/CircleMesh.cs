using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ip3d_tpc2
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
                
        // the texture to render
        Texture2D Texture;
        
        public CircleMesh(CustomModel parentModel, Game game, string textureKey, float radius = 5f, int sides = 3) : base(parentModel, game)
        {

            // basic assign
            Radius = radius;
            Sides = sides;

            // load and set material properties

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

            // draw with a triangle list
            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexList.Length);

        }

        public override void CreateGeometry()
        {
            base.CreateGeometry();

            // first, the array of vertices is created
            VertexList = Utils.CreateCircleVertices(Radius, Sides);  // see function source for full details

            // adjust to current texture
            // gives freedom to developers
            ScaleUVs(new Vector2(3f));

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

            // scale the uv's
            ScaleUVs(new Vector2(0.35f));

            // set the buffers
            VertexBuffer = new VertexBuffer(Game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, VertexList.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalTexture>(VertexList);

            IndexBuffer = new IndexBuffer(Game.GraphicsDevice, typeof(short), IndicesList.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData<short>(IndicesList);

        }

    }

}
