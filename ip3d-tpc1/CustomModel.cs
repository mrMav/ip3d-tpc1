using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ip3d_tpc1
{
    /// <summary>
    /// Holds a list of meshes and has a world matrix that affects those meshes.
    /// </summary>
    class CustomModel : GameComponent
    {
        
        // the current model position        
        Vector3 ModelPosition;

        // the current model orientation
        Vector3 ModelRotation;

        // the current model scale
        Vector3 ModelScale;

        // world transform or matrix is the matrix we will multiply 
        // our vertices for. this transforms the vertices from local
        // space to world space
        Matrix WorldTransform;

        // the meshes
        List<CustomMesh> Meshes;

        public CustomModel(Game game) : base(game)
        {

            // initialize properties

            ModelPosition = Vector3.Zero;
            ModelRotation = Vector3.Zero;
            ModelScale = new Vector3(1);

            WorldTransform = Matrix.Identity;

            Meshes = new List<CustomMesh>();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (CustomMesh m in Meshes)
            {
                
                // update mesh world transform

                m.Update(gameTime);

            }

        }

        public void Draw(GameTime gameTime)
        {

            foreach(CustomMesh m in Meshes)
            {

                m.Draw(gameTime);

            }

        }

    }
}
