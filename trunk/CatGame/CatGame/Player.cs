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

        private PlayerIndex playerIndex;
        private Model avatar;
        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;
        float currentPos;
        private float startingPos;
        private float targetPos;
        private float speed = 2f;
        
        public Player(Model avatar, PlayerIndex playerIndex, bool usesKeyboard){
            this.avatar = avatar;

            startingPos = 3;
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

            if (targetPos >= 1)
                targetPos--;
            
        }

        public void moveRight()
        {
            startingPos = currentPos;

            if (targetPos <= 5)
                targetPos++;
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
            updateInputs();

            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;
                        
            float direction = Math.Sign(targetPos - startingPos);
            float deltaPos = delta * speed;

            currentPos = direction > 0 ? Math.Min(currentPos + deltaPos, targetPos) : Math.Max(currentPos - deltaPos, targetPos);
            
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
            return currentPos;
        }
    }
}
