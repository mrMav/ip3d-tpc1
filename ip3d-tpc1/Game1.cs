using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc1
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        BasicCamera camera;
        Prism prism;
        
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
                                   
            Window.Title = "EP3D-TPC1";
            IsMouseVisible = true;
            
            // turn off bac face cull
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            prism = new Prism(this);
            Components.Add(prism);

            camera = new BasicCamera(this);
            camera.Target.Y = prism.Height / 2;
            Components.Add(camera);                        
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            
            // first, we update the base, so our components are all good and updated
            base.Update(gameTime);
            
            // here we update the object shader(effect) matrices
            // so it can perform the space calculations on the vertices
            prism.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));
            
            prism.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"Press 1(X), 2(Y) or 3(Z) to rotate the model.\nPress R to reset rotation.\nPress C to toogle camera animation.\nPress W to toogle wireframe.\nPress + and - to add sides to the object ({prism.Sides}).", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();
                       
            base.Draw(gameTime);
        }
    }
}
