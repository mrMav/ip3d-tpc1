using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        // I'm keeping the spritebatch because there will
        // be a gui string showing the controls
        SpriteBatch spriteBatch;

        // this will be the font we will use for text rendering
        SpriteFont font;

        // our camera class, check BasicCamera.cs
        BasicCamera camera;
        
        // the prism
        CylinderModel cylinderModel;

        // plane
        Plane plane;

        // absolute axis
        Axis3D worldAxis;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // prepare for anti aliasing
            // reference: http://community.monogame.net/t/solved-anti-aliasing/10561
            // state that we will use a HiDef profile, check here the difs:
            // https://blogs.msdn.microsoft.com/shawnhar/2010/03/12/reach-vs-hidef/
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;
            graphics.ApplyChanges();            
        }

        // callback for preparing device settings, see link above for more info
        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            graphics.PreferMultiSampling = true;
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
        }

        protected override void Initialize()
        {
                                   
            Window.Title = "EP3D-TPC2 - JORGE NORO - 15705";
            IsMouseVisible = false;
                        
            // create the camera object and add it aswell to the component system
            // we update the target to fit the model in the scren
            camera = new BasicCamera(this, 45f, 75f);            
            camera.RotateCamera = false;

            // initialize the axis and add it to the componetens manager
            worldAxis = new Axis3D(this, Vector3.Zero, 200f);
            Components.Add(worldAxis);

            // init
            cylinderModel = new CylinderModel(this, 7.5f, 30f, 16);

            // init
            plane = new Plane(null, this, "checker2", 150f, 150f, 1, 1);            

            // init controls
            Mouse.SetPosition(Window.Position.X + (graphics.PreferredBackBufferWidth / 2), Window.Position.Y + (graphics.PreferredBackBufferHeight / 2));
            Controls.Initilalize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // font loading
            font = Content.Load<SpriteFont>("font");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // set the current keyboard state
            Controls.UpdateCurrentStates();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // locking the mouse
            Mouse.SetPosition(Window.Position.X + (graphics.PreferredBackBufferWidth / 2), Window.Position.Y + (graphics.PreferredBackBufferHeight / 2));
            
            /*
             * Update game objects
             */ 

            cylinderModel.Update(gameTime);
            cylinderModel.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

            plane.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

            worldAxis.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

            camera.Update(gameTime);

            // set the last keyboard state
            Controls.UpdateLastStates();

        }

        protected override void Draw(GameTime gameTime)
        {
            // clear frame
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));

            /*
             * Draw game objects
             */ 

            cylinderModel.Draw(gameTime);

            plane.Draw(gameTime);

            /*
             * Draw gui text
             */ 
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"Press SPACE to toggle camera rotation.\nPress Up and Down arrows to move the cylinder.\nPress Left and Right arrows to change cylinder orientation.", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();
                       
            base.Draw(gameTime);
        }
    }
}
