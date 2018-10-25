using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // this shader will be used for rendering wireframe
        BasicEffect ColorShaderEffect;

        // the texture to render
        Texture2D Texture;
        
        public CircleMesh(CustomModel parentModel, Game game, string textureKey, float radius = 5f, int sides = 3) : base(parentModel, game)
        {

            // basic assign
            Radius = radius;
            Sides = sides;

            Texture = game.Content.Load<Texture2D>("textureKey");

            // create and setting up the effect settings
            ColorShaderEffect = new BasicEffect(game.GraphicsDevice);
            ColorShaderEffect.LightingEnabled = false;
            ColorShaderEffect.VertexColorEnabled = true;

            ShowWireframe = true;

            TextureShaderEffect = new BasicEffect(game.GraphicsDevice);
            TextureShaderEffect.LightingEnabled = true;
            TextureShaderEffect.VertexColorEnabled = false;

            CreateGeometry();

        }

        public override void CreateGeometry()
        {
            base.CreateGeometry();
            
            // first, the array of vertices is created
            VertexList = new VertexPositionNormalTexture[Sides + 1]; 
            /* number of sides + 1 because we need one extra for the middle
             *
             *   0 --- 1
             *  /       \
             * 5    6    2
             *  \       /
             *   4 --- 3
             * 
             * example of an hexagon Sides + 1 = 7
             */

            IndicesList = new short[3 * Sides];  // three times slides because we will render this with an indexed triangle list.
                                                 // so we need 3 indices per vertex
                                                             
            // this is the length of a unit circunference
            float pi2 = (float)Math.PI * 2;
            
            // first of all, add the middle vertex.
            // that will be the first index of the Vertex List
            VertexList[0] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Up, new Vector2(0.5f));
            // we create a middle vertex with the normal pointing upwards. 
            // we also say that the coordinates of any texture, are the middle one.

            // generate tris
            for (int i = 0; i < Sides; i++)
            {
                // we divide a unit circle circunference length by the number of sides
                float segment = pi2 / Sides; // this gives the next arc length

                float x = (float)Math.Cos(segment) * Radius;  // apply cos to the arc length, and get the x value from the arc center
                float z = (float)Math.Sin(segment) * Radius;  // same but sin

                // vector with the new position
                Vector3 newPosition = new Vector3(x, 0.0f, z);

                // vector with the corresponding uv mapping
                Vector2 uv = new Vector2(x / (2 * Radius) + 0.5f, z / (2 * Radius) + 0.5f);

                // store the vertex
                VertexList[i + 1] = new VertexPositionNormalTexture(newPosition, Vector3.Up, uv);
                
            }

            // generate the indices
            for(int i = 0; i < Sides; i++)
            {

                IndicesList[i + 0] = 0;  // the middle;
                IndicesList[i + 1] = (short)(i + 1);
                IndicesList[i + 2] = (short)(((i + 1) % (Sides + 1)) + 1);

            }


        }

    }
}
