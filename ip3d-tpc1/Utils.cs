using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ip3d_tpc2
{
    public static class Utils
    {
        /// <summary>
        /// The function creates an array with vertices for a circular shape.
        /// </summary>
        /// <param name="radius">The circle radius.</param>
        /// <param name="sides">The number of sides the circle will be composed of.</param>
        /// <returns>An array containing the vertices.</returns>
        public static VertexPositionNormalTexture[] CreateCircleVertices(float radius = 1f, int sides = 6)
        {

            // first, the array of vertices is created
            VertexPositionNormalTexture[] result = new VertexPositionNormalTexture[sides + 1];
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

            // this is the length of a unit circunference
            float pi2 = (float)Math.PI * 2;

            // first of all, add the middle vertex.
            // that will be the first index of the Vertex List
            result[0] = new VertexPositionNormalTexture(Vector3.Zero, Vector3.Up, new Vector2(0.5f));
            // we create a middle vertex with the normal pointing upwards. 
            // we also say that the coordinates of any texture, are the middle one.

            // generate tris
            for (int i = 0; i < sides; i++)
            {
                // we divide a unit circle circunference length by the number of sides
                float segment = pi2 / sides * i; // this gives the next arc length

                float x = (float)Math.Cos(segment) * radius;  // apply cos to the arc length, and get the x value from the arc center
                float z = (float)Math.Sin(segment) * radius;  // same but sin

                // vector with the new position
                Vector3 newPosition = new Vector3(x, 0.0f, z);

                // vector with the corresponding uv mapping
                Vector2 uv = new Vector2(x / (2 * radius) + 0.5f, z / (2 * radius) + 0.5f);

                // store the vertex
                result[i + 1] = new VertexPositionNormalTexture(new Vector3(x, 0.0f, z), Vector3.Up, uv);

            }

            return result;
        }
        
    }
}
