using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ip3d_tpc1
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

        // wireframe rendering toogle
        public bool ShowWireframe;


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

        public virtual void Update(GameTime gameTime)
        {



        }

        public virtual void Draw(GameTime gameTime)
        {



        }

        public void ReverseWinding()
        {

            if(IndicesList.Length < 3)
            {
                return;
            }

            for(int i = 0; i < IndicesList.Length; i += 3)
            {

                short a = IndicesList[i + 1];
                short b = IndicesList[i + 2];

                IndicesList[i + 1] = b;
                IndicesList[i + 2] = a;

            }

            Game.GraphicsDevice.Indices = null;

            IndexBuffer.SetData<short>(IndicesList);
            
        }

    }

}
