﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc2
{
    /*
     * Basic Camera
     */
    class BasicCamera : Camera
    {

        // the camera sphere constrain, for when rotating animation is activated
        public float SphereRadius;

        // trigger to toogle between animated camera or not
        public bool RotateCamera;

        // class constructor
        public BasicCamera(Game game, float fieldOfView, float sphereRadius = 10f) : base(game, fieldOfView)
        {
            // basic initializations
            SphereRadius = sphereRadius;
            RotateCamera = true;

            Position = new Vector3(SphereRadius, SphereRadius, SphereRadius);
            Target = Vector3.Zero;

            // view matrix is calculated with a LookAt method. It allows
            // to create a view matrix based on a position and target
            ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }

        public override void Update(GameTime gameTime)
        {

            if(Controls.IsKeyPressed(Keys.Space))
            {
                RotateCamera = !RotateCamera;
            }

            if (RotateCamera)
            {
                // camera rotation is based on the total milliseconds passed since the game init
                // it allows for continuous animation. We multiply the sin or cos for the sphereRadius
                Position.X = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.0001f) * SphereRadius;
                Position.Z = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.0001f) * SphereRadius;
                //Position.Y = (float)Math.Abs(Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.00005f) * SphereRadius) + 20f;


                // finnaly, update the view matrix
                ViewTransform = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            }

            base.Update(gameTime);
        }

        public override string About()
        {
            return "Endless aerial view.";
        }

    }
}