using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CatGame
{
    class Obstacle
    {
        private Model avatar;
        private int lane;
        private int size; // -1 = small; 0 = normal; 1 = big; 2 = huge
        private float distanceTravelled;

        public Obstacle(Model avatar, int lane)
        {
            this.avatar = avatar;
            this.lane = lane;
            size = 0; // "normal" size initially
            distanceTravelled = -100; // TODO use sensible value
        }

        public void moveLeft()
        {
            // TODO take size of obstacle into account
            if (lane >= 1)
                lane--;
        }

        public void moveRight()
        {
            // TODO take size of obstacle into account
            if (lane <= 5)
                lane++;
        }

        public void increaseSize()
        {
            // TODO what if obstacle is already on the edge of the ramp?
            if (size <= 1)
                size++;
        }

        public void decreaseSize()
        {
            if (size >= 0)
                size--;
        }

        public void progress(float amount)
        {
            distanceTravelled += amount;
        }

        public void draw(Matrix view, Matrix projection)
        {
            // TODO take size into account
            Matrix world = Matrix.Identity;
            world *= Matrix.CreateTranslation(lane, 0, distanceTravelled);
            avatar.Draw(world, view, projection);
        }
    }
}
