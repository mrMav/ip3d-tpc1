using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ip3d_tpc1
{
    class CylinderMesh : CustomMesh
    {

        // define some constants for the 
        // number of sides. These will be our constrains
        const int MAX_SIDES = 10;
        const int MIN_SIDES = 3;

        // the dimensions
        public float Radius;
        public float Height;
        public int Sides;

        // the texture to render
        Texture2D Texture;

        // constructor
        public CylinderMesh(CustomModel parentModel, Game game, string textureKey, float radius = 5f, float height = 10f, int sides = 3) : base(parentModel, game)
        {

            // basic assign
            Radius = radius;
            Height = height;
            Sides = sides;

            // load and define material proeperties

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

            // draw with a triangle strip
            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, VertexList.Length + 1);

        }
        
        public override void CreateGeometry()
        {
            base.CreateGeometry();

            VertexPositionNormalTexture[] topVertices    = Utils.CreateCircleVertices(Radius, Sides);  // see function source for full details
            VertexPositionNormalTexture[] bottomVertices = Utils.CreateCircleVertices(Radius, Sides);

            // now we will create an unique vertexlist
            // we must keep in mind that we do not want the middle vertex of the circle
            VertexList = new VertexPositionNormalTexture[topVertices.Length + bottomVertices.Length - 2 + 2];  // - 2 center vertices
                                                                                                               // + 2 vertices at the end
                                                                                                               // equal to the first two.
                                                                                                               // it must have those two extra for the uv 
                                                                                                               // for the uv mapping
                                                                                                               // (i know it looks silly, but i like to keep it
                                                                                                               // clear in the code)

            for (int i = 0; i < topVertices.Length - 1; i++)
            {
                // we also need to raise the height of the top vertices:
                topVertices[i + 1].Position.Y = Height;

                // now we will change the normal.
                // by default, the function that creates the vertices gives
                // us the normals facing upwards.
                // fortunally for us, this is trivial:
                /* 
                 *   0 --- 1
                 *  /       \
                 * 5    6    2
                 *  \       /
                 *   4 --- 3
                 * 
                 * the normals will be the point is self after being normalized!
                 * 
                 */

                topVertices[i + 1].Normal = Vector3.Normalize(topVertices[i + 1].Position);
                bottomVertices[i + 1].Normal = Vector3.Normalize(topVertices[i + 1].Position);

                // we must change the uv mapping as well.
                // this will map a texture like this:
                /*
                 *  (0,0) - (0.5,0) - (1,0)   <- top vertices uv coordinates
                 *    |   /   |     /   |
                 *    |  /    |    /    |
                 *    | /     |   /     |
                 *  (0,1) - (0.5,1) - (1,1)   <- bottom vertices uv coordinates
                 */

                topVertices[i + 1].TextureCoordinate    = new Vector2(1f - (float)i / (float)(topVertices.Length - 1), 0);
                bottomVertices[i + 1].TextureCoordinate = new Vector2(1f - (float)i / (float)(topVertices.Length - 1), 1);

                // add the vertices to the defacto array
                VertexList[2 * i] = topVertices[i + 1];
                VertexList[2 * i + 1] = bottomVertices[i + 1];

            }

            // we will add the repeating vertices now, with the different uv's
            VertexPositionNormalTexture a = VertexList[0];
            VertexPositionNormalTexture b = VertexList[1];

            a.TextureCoordinate.X = 0;

            b.TextureCoordinate.X = 0;
            b.TextureCoordinate.Y = 1;

            VertexList[VertexList.Length - 2] = a;
            VertexList[VertexList.Length - 1] = b;

            // adjust scale. just a preference to change when using different textures
            ScaleUVs(new Vector2(1f, 1.5f));

            /*
             * Now we will create the indices.
             * Because of the way we built the vertexlist
             * our indices will be in order. so I don't know 
             * which would be faster in this case.
             * 
             * We will draw the cylinder with a trianglestrip
             * 
             */

            IndicesList = new short[VertexList.Length];
            
            // generate the indices
            for (short i = 0; i < IndicesList.Length; i++)
            {

                IndicesList[i] = i;

            }

            // set the buffers
            VertexBuffer = new VertexBuffer(Game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, VertexList.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalTexture>(VertexList);

            IndexBuffer = new IndexBuffer(Game.GraphicsDevice, typeof(short), IndicesList.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData<short>(IndicesList);

        }

    }

}
