using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ip3d_tpc2
{
    /// <summary>
    /// Holds a list of meshes and has a world matrix that affects those meshes.
    /// </summary>
    class CustomModel : GameComponent
    {

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

        // the meshes
        protected List<CustomMesh> Meshes;

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

            // update the model transform
            Matrix rotation = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(ModelRotation.Y),
                MathHelper.ToRadians(ModelRotation.X),
                MathHelper.ToRadians(ModelRotation.Z)
            );

            Matrix translation = Matrix.CreateTranslation(ModelPosition);

            Matrix scale = Matrix.CreateScale(ModelScale);

            WorldTransform = scale * rotation * translation;
            
            // update childs
            foreach (CustomMesh m in Meshes)
            {
                
                // update mesh world transform
                m.Update(gameTime);

            }

        }

        // updates the effect matrices
        public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
        {

            foreach (CustomMesh m in Meshes)
            {

                m.UpdateShaderMatrices(viewTransform, projectionTransform);

            }

        }

        // draw all children
        public void Draw(GameTime gameTime)
        {

            foreach(CustomMesh m in Meshes)
            {

                m.Draw(gameTime);

            }

        }

    }
}
