using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ip3d_tpc1
{
    class Prism : GameComponent
    {

        const int MAX_SIDES = 10;
        const int MIN_SIDES = 3;

        public float Radius;
        public float Height;
        public int Sides;

        bool DirtyGeometry;

        Vector3 ModelRotation;

        Matrix WorldTransform;
        VertexPositionColor[] VertexList;
        BasicEffect ColorShaderEffect;
        bool ShowWireframe;

        KeyboardState OldKeyboardState;

        public Prism(Game game, float radius = 5f, float height = 10f, int sides = 3) : base(game)
        {

            Radius = radius;
            Height = height;
            Sides = sides;
            DirtyGeometry = false;

            ModelRotation = Vector3.Zero;            
            WorldTransform = Matrix.Identity;

            ColorShaderEffect = new BasicEffect(game.GraphicsDevice);
            ColorShaderEffect.LightingEnabled = false;
            ColorShaderEffect.VertexColorEnabled = true;
            ShowWireframe = false;

            OldKeyboardState = Keyboard.GetState();

            CreateGeometry();

        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState ks = Keyboard.GetState();

            float rotationIncr = 0.1f;
            if (ks.IsKeyDown(Keys.D1))
            {
                ModelRotation.X += rotationIncr;
            }

            if (ks.IsKeyDown(Keys.D2))
            {
                ModelRotation.Y += rotationIncr;
            }

            if (ks.IsKeyDown(Keys.D3))
            {
                ModelRotation.Z += rotationIncr;
            }

            if (OldKeyboardState.IsKeyUp(Keys.R) && ks.IsKeyDown(Keys.R))
            {
                ModelRotation = Vector3.Zero;
            }            

            if (OldKeyboardState.IsKeyUp(Keys.W) && ks.IsKeyDown(Keys.W))
            {
                ShowWireframe = !ShowWireframe;
            }

            if (OldKeyboardState.IsKeyUp(Keys.OemPlus) && ks.IsKeyDown(Keys.OemPlus))
            {
                Sides = Sides + 1 > MAX_SIDES ? Sides : Sides + 1;
                DirtyGeometry = true;
            }

            if (OldKeyboardState.IsKeyUp(Keys.OemMinus) && ks.IsKeyDown(Keys.OemMinus))
            {
                Sides = Sides - 1 < MIN_SIDES ? Sides : Sides - 1; ;
                DirtyGeometry = true;
            }

            OldKeyboardState = ks;

            if (DirtyGeometry)
            {
                CreateGeometry();
                DirtyGeometry = false;
            }

            WorldTransform = Matrix.CreateRotationY(ModelRotation.Y) * Matrix.CreateRotationX(ModelRotation.X) * Matrix.CreateRotationZ(ModelRotation.Z);

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {

            ColorShaderEffect.VertexColorEnabled = true;
            ColorShaderEffect.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, VertexList, 0, Sides * 4);
            
            if (ShowWireframe)
            {
                ColorShaderEffect.VertexColorEnabled = false;
                ColorShaderEffect.CurrentTechnique.Passes[0].Apply();
                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, VertexList, 0, VertexList.Length - 1);

            }

        }

        public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
        {
            ColorShaderEffect.Projection = projectionTransform;
            ColorShaderEffect.View = viewTransform;
            ColorShaderEffect.World = WorldTransform;
        }

        private void CreateGeometry()
        {

            VertexList = new VertexPositionColor[Sides * 12];  // number of sides * 12 because we need 12 vertices to define 4 triangles

            float pi2 = (float)Math.PI * 2;
            Vector3 lastPosition = new Vector3(Radius, 0f, 0f);

            // generate tris
            for (int i = 0; i < Sides; i++)
            {
                // we divide a unit circle circunference length by the number of sides
                float segment = pi2 / Sides * (i + 1); // this gives the next arc length

                float x = (float)Math.Cos(segment) * Radius;
                float z = (float)Math.Sin(segment) * Radius;

                Vector3 newPosition = new Vector3(x, 0.0f, z);

                // create bottom cap
                VertexList[12 * i + 0] = new VertexPositionColor(Vector3.Zero, Color.Blue);  // center coord
                VertexList[12 * i + 1] = new VertexPositionColor(newPosition, Color.Blue);  // new coord
                VertexList[12 * i + 2] = new VertexPositionColor(lastPosition, Color.Blue);  // last coord
                // 12 is the stride, i is the index, 0, 1, 2 etc is the increment

                //// create side tri 1
                VertexList[12 * i + 3] = new VertexPositionColor(lastPosition, Color.Gray);
                VertexList[12 * i + 4] = new VertexPositionColor(newPosition, Color.Gray);  // new coord
                VertexList[12 * i + 5] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), Color.Gray);  // last coord

                // create side tri 2
                VertexList[12 * i + 6] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), Color.Gray);
                VertexList[12 * i + 7] = new VertexPositionColor(newPosition, Color.Gray);  // last coord
                VertexList[12 * i + 8] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + Height, newPosition.Z), Color.Gray);  // new coord

                // create top cap
                VertexList[12 * i + 9] = new VertexPositionColor(new Vector3(0.0f, Height, 0.0f), Color.Blue);  // center coord
                VertexList[12 * i + 10] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), Color.Blue);  // new coord
                VertexList[12 * i + 11] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + Height, newPosition.Z), Color.Blue);  // last coord

                lastPosition = newPosition;

            }

        }

    }
}
