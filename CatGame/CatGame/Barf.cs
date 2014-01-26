using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CatGame
{
    class Barf : ThreeDObject
    {

        public Vector3 currentPosition;
        const float speed = 20;
        public Player spawner;
        public Player.Bonus type;
        

        public Barf(Player spawner)
            : base("barf")
        {
            this.currentPosition = spawner.currentPos;
            this.type = spawner.getActiveBonus();
            this.spawner = spawner;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            this.currentPosition.Z -= speed * delta;
            world = Matrix.Identity;
            world *= Matrix.CreateTranslation(currentPosition);
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, RainbowLighting lighting, Player activePlayer)
        {
            base.Draw(gameTime, view, projection, lighting, activePlayer);

        }

        internal BoundingBox getBoundingBox()
        {
            Vector3 min = Vector3.Transform(new Vector3(-.45f, -.45f, -.45f), world);
            Vector3 max = Vector3.Transform(new Vector3(.45f, .45f, .45f), world);
            /*Vector3 min = currentPosition - new Vector3(0.45f, 0.45f, 0.45f);
            Vector3 max = currentPosition + new Vector3(0.45f, 0.45f, 0.45f);*/
            return new BoundingBox(min, max);
        }
    }
}
