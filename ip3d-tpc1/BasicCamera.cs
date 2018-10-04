using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ip3d_tpc1
{
    class BasicCamera : GameComponent
    {

        public Vector3 Position;
        public Vector3 Target;

        public Matrix ViewTransform;
        public Matrix ProjectionTransform;

        float FieldOfView;
        float SphereRadius;

        bool RotateCamera;

        KeyboardState OldKeyboardState;

        public BasicCamera(Game game, float fieldOfView = 45f, float sphereRadius = 10f) : base(game)
        {
            FieldOfView = fieldOfView;
            SphereRadius = sphereRadius;
            RotateCamera = true;
            
            Position = new Vector3(0f, 0f, -SphereRadius);
            Target = Vector3.Zero;

            ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);
            ProjectionTransform = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView), game.GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 1000f);

            OldKeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState ks = Keyboard.GetState();

            if (OldKeyboardState.IsKeyUp(Keys.C) && ks.IsKeyDown(Keys.C))
            {
                RotateCamera = !RotateCamera;
            }

            if (RotateCamera)
            {
                Position.X = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 20f;
                Position.Z = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 20f;
                Position.Y = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.00025f) * 20f;

                ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            }

            base.Update(gameTime);
        }

    }
}
