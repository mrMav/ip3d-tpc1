using Microsoft.Xna.Framework;
using System;

namespace ip3d_tpc2
{
    class CylinderModel : CustomModel
    {

        // define some constants for the 
        // number of sides. These will be our constrains
        const int MAX_SIDES = 10;
        const int MIN_SIDES = 3;

        // the dimensions
        public float Radius;
        public float Height;
        public int Sides;

        // the model direction vectors
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        
        // the step to increase rotation
        float YawStep = 8f;

        // increase in acceleration
        public float AccelerationValue = 1.0f;

        // velocity will be caped to this maximum
        public float MaxVelocity = 5f;

        // the velocity vector
        public Vector3 Velocity = Vector3.Zero;

        // the drag to apply
        public float Drag = 0.8f;

        // constructor
        public CylinderModel(Game game, float radius, float height, int sides) : base(game)
        {
            // assign
            Radius = radius;
            Height = height;
            Sides = sides;

            // create a cylinder
            CylinderMesh cylinder = new CylinderMesh(this, Game, "checker", Radius, Height, Sides);
            CircleMesh topcap     = new CircleMesh(this, Game, "arrow", Radius, Sides);
            CircleMesh bottomcap  = new CircleMesh(this, Game, "checker", Radius, Sides);

            topcap.ModelPosition.Y = Height;

            bottomcap.ReverseWinding(true);  // needs to invert so it doesnt get culled
                       
            Meshes.Add(cylinder);
            Meshes.Add(topcap);
            Meshes.Add(bottomcap);

            // set the shaders
            foreach(CustomMesh m in Meshes)
            {

                // override meshes default values, and add model own material specifications

                m.TextureShaderEffect.VertexColorEnabled = false;
                m.TextureShaderEffect.PreferPerPixelLighting = true;

                m.TextureShaderEffect.LightingEnabled = true;
                m.TextureShaderEffect.AmbientLightColor = new Vector3(0.15f);
                m.TextureShaderEffect.DirectionalLight0.Enabled = true;
                m.TextureShaderEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
                m.TextureShaderEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1, -1));
                m.TextureShaderEffect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
                m.TextureShaderEffect.SpecularPower = 3.0f;


            }

        }

        // updates the vectors, using basic trigonometry
        private void UpdateCameraVectors()
        {

            // thes function was built with the help found on the current article:
            // https://learnopengl.com/Getting-started/Camera

            // first the front vector is calculated and normalized.
            // then, the right vector is calculated, crossing the front and world up vector
            // the camera up vector, is then calculated, crossing the right and the front

            Front.X = (float)Math.Cos(MathHelper.ToRadians(-ModelRotation.Y)) * (float)Math.Cos(MathHelper.ToRadians(ModelRotation.X));
            Front.Y = (float)Math.Sin(MathHelper.ToRadians(ModelRotation.X));
            Front.Z = (float)Math.Sin(MathHelper.ToRadians(-ModelRotation.Y)) * (float)Math.Cos(MathHelper.ToRadians(ModelRotation.X));
            Front.Normalize();

            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.Up));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public override void Update(GameTime gameTime)
        {
            
            float dt = (float)gameTime.TotalGameTime.TotalSeconds;

            // controls rotation
            if (Controls.IsKeyDown(Controls.RotateLeft))
            {
                ModelRotation.Y += YawStep;

            }
            else if (Controls.IsKeyDown(Controls.RotateRight))
            {
                ModelRotation.Y -= YawStep;
            }

            UpdateCameraVectors();

           // update the model position, based on the updated vectors
            if (Controls.IsKeyDown(Controls.Forward))
            {
                Velocity += Front * AccelerationValue;

            }
            else if (Controls.IsKeyDown(Controls.Backward))
            {
                Velocity -= Front * AccelerationValue;
            }

            // cap the velocity so we don't move faster diagonally
            if (Velocity.Length() > MaxVelocity)
            {
                Velocity.Normalize();
                Velocity *= MaxVelocity;
            }

            // apply the velocity to the position, based on the delta time between frames
            ModelPosition += Velocity * (dt * 0.01f);

            // add some sexy drag
            Velocity *= Drag;

            base.Update(gameTime);
        }
    }
}
