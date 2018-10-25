using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ip3d_tpc1
{
    /*
     * Prism will extend the base GameComponent class
     * TODO: Evaluate if DrawableGameComponent would be a better option and why
     */
    //class Prism : GameComponent
    //{
        
    //    // define some constants for the 
    //    // number of sides. These will be our constrains
    //    const int MAX_SIDES = 10;
    //    const int MIN_SIDES = 3;

    //    // the prism dimensions
    //    public float Radius;
    //    public float Height;
    //    public int Sides;

    //    // if true, a geometry update is needed
    //    bool DirtyGeometry;

    //    // the current model position        
    //    Vector3 ModelPosition;

    //    // the current model orientation
    //    Vector3 ModelRotation;

    //    // the current model scale
    //    Vector3 ModelScale;

    //    // world transform or matrix is the matrix we will multiply 
    //    // our vertices for. this transforms the vertices from local
    //    // space to world space
    //    Matrix WorldTransform;

    //    // the vertex list.
    //    // the vertex is not a position, it is named after it.
    //    // we could store any values we want and interpet those values in
    //    // the shader as we want.
    //    // in this case, we create a list of vertices to hold a position, a normal and a texture coordinate
    //    // giving us two vector3 and one vector3
    //    VertexPositionNormalTexture[] VertexList;

    //    // we are also going to use indexed drawing
    //    // for that we create an indices list
    //    short[] IndicesList;

    //    // the effect(shader) to apply when rendering
    //    // we will use Monogame built in BasicEffect for the purpose
    //    BasicEffect TextureShaderEffect;

    //    // this shader will be used for rendering wireframe
    //    BasicEffect ColorShaderEffect;

    //    // the texture to render
    //    Texture2D Texture;

    //    // wireframe rendering toogle
    //    bool ShowWireframe;
        
    //    // constructor
    //    public Prism(Game game, string textureKey, float radius = 5f, float height = 10f, int sides = 3) : base(game)
    //    {

    //        // basic assign
    //        Radius = radius;
    //        Height = height;
    //        Sides = sides;
    //        DirtyGeometry = false;

    //        ModelPosition = Vector3.Zero;
    //        ModelRotation = Vector3.Zero;
    //        ModelScale    = new Vector3(1);            

    //        WorldTransform = Matrix.Identity;

    //        // create and setting up the effect settings
    //        ColorShaderEffect = new BasicEffect(game.GraphicsDevice);  
    //        ColorShaderEffect.LightingEnabled = false;  // we won't be using light. we would need normals for that
    //        ColorShaderEffect.VertexColorEnabled = true;  // we do want color though
    //        ShowWireframe = true;  // enable out of the box wireframe

    //        TextureShaderEffect = new BasicEffect(game.GraphicsDevice);
    //        TextureShaderEffect.LightingEnabled = true;
    //        TextureShaderEffect.VertexColorEnabled = false;


    //        // create the geometry
    //        CreateGeometry();

    //    }

    //    public override void Update(GameTime gameTime)
    //    {

    //        KeyboardState ks = Keyboard.GetState();

    //        /*
    //         * This here is just some functionality for fun.
    //         * The code is pretty readable. We detect a key press od down
    //         * and we perform an action.
    //         * 
    //         * Notice the sides add and subtract constrains.
    //         * We we alter the geometry, we also flag it as dirty.
    //         * 
    //         */ 

    //        //float rotationIncr = 0.1f;
    //        //if (ks.IsKeyDown(Keys.D1))
    //        //{
    //        //    ModelRotation.X += rotationIncr;
    //        //}

    //        //if (ks.IsKeyDown(Keys.D2))
    //        //{
    //        //    ModelRotation.Y += rotationIncr;
    //        //}

    //        //if (ks.IsKeyDown(Keys.D3))
    //        //{
    //        //    ModelRotation.Z += rotationIncr;
    //        //}

    //        //if (OldKeyboardState.IsKeyUp(Keys.R) && ks.IsKeyDown(Keys.R))
    //        //{
    //        //    ModelRotation = Vector3.Zero;
    //        //}            

    //        //if (OldKeyboardState.IsKeyUp(Keys.W) && ks.IsKeyDown(Keys.W))
    //        //{
    //        //    ShowWireframe = !ShowWireframe;
    //        //}

    //        ///*
    //        // * We could keep the model from updating if the sides are constrained to a min or max value, 
    //        // * but this way we can change the color if for some reason with don't like the current
    //        // */ 
    //        //if (OldKeyboardState.IsKeyUp(Keys.OemPlus) && ks.IsKeyDown(Keys.OemPlus))
    //        //{
    //        //    Sides = Sides + 1 > MAX_SIDES ? Sides : Sides + 1;
    //        //    DirtyGeometry = true;
    //        //}

    //        //if (OldKeyboardState.IsKeyUp(Keys.OemMinus) && ks.IsKeyDown(Keys.OemMinus))
    //        //{
    //        //    Sides = Sides - 1 < MIN_SIDES ? Sides : Sides - 1; ;
    //        //    DirtyGeometry = true;
    //        //}

    //        //OldKeyboardState = ks;

    //        // if the geomwtry is dirty, create a new one.
    //        if (DirtyGeometry)
    //        {
    //            CreateGeometry();
    //            DirtyGeometry = false;
    //        }

    //        // update the world transform with all our current stats.
    //        // TODO: update this with position and scale.
    //        WorldTransform = Matrix.CreateRotationY(ModelRotation.Y) * Matrix.CreateRotationX(ModelRotation.X) * Matrix.CreateRotationZ(ModelRotation.Z);

    //        base.Update(gameTime);
    //    }

    //    public void Draw(GameTime gameTime)
    //    {

    //        // render the geometry using a triangle list
    //        // we only use one pass of the shader, but we could have more.
    //        ColorShaderEffect.VertexColorEnabled = true;
    //        ColorShaderEffect.CurrentTechnique.Passes[0].Apply();
    //        Game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, VertexList, 0, Sides * 4); // Sides * 4 because for each side, there are 4 tris. Top, Bottom, and 2 for the sides
            
    //        if (ShowWireframe)
    //        {
    //            // the color of the wireframe is white by default
    //            // it is stored in the DiffuseColor porperty of the effect

    //            ColorShaderEffect.VertexColorEnabled = false;  // deactivate the color channel
    //            ColorShaderEffect.CurrentTechnique.Passes[0].Apply();
    //            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineStrip, VertexList, 0, VertexList.Length - 1);  // here we connect all vertices width a line strip

    //        }

    //    }

    //    // updates the effect matrices
    //    public void UpdateShaderMatrices(Matrix viewTransform, Matrix projectionTransform)
    //    {
    //        ColorShaderEffect.Projection = projectionTransform;
    //        ColorShaderEffect.View = viewTransform;
    //        ColorShaderEffect.World = WorldTransform;
    //    }

    //    // creates and returns the color array
    //    private Color[] CreateColorArray()
    //    {
            
    //        // colors were chosen randomly by me, no preferences here

    //        Color[] c = new Color[10];

    //        c[0] = Color.RosyBrown;
    //        c[1] = Color.Coral;
    //        c[2] = Color.DarkGreen;
    //        c[3] = Color.DarkOrchid;
    //        c[4] = Color.MonoGameOrange;
    //        c[5] = Color.Olive;
    //        c[6] = Color.Navy;
    //        c[7] = Color.RosyBrown;
    //        c[8] = Color.Sienna;
    //        c[9] = Color.Maroon;

    //        return c;
    //    }

    //    // builds the geometry
    //    private void CreateGeometry()
    //    {
    //        // first, the array of vertices is created
    //        VertexList = new VertexPositionNormalTexture[Sides * 12];  // number of sides * 12 because we need 12 vertices to define 4 triangles
    //                                                           // 4 tris because for each side, there are 4 tris. Top, Bottom, and 2 for the sides

    //        // this is the length of a unit circunference
    //        float pi2 = (float)Math.PI * 2;

    //        // the latest position we created.
    //        // we begin by starting at length 0 in the circunference
    //        Vector3 lastPosition = new Vector3(Radius, 0f, 0f);

    //        // a random number generator for random color picking
    //        Random rnd = new Random();

    //        // generate tris
    //        for (int i = 0; i < Sides; i++)
    //        {
    //            // we divide a unit circle circunference length by the number of sides
    //            float segment = pi2 / Sides * (i + 1); // this gives the next arc length

    //            float x = (float)Math.Cos(segment) * Radius;  // apply cos to the arc length, and get the x value from the arc center
    //            float z = (float)Math.Sin(segment) * Radius;  // same but sin

    //            // vector with the new position
    //            Vector3 newPosition = new Vector3(x, 0.0f, z);
                
    //            /*
    //             * The vertices will be created clockwise so they don't get culled
    //             * We create four triangles for each side.
    //             * The upper cap, the bottom cap, and 2 for the sides
    //             * 
    //             */ 

    //            // create bottom cap
    //            VertexList[12 * i + 0] = new VertexPositionColor(Vector3.Zero, Color.Blue);  // center coord
    //            VertexList[12 * i + 1] = new VertexPositionColor(newPosition, Color.Blue);  // new coord
    //            VertexList[12 * i + 2] = new VertexPositionColor(lastPosition, Color.Blue);  // last coord
    //            // 12 is the stride, i is the index, 0, 1, 2 etc is the increment in index

    //            // create side tri 1
    //            VertexList[12 * i + 3] = new VertexPositionColor(lastPosition, sideColor);
    //            VertexList[12 * i + 4] = new VertexPositionColor(newPosition, sideColor);  // new coord
    //            VertexList[12 * i + 5] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), sideColor);  // last coord

    //            // create side tri 2
    //            VertexList[12 * i + 6] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), sideColor);
    //            VertexList[12 * i + 7] = new VertexPositionColor(newPosition, sideColor);  // last coord
    //            VertexList[12 * i + 8] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + Height, newPosition.Z), sideColor);  // new coord

    //            // create top cap
    //            VertexList[12 * i + 9] = new VertexPositionColor(new Vector3(0.0f, Height, 0.0f), Color.Blue);  // center coord
    //            VertexList[12 * i + 10] = new VertexPositionColor(new Vector3(lastPosition.X, lastPosition.Y + Height, lastPosition.Z), Color.Blue);  // new coord
    //            VertexList[12 * i + 11] = new VertexPositionColor(new Vector3(newPosition.X, newPosition.Y + Height, newPosition.Z), Color.Blue);  // last coord

    //            // update the last position with the new one
    //            lastPosition = newPosition;

    //        }

    //    }

    //}
}
