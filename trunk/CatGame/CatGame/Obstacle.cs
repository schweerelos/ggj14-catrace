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

        private const float TRANSFORM_TIME = 0.2f;
        private const bool freezeInFinalState = false;

        private int lane;
        private int initialLane;
        private Size size;
        public float distanceTravelled;
        private float scaleFactor;
        private Dictionary<Player, Player.Bonus> playerHasBarfed = new Dictionary<Player, Player.Bonus>();
        private Dictionary<Player, float> playerElapsedBarf = new Dictionary<Player, float>();
        private float elapsedScale;
        private float elapsedTrans;

        public Obstacle(int lane) : base("")
        {
            filename = "twine";
            this.lane = lane;
            initialLane = lane;
            size = Size.NORMAL;
            distanceTravelled = -100;
            world = Matrix.CreateTranslation(initialLane, 0, distanceTravelled);
            this.collisionTested = false;
            this.usingFinalState = false;
        }

        public void moveLeft(Player player)
        {
            if (usingFinalState && freezeInFinalState)
                return;
            if (hasBeenHitByPlayer(player))
                return;
            playerHasBarfed.Add(player, Player.Bonus.MOVE_LEFT);
            elapsedTrans = 0;

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

        public void moveRight(Player player)
        {
            if (usingFinalState && freezeInFinalState)
                return;
            if (hasBeenHitByPlayer(player))
                return;
            playerHasBarfed.Add(player, Player.Bonus.MOVE_RIGHT);
            elapsedTrans = 0;

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

        public void increaseSize(Player player)
        {
            if (usingFinalState && freezeInFinalState)
                return;
            if (hasBeenHitByPlayer(player))
                return;
            playerHasBarfed.Add(player, Player.Bonus.SCALE_UP);
            elapsedScale = 0;

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

        public void decreaseSize(Player player)
        {
            if (usingFinalState && freezeInFinalState)
                return;
            if (hasBeenHitByPlayer(player))
                return;
            playerHasBarfed.Add(player, Player.Bonus.SCALE_DOWN);
            elapsedScale = 0;

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
                    newScaleFactor = 0.5f;
                    break;
                case Size.BIG:
                    newScaleFactor = 3;
                    break;
                case Size.HUGE:
                    newScaleFactor = 5;
                    break;
            }

            scaleFactor = 1;
            //if (usingFinalState)
            //{
                scaleFactor = newScaleFactor;
            //}
            world = Matrix.Identity;
            float lerpScaleFactor = MathHelper.Lerp(1, scaleFactor, elapsedScale / TRANSFORM_TIME);
            world = Matrix.CreateScale(lerpScaleFactor);
            world *= Matrix.CreateTranslation(lane, 0, distanceTravelled);

            elapsedScale = Math.Min(elapsedScale + delta, TRANSFORM_TIME);
            elapsedTrans = Math.Min(elapsedTrans + delta, TRANSFORM_TIME);
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

        public override void SetEffect(Matrix view, Matrix projection, RainbowLighting lighting, BasicEffect effect, Player activePlayer, GameTime gameTime)
        {
            base.SetEffect(view, projection, lighting, effect, activePlayer, gameTime);
            effect.DirectionalLight0.Direction = Vector3.Down;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            if (hasBeenHitByPlayer(activePlayer))
            {
                effect.EmissiveColor = Player.BONUS_COLORS[(int)playerHasBarfed[activePlayer]].ToVector3();
                //float selectedScale = 0.9f + (float) (Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 100) * 0.1);
                //effect.World = Matrix.CreateScale(selectedScale) * effect.World;
            }
            else
            {
                effect.EmissiveColor = Color.Black.ToVector3();
            }
        }

        public bool collisionTested { get; set; }
        public bool usingFinalState { get; set; }


        internal BoundingBox getBoundingBox()
        {
            Vector3 min = Vector3.Transform(new Vector3(-.45f, -.45f, -.45f), world);
            Vector3 max = Vector3.Transform(new Vector3(.45f, .45f, .45f), world);
            return new BoundingBox(min, max);
        }

        internal bool hasBeenHitByPlayer(Player player)
        {
            return playerHasBarfed.ContainsKey(player);
        }

        internal float GetScale()
        {
            return scaleFactor;
        }
    }
}
