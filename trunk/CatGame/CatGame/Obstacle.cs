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
            distanceTravelled = -100;
        }

        public void moveLeft()
        {
            int leftCutoff = Math.Max(1, size + 1);   
            if (lane >= leftCutoff)
                lane--;
        }

        public void moveRight()
        {
            if (lane <= Math.Min(5, 5 - size))
                lane++;
        }

        public void increaseSize()
        {
            Console.WriteLine("Old size is " + size + "; old lane is " + lane);
            switch (size)
            {
                case -1:
                    size = 0;
                    return;
                case 0:
                    size = 1;
                    if (lane == 0) lane = 1;
                    else if (lane == 6) lane = 5;
                    return;
                case 1:
                    size = 2;
                    if (lane <= 1) lane = 2;
                    else if (lane >= 5) lane = 4;
                    return;
            }
            Console.WriteLine("New size is " + size + "; new lane is " + lane);
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

        internal bool hasReached(float position)
        {
            return distanceTravelled >= position;
        }

        public void draw(Matrix view, Matrix projection)
        {
            // TODO take size into account
            Matrix world = Matrix.Identity;
            world *= Matrix.CreateTranslation(lane, 0, distanceTravelled);
            avatar.Draw(world, view, projection);
        }


        internal bool covers(int queryLane, bool isJumping)
        {
            switch (size)
            {
                case -1:
                    return queryLane == lane && !isJumping;
                case 0:
                    return queryLane == lane;
                case 1:
                    return Math.Abs(lane - queryLane) <= 1;
                case 2:
                    return Math.Abs(lane - queryLane) <= 2;
            }
            return false;
        }
    }
}
