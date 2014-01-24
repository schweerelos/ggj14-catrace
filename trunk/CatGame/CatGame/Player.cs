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
        private int lane = 3;
        private int playerNumber;
        private Model avatar;

        public Player(Model avatar, int playerNumber){
            this.avatar = avatar;
            lane = 3;
            this.playerNumber = playerNumber;

        }

        public void moveLeft()
        {
            if (lane >= 1)
                lane--;
        }

        public void moveRight()
        {
            if (lane <= 5)
                lane++;
        }

        public void draw(Matrix world, Matrix view, Matrix projection)
        {
            world *= Matrix.CreateTranslation(lane, 0, 0);
            avatar.Draw(world, view, projection);
        }

    }
}
