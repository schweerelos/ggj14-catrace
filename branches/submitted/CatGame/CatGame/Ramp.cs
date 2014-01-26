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

        public override void SetEffect(Matrix view, Matrix projection, RainbowLighting lighting, BasicEffect effect, Player activePlayer, GameTime gameTime)
        {
            base.SetEffect(view, projection, lighting, effect, activePlayer, gameTime);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = Vector3.Down;
            //effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.SpecularColor = Vector3.Zero;
            effect.AmbientLightColor = Vector3.One;
        }
    }
}
