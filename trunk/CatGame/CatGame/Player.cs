using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CatGame
{
    class Player : ThreeDObject
    {

        private PlayerIndex playerIndex;
        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;
        private Vector3 currentPos;
        private Vector3 startingPos;
        private Vector3 targetPos;
        private float speed = 5f;
        private float yAccel = 0;
        private const float gravity = 20.82f;
        private const float jumpAccel = 9.82f;

        public const int AVATAR_SIZE = 128;
        private Texture2D texture;

        public Player(PlayerIndex playerIndex, bool usesKeyboard) : base("cube") {

            startingPos = new Vector3(3,0,0);
            targetPos = startingPos;
            lives = 9;
            score = 0;
            this.playerIndex = playerIndex;
            this.usesKeyboard = usesKeyboard;

            if (this.usesKeyboard)
                oldKeyboardState = Keyboard.GetState();
            else
                oldGamePadState = GamePad.GetState(playerIndex);
        }

        public void moveLeft()
        {
            startingPos = currentPos;

            if (targetPos.X >= 1)
                targetPos.X--;
            
        }

        public void moveRight()
        {
            startingPos = currentPos;

            if (targetPos.X <= 5)
                targetPos.X++;
        }

        public void jump()
        {
            if(yAccel <= 0)
                yAccel = jumpAccel;
        }

        public bool usesKeyboard { get; set; }

        public void update(GameTime gameTime)
        {
            updateInputs();

            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;
                        
            // X position
            float direction = Math.Sign(targetPos.X - startingPos.X);
            float deltaPos = delta * speed;

            currentPos.X = direction > 0 ? Math.Min(currentPos.X + deltaPos, targetPos.X) : Math.Max(currentPos.X - deltaPos, targetPos.X);

            // Y position
            currentPos.Y = Math.Max(0, currentPos.Y + delta * yAccel);
            yAccel = yAccel - gravity * delta;

            world = Matrix.CreateTranslation(currentPos);
        }

        private void updateInputs()
        {

            if (this.usesKeyboard)
            {
                KeyboardState newState = Keyboard.GetState();
                if (newState.IsKeyDown(Keys.Left) && !oldKeyboardState.IsKeyDown(Keys.Left))
                    this.moveLeft();
                if (newState.IsKeyDown(Keys.Right) && !oldKeyboardState.IsKeyDown(Keys.Right))
                    this.moveRight();
                if (newState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))
                    this.jump();
                oldKeyboardState = newState;
            }
            else
            {
                GamePadState newState = GamePad.GetState(playerIndex);

                if (newState.Buttons.LeftShoulder == ButtonState.Pressed &&
                    oldGamePadState.Buttons.LeftShoulder == ButtonState.Released)
                    this.moveLeft();
                if (newState.Buttons.RightShoulder == ButtonState.Pressed &&
                    oldGamePadState.Buttons.RightShoulder == ButtonState.Released)
                    this.moveRight();

                oldGamePadState = newState;
            }
        }

        internal Texture2D GetTexture()
        {
            return texture;
        }

        internal void takeHit()
        {
            if (lives <= 1)
            {
                throw new OutOfLivesException();
            }
            lives--;
            Console.WriteLine("Lives: " + lives);
        }

        internal void incrementSurvivedObstacles()
        {
            score++;
            Console.WriteLine("Score: " + score);
        }



        public int score { get; set; }

        public int lives { get; set; }


        internal float getLane()
        {
            return currentPos.X;
        }

        
    }
}
