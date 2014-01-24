using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CatGame
{
    class Player
    {
        private int lane = 3;
        private PlayerIndex playerIndex;
        private Model avatar;
        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;
        float currentPos, progress;
        private float startingPos;
        private float targetPos;
        private float speed = 0.90f;
        
        public Player(Model avatar, PlayerIndex playerIndex, bool usesKeyboard){
            this.avatar = avatar;
            lane = 3;
            startingPos = lane;
            targetPos = lane;
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
            progress = 0;
            if (lane >= 1)
                lane--;
            targetPos = lane;
            
        }

        public void moveRight()
        {
            startingPos = currentPos;
            progress = 0;
            if (lane <= 5)
                lane++;
            targetPos = lane;
        }

        public void draw(Matrix view, Matrix projection)
        {
            Matrix world = Matrix.Identity;
            
            world = Matrix.CreateTranslation(currentPos, 0, 0);
            avatar.Draw(world, view, projection);
        }


        public bool usesKeyboard { get; set; }

        public void update(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            progress = (progress + delta);
            if (progress >= speed)
            {
                progress = speed;
                startingPos = targetPos;
            }
            
            currentPos = MathHelper.Lerp(startingPos, targetPos, progress/speed);
            Console.WriteLine(startingPos + ", " + targetPos + ", " + currentPos);
            updateInputs();
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


        internal int getLane()
        {
            return lane;
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
        
    }
}
