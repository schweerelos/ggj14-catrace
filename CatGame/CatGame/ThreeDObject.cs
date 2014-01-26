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

        public virtual void Draw(GameTime gameTime, Matrix view, Matrix projection, RainbowLighting lighting, Player activePlayer)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    SetEffect(view, projection, lighting, effect, activePlayer, gameTime);
                }
                mesh.Draw();
            }
        }

        public virtual void SetEffect(Matrix view, Matrix projection, RainbowLighting lighting, BasicEffect effect, Player activePlayer, GameTime gameTime)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.LightingEnabled = true;
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = lighting.GetColor();
            effect.DirectionalLight0.Direction = lighting.GetDirection();
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
            effect.DirectionalLight1.Enabled = false;
            effect.DirectionalLight2.Enabled = false;
            effect.SpecularPower = 10;
            effect.SpecularColor = lighting.GetColor();
            effect.AmbientLightColor = Color.DarkGray.ToVector3();
            effect.PreferPerPixelLighting = true;
        }
        
    }
}
