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
        public enum Size { SMALL, NORMAL, BIG, HUGE };

        private const bool freezeInFinalState = true;

        private int lane;
        private int initialLane;
        private Size size;
        public float distanceTravelled;
        private static Model cubeModel;
        private float scaleFactor;

        public Obstacle(int lane) : base("cube")
        {
            model = cubeModel;
            this.lane = lane;
            initialLane = lane;
            size = Size.NORMAL;
            distanceTravelled = -100;
            world = Matrix.CreateTranslation(initialLane, 0, distanceTravelled);
            this.collisionTested = false;
            this.usingFinalState = false;
        }

        public override void LoadContent(ContentManager content)
        {
        }

        public void moveLeft()
        {
            if (usingFinalState && freezeInFinalState)
                return;

            int cutoff = 1;
            switch (size)
            {
                case Size.BIG:
                    cutoff = 2;
                    break;
                case Size.HUGE:
                    cutoff = 3;
                    break;
            }
            if (lane >= cutoff)
                lane--;
        }

        public void moveRight()
        {
            if (usingFinalState && freezeInFinalState)
                return;

            int cutoff = 5;
            switch (size)
            {
                case Size.BIG:
                    cutoff = 4;
                    break;
                case Size.HUGE:
                    cutoff = 5;
                    break;
            }
            if (lane <= cutoff)
                lane++;
        }

        public void increaseSize()
        {
            if (usingFinalState && freezeInFinalState)
                return;

            switch (size)
            {
                case Size.SMALL:
                    size = Size.NORMAL;
                    return;
                case Size.NORMAL:
                    size = Size.BIG;
                    if (lane == 0) lane = 1;
                    else if (lane == 6) lane = 5;
                    return;
                case Size.BIG:
                    size = Size.HUGE;
                    if (lane <= 1) lane = 2;
                    else if (lane >= 5) lane = 4;
                    return;
            }
        }

        public void decreaseSize()
        {
            if (usingFinalState && freezeInFinalState)
                return;

            switch (size)
            {
                case Size.NORMAL:
                    size = Size.SMALL;
                    break;
                case Size.BIG:
                    size = Size.NORMAL;
                    break;
                case Size.HUGE:
                    size = Size.BIG;
                    break;
            }
        }

        internal bool hasReached(float position)
        {
            return distanceTravelled >= position;
        }

        public override void Update(float delta)
        {
            distanceTravelled += (delta * 10);
            float newScaleFactor = 1;
            switch (size)
            {
                case Size.SMALL:
                    newScaleFactor = 0.7f;
                    break;
                case Size.BIG:
                    newScaleFactor = 3;
                    break;
                case Size.HUGE:
                    newScaleFactor = 5;
                    break;
            }
            scaleFactor = usingFinalState ? newScaleFactor : 1;
            world = Matrix.CreateScale(scaleFactor, scaleFactor, 1);
            world *= Matrix.CreateTranslation(usingFinalState ? lane : initialLane, 0, distanceTravelled);
        }

        internal bool covers(float queryLane)
        {
            int qLane = (int) Math.Round(queryLane);
            switch (size)
            {
                case Size.SMALL:
                    return qLane == lane;
                case Size.NORMAL:
                    return qLane == lane;
                case Size.BIG:
                    return Math.Abs(lane - qLane) <= 1;
                case Size.HUGE:
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
        public bool usingFinalState { get; set; }


        internal BoundingBox getBoundingBox()
        {
            Vector3 min = Vector3.Transform(new Vector3(0, 0, 0), world);
            Vector3 max = Vector3.Transform(new Vector3(scaleFactor * 0.9f, scaleFactor * 0.9f, 0.9f), world);
            return new BoundingBox(min, max);
        }
    }
}
