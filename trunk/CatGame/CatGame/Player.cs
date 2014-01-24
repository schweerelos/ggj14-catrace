using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CatGame
{
    class Player
    {
        protected int lane;
        Model avatar;

        public Player(Model avatar){
            this.avatar = avatar;
        }

        public Player(int number)
        {
            lane = 0;
        }

        public void moveLeft()
        {
            if (lane >= -2)
                lane--;
        }

        public void moveRight()
        {
            if (lane <= 2)
                lane++;
        }

        public void draw(Matrix world, Matrix view, Matrix projection)
        {
            world *= Matrix.CreateTranslation(lane, 0, 0);
            avatar.Draw(world, view, projection);
        }

    }
}
