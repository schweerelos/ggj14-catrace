using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CatGame
{
    class Ramp
    {
        Model model;
        float textureCycle = 0;
        Matrix world = Matrix.CreateTranslation(3, -1, 5);

        public void Update(float delta)
        {
            // Cycle the texture
            textureCycle += delta;
        }

        internal void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("ramp");
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.AmbientLightColor = Color.White.ToVector3();
                }
        }

        internal void Draw(Matrix view, Matrix projection)
        {
            model.Draw(world, view, projection);
        }
    }
}
