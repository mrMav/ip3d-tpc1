using Microsoft.Xna.Framework;

namespace ip3d_tpc1
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

        public CylinderModel(Game game, float radius, float height, int sides) : base(game)
        {
            // assign
            Radius = radius;
            Height = height;
            Sides = sides;

            // create a cylinder
            CylinderMesh cylinder = new CylinderMesh(this, Game, "checker", Radius, Height, Sides);
            CircleMesh topcap     = new CircleMesh(this, Game, "checker", Radius, Sides);
            CircleMesh bottomcap  = new CircleMesh(this, Game, "checker", Radius, Sides);

            topcap.ModelPosition.Y = Height;

            bottomcap.ReverseWinding(true);
                       
            Meshes.Add(cylinder);
            Meshes.Add(topcap);
            Meshes.Add(bottomcap);

            // set the shaders
            foreach(CustomMesh m in Meshes)
            {

                m.TextureShaderEffect.VertexColorEnabled = false;
                m.TextureShaderEffect.PreferPerPixelLighting = true;

                m.TextureShaderEffect.LightingEnabled = true;
                m.TextureShaderEffect.AmbientLightColor = new Vector3(0.15f);
                m.TextureShaderEffect.DirectionalLight0.Enabled = true;
                m.TextureShaderEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
                m.TextureShaderEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1, -1));
                m.TextureShaderEffect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1); 


            }

        }
    }
}
