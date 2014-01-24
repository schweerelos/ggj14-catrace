using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CatGame
{
    class ThreeDObject
    {
        public Model model;
        public String filename;
        public Matrix world = Matrix.Identity;

        public ThreeDObject(String modelFile)
        {
            filename = modelFile;
        }

        public virtual void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(filename);
            // Turn on default lighting in the BasicEffects used by the model
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
        }

        public virtual void Update(float delta)
        {

        }

        public virtual void Draw(float delta, Matrix view, Matrix projection)
        {
            model.Draw(world, view, projection);
        }
    }
}
