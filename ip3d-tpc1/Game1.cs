using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;  // i'll keep this to see if i can render some text

        Matrix world, view, projection;

        int numberOfSides = 10;
        float size = 2f;
        VertexPositionColor[] vertexList;

        BasicEffect basicEffect;
        BasicEffect basicEffectBlack;

        Vector3 cameraPosition;
        Vector3 cameraTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            // set render properties, eg AA
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            graphics.ApplyChanges();

            Window.Title = "EP3D-TPC1";
            IsMouseVisible = true;

            cameraPosition = new Vector3(2f, 2f, 2f);
            cameraTarget = new Vector3(0f, size / 2, 0f);

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                cameraPosition: cameraPosition,
                cameraTarget: cameraTarget,
                cameraUpVector: Vector3.Up
            );
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.DisplayMode.AspectRatio,
                1f,
                1000f
            );

            vertexList = new VertexPositionColor[numberOfSides * 2 * 3];
            // number of sides * 2 because one is for bottom and other for side.
            // times 3 because we need 3 coords for each tri

            float pi2 = (float)Math.PI * 2;
            Vector3 lastPosition = new Vector3(1f, 0f, 0f);

            // generate tris
            for (int i = 0; i < numberOfSides; i++)
            {
                float segment = pi2 / numberOfSides * (i + 1);

                float x = (float)Math.Cos(segment);
                float z = (float)Math.Sin(segment);

                Vector3 newPosition = new Vector3(x, 0.0f, z);

                // create bottom tri
                vertexList[6 * i + 0] = new VertexPositionColor(Vector3.Zero, Color.Blue);  // center coord
                vertexList[6 * i + 1] = new VertexPositionColor(newPosition, Color.Blue);  // new coord
                vertexList[6 * i + 2] = new VertexPositionColor(lastPosition, Color.Blue);  // last coord

                // create side tri
                vertexList[6 * i + 3] = new VertexPositionColor(new Vector3(0f, size, 0f), Color.Gray);  // center upper coord
                vertexList[6 * i + 4] = new VertexPositionColor(lastPosition, Color.Gray);  // last coord
                vertexList[6 * i + 5] = new VertexPositionColor(newPosition, Color.Gray);  // new coord

                lastPosition = newPosition;

            }

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;

            basicEffectBlack = new BasicEffect(GraphicsDevice);

            // turn off bac face cull
            RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            cameraPosition.X = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 4f;
            cameraPosition.Z = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 4f;
            cameraPosition.Y = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.0005f) * 4f;

            view = Matrix.CreateLookAt(
                cameraPosition: cameraPosition,
                cameraTarget: cameraTarget,
                cameraUpVector: Vector3.Up
            );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            basicEffect.Projection = projection;
            basicEffect.View = view;
            basicEffect.World = world;

            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertexList, 0, numberOfSides * 2);

            basicEffectBlack.Projection = projection;
            basicEffectBlack.View = view;
            basicEffectBlack.World = world;
            basicEffectBlack.DiffuseColor = Vector3.Zero;
            basicEffectBlack.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertexList, 0, vertexList.Length - 1);


            base.Draw(gameTime);
        }
    }
}
