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
        SpriteFont font;

        Matrix world, view, projection;

        int numberOfSides = 10;
        float size = 2f;
        VertexPositionColor[] vertexList;

        BasicEffect basicEffect;
        BasicEffect basicEffectBlack;

        Vector3 cameraPosition;
        Vector3 cameraTarget;
        
        Vector3 modelRotate = Vector3.Zero;
        float ammount = 0.1f;

        bool rotateCamera = true;
        bool showWireframe = true;
        bool dirtyObject = false;

        const int MAX_SIDES = 20;
        const int MIN_SIDES = 3;

        KeyboardState oldks;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // prepare for anti aliasing
            // reference: http://community.monogame.net/t/solved-anti-aliasing/10561
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;
            graphics.ApplyChanges();

        }

        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            graphics.PreferMultiSampling = true;
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
        }

        protected override void Initialize()
        {

            Console.WriteLine(graphics.GraphicsProfile);
            Console.WriteLine(GraphicsDevice.PresentationParameters.MultiSampleCount);
                       
            Window.Title = "EP3D-TPC1";
            IsMouseVisible = true;

            cameraPosition = new Vector3(4f, 4f, 4f);
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

            MakeObject();

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;

            basicEffectBlack = new BasicEffect(GraphicsDevice);

            // turn off bac face cull
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            oldks = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // some functionality
            KeyboardState ks = Keyboard.GetState();
            
            if(ks.IsKeyDown(Keys.D1))
            {
                modelRotate.X += ammount;

            }

            if (ks.IsKeyDown(Keys.D2))
            {
                modelRotate.Y += ammount;

            }

            if (ks.IsKeyDown(Keys.D3))
            {
                modelRotate.Z += ammount;
            }

            if (oldks.IsKeyUp(Keys.R) && ks.IsKeyDown(Keys.R))
            {
                modelRotate = Vector3.Zero;
            }

            if (oldks.IsKeyUp(Keys.C) && ks.IsKeyDown(Keys.C))
            {
                rotateCamera = !rotateCamera;
            }

            if (oldks.IsKeyUp(Keys.W) && ks.IsKeyDown(Keys.W))
            {
                showWireframe = !showWireframe;
            }

            if(oldks.IsKeyUp(Keys.OemPlus) && ks.IsKeyDown(Keys.OemPlus))
            {
                numberOfSides = numberOfSides + 1 > MAX_SIDES ? numberOfSides : numberOfSides + 1;
                dirtyObject = true;
            }

            if (oldks.IsKeyUp(Keys.OemMinus) && ks.IsKeyDown(Keys.OemMinus))
            {
                numberOfSides = numberOfSides - 1 < MIN_SIDES ? numberOfSides : numberOfSides - 1; ;
                dirtyObject = true;
            }
            
            oldks = ks;

            if(dirtyObject)
            {
                MakeObject();
                dirtyObject = false;
            }

            // update object world matrix
            world = Matrix.CreateRotationY(modelRotate.Y) * Matrix.CreateRotationX(modelRotate.X) * Matrix.CreateRotationZ(modelRotate.Z);

            if(rotateCamera)
            {
                cameraPosition.X = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 4f;
                cameraPosition.Z = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.001f) * 4f;
                cameraPosition.Y = (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 0.00025f) * 4f;

                view = Matrix.CreateLookAt(
                    cameraPosition: cameraPosition,
                    cameraTarget: cameraTarget,
                    cameraUpVector: Vector3.Up
                );

            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));

            // TODO: Add your drawing code here

            basicEffect.Projection = projection;
            basicEffect.View = view;
            basicEffect.World = world;
            
            basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertexList, 0, numberOfSides * 4);

            if (showWireframe)
            {

                basicEffectBlack.Projection = projection;
                basicEffectBlack.View = view;
                basicEffectBlack.World = world;
                basicEffectBlack.DiffuseColor = Vector3.Zero;
                basicEffectBlack.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertexList, 0, vertexList.Length - 1);

            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"Press 1(X), 2(Y) or 3(Z) to rotate the model.\nPress R to reset rotation.\nPress C to toogle camera animation.\nPress W to toogle wireframe.\nPress + and - to add sides to the object ({numberOfSides}).", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();



            base.Draw(gameTime);
        }

        private void MakeObject()
        {

            vertexList = new VertexPositionColor[numberOfSides * 12];
            // number of sides * 12 because we need 12 vertices to define 4 triangles

            float pi2 = (float)Math.PI * 2;
            Vector3 lastPosition = new Vector3(1f, 0f, 0f);

            // generate tris
            for (int i = 0; i < numberOfSides; i++)
            {
                float segment = pi2 / numberOfSides * (i + 1);

                float x = (float)Math.Cos(segment);
                float z = (float)Math.Sin(segment);

                Vector3 newPosition = new Vector3(x, 0.0f, z);

                // create bottom cap
                vertexList[12 * i + 0] = new VertexPositionColor(Vector3.Zero, Color.Blue);  // center coord
                vertexList[12 * i + 1] = new VertexPositionColor(newPosition, Color.Blue);  // new coord
                vertexList[12 * i + 2] = new VertexPositionColor(lastPosition, Color.Blue);  // last coord
                // 12 is the stride, i is the index, 0, 1, 2 etc is the increment

                //// create side tri 1
                vertexList[12 * i + 3] = new VertexPositionColor(lastPosition, Color.Gray);
                vertexList[12 * i + 4] = new VertexPositionColor(newPosition, Color.Gray);  // new coord
                vertexList[12 * i + 5] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + size, lastPosition.Z), Color.Gray);  // last coord

                // create side tri 2
                vertexList[12 * i + 6] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + size, lastPosition.Z), Color.Gray);
                vertexList[12 * i + 7] = new VertexPositionColor(newPosition, Color.Gray);  // last coord
                vertexList[12 * i + 8] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + size, newPosition.Z), Color.Gray);  // new coord

                // create top cap
                vertexList[12 * i + 9] = new VertexPositionColor(new Vector3(0.0f, size, 0.0f), Color.Blue);  // center coord
                vertexList[12 * i + 10] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + size, lastPosition.Z), Color.Blue);  // new coord
                vertexList[12 * i + 11] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + size, newPosition.Z), Color.Blue);  // last coord

                lastPosition = newPosition;

            }

        }
    }
}
