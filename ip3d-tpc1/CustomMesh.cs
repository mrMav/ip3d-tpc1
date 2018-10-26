using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ip3d_tpc2
{
    /// <summary>
    /// The mesh will hold all the geometry data
    /// </summary>
    class CustomMesh
    {

        // a reference to the parent model
        protected CustomModel Parent;

        // a reference to the the game class
        protected Game Game;

        // if true, a geometry update is needed
        protected bool DirtyGeometry;

        // the current model position        
        public Vector3 ModelPosition;

        // the current model orientation
        public Vector3 ModelRotation;

        // the current model scale
        public Vector3 ModelScale;

        // world transform or matrix is the matrix we will multiply 
        // our vertices for. this transforms the vertices from local
        // space to world space
        public Matrix WorldTransform;

        // the vertex list.
        // the vertex is not a position, it is named after it.
        // we could store any values we want and interpet those values in
        // the shader as we want.
        // in this case, we create a list of vertices to hold a position, a normal and a texture coordinate
        // giving us two vector3 and one vector3
        protected VertexPositionNormalTexture[] VertexList;

        // we are also going to use indexed drawing
        // for that we create an indices list
        protected short[] IndicesList;

        // the buffers to send to gpu
        // the vertices:
        protected VertexBuffer VertexBuffer;

        // indicex buffer
        protected IndexBuffer IndexBuffer;

        // the effect(shader) to apply when rendering
        // we will use Monogame built in BasicEffect for the purpose
        public BasicEffect TextureShaderEffect;        

        public CustomMesh(CustomModel model, Game game)
        {
            // initialize properties

            Parent = model;

            Game = game;

            DirtyGeometry = false;

            ModelPosition = Vector3.Zero;
            ModelRotation = Vector3.Zero;
            ModelScale    = new Vector3(1);

            WorldTransform = Matrix.Identity;            

        }

        public virtual void CreateGeometry()
        {



        }

        // updates the effect matrices
        public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
        {
            TextureShaderEffect.Projection = projectionTransform;
            TextureShaderEffect.View = viewTransform;
            TextureShaderEffect.World = WorldTransform;
        }

        public virtual void Update(GameTime gameTime)
        {

            Matrix rotation = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(ModelRotation.Y),
                MathHelper.ToRadians(ModelRotation.X),
                MathHelper.ToRadians(ModelRotation.Z)
            );

            Matrix translation = Matrix.CreateTranslation(ModelPosition);

            Matrix scale = Matrix.CreateScale(ModelScale);

            WorldTransform = scale * rotation * translation;

            // update with parent position
            WorldTransform = WorldTransform * Parent.WorldTransform;

        }

        public virtual void Draw(GameTime gameTime)
        {



        }

        /// <summary>
        /// Reverses the winding of the mesh polygons.
        /// Optionaly reverses the vertices normal.
        /// </summary>
        /// <param name="reverseNormal"></param>
        public void ReverseWinding(bool reverseNormal = false)
        {

            if(IndicesList.Length < 3)
            {
                return;
            }

            // reverses the order by which the triangle is drawn
            for(int i = 0; i < IndicesList.Length; i += 3)
            {

                short a = IndicesList[i + 1];
                short b = IndicesList[i + 2];

                IndicesList[i + 1] = b;
                IndicesList[i + 2] = a;

            }

            if(reverseNormal)
            {

                // reverse the normal
                for(int i = 0; i < VertexList.Length; i++)
                {

                    VertexList[i].Normal *= -1;

                }

            }

            Game.GraphicsDevice.Indices = null;

            IndexBuffer.SetData<short>(IndicesList);
            
        }

        /// <summary>
        /// Scales the texture coordinates by X and Y of the given Vector2
        /// </summary>
        /// <param name="scale"></param>
        public void ScaleUVs(Vector2 scale)
        {

            for(int i = 0; i < VertexList.Length; i++)
            {

                VertexList[i].TextureCoordinate.X /= scale.X;
                VertexList[i].TextureCoordinate.Y /= scale.Y;

            }

        }

    }

}
