using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CatGame
{
    class Ramp : ThreeDObject
    {
        float textureCycle = 0;

        public Ramp()
            : base("ramp")
        {
            world = Matrix.CreateTranslation(3, -1, 5);
        }

        public override void Update(float delta)
        {
            // Cycle the texture
            world = Matrix.CreateTranslation(3, -1, 5);
            textureCycle += delta;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = Vector3.Down;
                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    effect.AmbientLightColor = Color.White.ToVector3();
                    effect.LightingEnabled = true;
                }
        }
    }
}
