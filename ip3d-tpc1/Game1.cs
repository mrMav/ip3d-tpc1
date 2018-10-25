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

        // disc
        CircleMesh disc;

        CircleMesh disc2;

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // prepare for anti aliasing
            // reference: http://community.monogame.net/t/solved-anti-aliasing/10561
            // state that we will use a HiDef profile, check here the difs:
            // https://blogs.msdn.microsoft.com/shawnhar/2010/03/12/reach-vs-hidef/
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
                                   
            Window.Title = "EP3D-TPC1 - JORGE NORO - 15705";
            IsMouseVisible = true;

            // ensure that the culling is happening for counterclockwise polygons
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;

            disc = new CircleMesh(null, this, "checker", 10f, 3);
            disc.ReverseWinding();  // I'm actually proud by thinking on this

            disc2 = new CircleMesh(null, this, "checker", 8f, 8);


            // create the camera object and add it aswell to the component system
            // we update the target to fit the model in the scren
            camera = new BasicCamera(this, 45f, 20f);
            camera.Target.Y = 0;
            camera.RotateCamera = true;
            Components.Add(camera);                        
            
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // first, we update the base, so our components are all good and updated
            base.Update(gameTime);
            
            // here we update the object shader(effect) matrices
            // so it can perform the space calculations on the vertices
            disc.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);
            disc2.UpdateShaderMatrices(camera.ViewTransform, camera.ProjectionTransform);

        }

        protected override void Draw(GameTime gameTime)
        {
            // clear frame
            GraphicsDevice.Clear(new Color(0.15f, 0.15f, 0.15f));
            
            // draw the object
            disc.Draw(gameTime);
            disc2.Draw(gameTime);

            // render the gui text
            // notive the DepthStencilState, without default set in, depth will not 
            // be calculated when drawing wireframes.
            // more investigation needs to be done in order to understand why Monogame
            // is doing it this way.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.Default, null, null, null);
            spriteBatch.DrawString(font, $"Press 1(X), 2(Y) or 3(Z) to rotate the model.\nPress R to reset rotation.\nPress C to toogle camera animation.\nPress W to toogle wireframe.\nPress + and - to add sides to the object ({disc.Sides}).", new Vector2(10f, 10f), new Color(0f, 1f, 0f));
            spriteBatch.End();
                       
            base.Draw(gameTime);
        }
    }
}
