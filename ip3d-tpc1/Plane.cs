﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ip3d_tpc1
{
    /// <summary>
    /// The plane class creates a plane primitive in the XZ plane.
    /// There are options to define the size and number of subdivisions.
    /// </summary>
    class Plane : CustomMesh
    {

        // basic dimensions
        public float Width { get; private set; }
        public float Depth { get; private set; }
        public int XSubs { get; private set; }
        public int ZSubs { get; private set; }

        public float SubWidth { get; private set; }
        public float SubHeight { get; private set; }
        
        // reference to the texture
        Texture2D DiffuseMap;
        
        // constructor 
        public Plane(CustomModel parent, Game game, string textureKey, float width = 10f, float depth = 10f, int xSubs = 1, int zSubs = 1) : base(parent, game)
        {

            // some basic assigning is done here

            Width = width;
            Depth = depth;
            XSubs = xSubs;
            ZSubs = zSubs;

            DirtyGeometry = false;
            
            DiffuseMap = Game.Content.Load<Texture2D>(textureKey);

            // in this phase, I want to stop everything if the texture is null
            if (DiffuseMap == null)
            {
                throw new Exception($"diffuseMap is null. key {textureKey} not found in content.");
            }

            // properties for the texture shader
            TextureShaderEffect = new BasicEffect(game.GraphicsDevice);
            TextureShaderEffect.VertexColorEnabled = false;
            TextureShaderEffect.PreferPerPixelLighting = true;

            TextureShaderEffect.TextureEnabled = true;
            TextureShaderEffect.Texture = DiffuseMap;  // assign out texture

            TextureShaderEffect.LightingEnabled = true;
            TextureShaderEffect.AmbientLightColor = new Vector3(0.15f);
            TextureShaderEffect.DirectionalLight0.Enabled = true;
            TextureShaderEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            TextureShaderEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1, -1));
            TextureShaderEffect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);            
            TextureShaderEffect.SpecularPower = 8f;

            // create the geometry
            CreateGeometry();

        }

        public override void Draw(GameTime gameTime)
        {

            Game.GraphicsDevice.Indices = IndexBuffer;
            Game.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            
            // render the geometry using a triangle list
            // we only use one pass of the shader, but we could have more.
            TextureShaderEffect.CurrentTechnique.Passes[0].Apply();

            // send a draw call to the gpu, using a triangle list as a primitive. 
            // I could be using a triangle strip, but nowadays computers have tons of memory
            // and I'd rather keep the draw calls at a minimum and save the cpu the struggle
            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndicesList.Length / 3);

        }

        public override void CreateGeometry()
        {

            // creates the geometry, whithout taking into account any heightmap!
            // if we want that, next, we call the setheight function


            // calculate the number of vertices we will have in each side
            int nVerticesWidth = XSubs + 1;
            int nVerticesDepth = ZSubs + 1;

            // total vertices and total indices
            int verticesCount = nVerticesWidth * nVerticesDepth;
            int indicesCount = XSubs * ZSubs * 6;  // 6 because a subdivision is made of 2 triagnles, each one with 3 vertices(indexed)

            // the array of vertices and indices are created
            VertexList = new VertexPositionNormalTexture[verticesCount];
            IndicesList = new short[indicesCount];

            // the size of each subdivision
            SubWidth = Width / XSubs;
            SubHeight = Depth / ZSubs;

            int currentVertice = 0;

            // create the vertices
            for (int z = 0; z <= ZSubs; z++)
            {
                for (int x = 0; x <= XSubs; x++)
                {

                    // we will put the 0, 0 on the center of the plane
                    // because of that we will translate every vertex halfwidth and halfdepth
                    // this ensures our plane has the origin in the middle

                    float xx = x * SubWidth - Width / 2;
                    float zz = z * SubHeight - Depth / 2;

                    // texture coordinates, must be normalized
                    float u = (float)x / ((float)XSubs);
                    float v = (float)z / ((float)ZSubs);

                    // create the vertice, and increment to the next one
                    VertexList[currentVertice++] = new VertexPositionNormalTexture(new Vector3(xx, 0, zz), Vector3.Zero, new Vector2(u, v));

                }
            }

            // create indices
            int currentIndice = 0;
            int currentSubDivison = 0;
            for (int z = 0; z < ZSubs; z++)
            {
                for (int x = 0; x < XSubs; x++)
                {
                    /* calculate positions in the array
                     * 
                     *  1---2
                     *  | / |
                     *  4---3
                     *  
                     */

                    int vert1 = currentSubDivison + z;
                    int vert2 = vert1 + 1;
                    int vert3 = vert2 + nVerticesWidth;
                    int vert4 = vert3 - 1;

                    // first tri
                    IndicesList[currentIndice++] = (short)vert1;
                    IndicesList[currentIndice++] = (short)vert2;
                    IndicesList[currentIndice++] = (short)vert4;

                    // secon tri
                    IndicesList[currentIndice++] = (short)vert4;
                    IndicesList[currentIndice++] = (short)vert2;
                    IndicesList[currentIndice++] = (short)vert3;

                    currentSubDivison++;

                }

            }

            CalculateNormals();
            
            // create the buffers, and attach the corresponding data:

            VertexBuffer = new VertexBuffer(Game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, VertexList.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalTexture>(VertexList);

            IndexBuffer = new IndexBuffer(Game.GraphicsDevice, typeof(short), IndicesList.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData<short>(IndicesList);

        }

        /// <summary>
        /// This method uses an algorithm that is able to calculate the vertex normals of a subdivided and deformed plane.
        /// </summary>
        private void CalculateNormals()
        {
            /* in order to calculte the normals we need to understand that we won't be able to just loop through all the vertices
             * the normals calculation will be based on the vertices neighbours, so, it won't be equal to calculate a corner and a middle vertice.
             * lets ilustrate this:
             * 
             * 0 - 1 - 2
             * | / | / |
             * 3 - 4 - 5
             * | / | / |
             * 6 - 7 - 8
             * 
             * above, we have a basic mesh with the vertices numbered.
             * in order to calculate each vertex normal, we will need to get the neighbours.
             * 
             * for corner vertices, we will have to get the three vertices surrounding it. Let's take vertice 0 as an example:
             *     
             *     -> the vertex 0 normal, will be influenced by vertices 1 and 3, so we can say that vertex 0 normal is equal to
             *     -> the cross product of 0 to 3 and 0 to 1.
             *     -> in code, this will look like this:
             *
             * tip: make the cross product vectors from a clockwise motion, otherwise, the vector will be inverted.
             */

            Vector3 Current = VertexList[0].Position;
            Vector3 Left = VertexList[1].Position;
            Vector3 Bottom = VertexList[XSubs + 2].Position;

            Vector3 TopLeftCornerNormal = Vector3.Cross((Current - Bottom), (Current - Left));

            /*
             * now that we have the normal vector, we should normalize it and then assign it.
             */

            VertexList[0].Normal = Vector3.Normalize(TopLeftCornerNormal);

            /*
             * we will also add it to a normal list
             * because we want to see them displayed when debugging
             */

            int currentNormalList = 0;
            
            /*
             * and that's it!
             * now we will do it for the other corners.
             */

            // top right
            Current = VertexList[XSubs].Position;
            Left = VertexList[XSubs - 1].Position;
            Bottom = VertexList[XSubs * 2].Position;

            Vector3 TopRightCornerNormal = Vector3.Cross((Current - Left), (Current - Bottom));

            VertexList[XSubs].Normal = Vector3.Normalize(TopRightCornerNormal);

            // bottom right
            Current = VertexList[VertexList.Length - 1].Position;
            Vector3 Right = VertexList[VertexList.Length - 1 - 1].Position;
            Vector3 Top = VertexList[VertexList.Length - 1 - (XSubs + 1)].Position;

            Vector3 BottomRightCorner = Vector3.Cross((Right - Current), (Current - Top));

            VertexList[VertexList.Length - 1].Normal = Vector3.Normalize(BottomRightCorner);

            // bottom left
            Current = VertexList[VertexList.Length - 1 - XSubs].Position;
            Right = VertexList[VertexList.Length - 1 - XSubs + 1].Position;
            Top = VertexList[VertexList.Length - 1 - XSubs - (XSubs + 1)].Position;

            Vector3 BottomLeftCorner = Vector3.Cross((Current - Top), (Right - Current));

            VertexList[VertexList.Length - 1 - XSubs].Normal = Vector3.Normalize(BottomLeftCorner);
            
            /*
             * now that we have the corners, we will focus on the edges.
             * in order to calculate the edges, again, we need to know which influences 
             * the vertex normal is going to have.
             * let's have a look at our little drawing again:
             * 
             * 0 - 1 - 2
             * | / | / |
             * 3 - 4 - 5
             * | / | / |
             * 6 - 7 - 8
             * 
             * we will influence the normal of the vertex 1, by three vertices. 0, 2 and 4.
             * the vertex 3, will be influenced by vertices 0, 4 and 6, and so on.
             * 
             * with this in mind, we will have to create two loops, one in x and one in z
             * because the logic will be different. We can however, make the left and right 
             * in the same loop for example.
             * 
             * let's code this then:
             */

            for (int x = 1; x < XSubs; x++)
            {
                /* notice that we skip the first and last vertices
                 * because we already calculated the normal for that
                 */

                int currentVertexIndex = x;

                /* we will now grab the neighbours of the current vertex of the top edge
                 * 
                 * 0 - 1 - 2
                 *     |
                 *     4
                 */

                Current = VertexList[currentVertexIndex].Position;
                Left = VertexList[currentVertexIndex - 1].Position;
                Right = VertexList[currentVertexIndex + 1].Position;
                Bottom = VertexList[currentVertexIndex + XSubs + 1].Position;

                Vector3 CurrentTopNormal = Vector3.Cross(Current - Left, Current - Bottom) + Vector3.Cross(Current - Bottom, Current - Right);

                VertexList[currentVertexIndex].Normal = Vector3.Normalize(CurrentTopNormal);

                /* bottom edge
                 * 
                 *     4
                 *     |
                 * 6 - 7 - 8
                 */

                currentVertexIndex = x + ((XSubs + 1) * ZSubs);

                Current = VertexList[currentVertexIndex].Position;
                Left = VertexList[currentVertexIndex - 1].Position;
                Right = VertexList[currentVertexIndex + 1].Position;
                Top = VertexList[currentVertexIndex - (XSubs + 1)].Position;

                Vector3 CurrentBottomNormal = Vector3.Cross(Current - Top, Current - Left) + Vector3.Cross(Current - Right, Current - Top);

                VertexList[currentVertexIndex].Normal = Vector3.Normalize(CurrentBottomNormal);

            }

            /*
             * now we will the same thing for the sides
             */
            for (int z = 1; z < ZSubs; z++)
            {

                /* left edge
                 * 
                 * 0 
                 * | 
                 * 3 - 4
                 * | 
                 * 6 
                */

                int currentVertexIndex = z * (XSubs + 1);

                Current = VertexList[currentVertexIndex].Position;
                Top = VertexList[currentVertexIndex - XSubs - 1].Position;
                Right = VertexList[currentVertexIndex + 1].Position;
                Bottom = VertexList[currentVertexIndex + XSubs + 1].Position;

                Vector3 CurrentLeftNormal = Vector3.Cross(Current - Right, Current - Top) + Vector3.Cross(Current - Bottom, Current - Right);

                VertexList[currentVertexIndex].Normal = Vector3.Normalize(CurrentLeftNormal);

                /* right edge
                 * 
                 *     2 
                 *     | 
                 * 4 - 5
                 *     | 
                 *     8 
                */

                currentVertexIndex = currentVertexIndex + XSubs;

                Current = VertexList[currentVertexIndex].Position;
                Top = VertexList[currentVertexIndex - XSubs - 1].Position;
                Bottom = VertexList[currentVertexIndex + XSubs + 1].Position;
                Left = VertexList[currentVertexIndex - 1].Position;

                Vector3 CurrentRightNormal = Vector3.Cross(Current - Top, Current - Left) + Vector3.Cross(Current - Left, Current - Bottom);

                VertexList[currentVertexIndex].Normal = Vector3.Normalize(CurrentRightNormal);

            }

            /*
             * few, this code is piling up, right?!
             * 
             * there is one last loop we should build.
             * this last loop will iterate through the rest of the vertices,
             * think of it as a filler. We lined up the outside edges
             * and now we are filling the inside.
             * 
             * each vertex will be influenced by the four neighbours
             *     1
             *     |
             * 3 - 4 - 5
             *     | 
             *     7 
             * 
             */

            for (int z = 1; z < ZSubs; z++)
            {
                for (int x = 1; x < XSubs; x++)
                {

                    int currentVertexIndex = z * (XSubs + 1) + x;

                    Current = VertexList[currentVertexIndex].Position;
                    Top = VertexList[currentVertexIndex - XSubs - 1].Position;
                    Right = VertexList[currentVertexIndex + 1].Position;
                    Bottom = VertexList[currentVertexIndex + XSubs + 1].Position;
                    Left = VertexList[currentVertexIndex - 1].Position;

                    Vector3 CurrentNormal = Vector3.Cross(Current - Top, Current - Left) +
                                                Vector3.Cross(Current - Right, Current - Top) +
                                                Vector3.Cross(Current - Bottom, Current - Right) +
                                                Vector3.Cross(Current - Left, Current - Bottom);

                    VertexList[currentVertexIndex].Normal = Vector3.Normalize(CurrentNormal);

                }
            }

            /*
             * and that is it, simple! *cough, cough*
             */

        }

    }

}