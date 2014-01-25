using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CatGame
{
    class Obstacle : ThreeDObject
    {
        private int lane;
        private int size; // -1 = small; 0 = normal; 1 = big; 2 = huge
        public float distanceTravelled;
        private static Model cubeModel;

        public Obstacle(int lane) : base("cube")
        {
            model = cubeModel;
            this.lane = lane;
            size = 0; // "normal" size initially
            distanceTravelled = -100;
            world = Matrix.CreateTranslation(lane, 0, distanceTravelled);
            this.collisionTested = false;
        }

        public override void LoadContent(ContentManager content)
        {
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

        internal bool hasReached(float position)
        {
            return distanceTravelled >= position;
        }

        public override void Update(float delta)
        {
            distanceTravelled += (delta * 10);
            world = Matrix.CreateTranslation(lane, 0, distanceTravelled);
        }

        internal bool covers(float queryLane)
        {
            int qLane = (int) Math.Round(queryLane);
            switch (size)
            {
                case -1:
                    return qLane == lane;
                case 0:
                    return qLane == lane;
                case 1:
                    return Math.Abs(lane - qLane) <= 1;
                case 2:
                    return Math.Abs(lane - qLane) <= 2;
            }
            return false;
        }

        public static void StaticLoadContent(ContentManager content)
        {
            cubeModel = content.Load<Model>("cube");
            foreach (ModelMesh mesh in cubeModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
        }

        public bool collisionTested { get; set; }

    }
}
