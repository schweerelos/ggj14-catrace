using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CatGame
{
    class Player : ThreeDObject
    {
        public enum Bonus { SCALE_UP, MOVE_LEFT, SCALE_DOWN, MOVE_RIGHT, LAST };
        Bonus activeBonus = Bonus.SCALE_UP;

        private PlayerIndex playerIndex;
        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;
        private Vector3 currentPos;
        private Vector3 startingPos;
        private Vector3 targetPos;
        private Game1 gameEngine;
        private int bounce;

        private float speed = 7f;
        
        private float yAccel = 0;
        private const float gravity = 20.82f;
        private const float jumpAccel = 9.82f;
        private float[] bounceAccel = new float[4]{1,2,4,6};
        
        public const int AVATAR_SIZE = 128;
        private Texture2D texture;
        private String textureFile;
        private bool p;
        private Game1 game1;
        private string p_2;

        public Player(PlayerIndex playerIndex, bool usesKeyboard, Game1 gameEngine, String textureFile) : base("cube") {

            startingPos = new Vector3(3,0,0);
            targetPos = startingPos;
            lives = 9;
            score = 0;

            this.playerIndex = playerIndex;
            this.usesKeyboard = usesKeyboard;
            this.gameEngine = gameEngine;
            this.textureFile = textureFile;

            if (this.usesKeyboard)
                oldKeyboardState = Keyboard.GetState();
            else
                oldGamePadState = GamePad.GetState(playerIndex);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            texture = content.Load<Texture2D>(textureFile);
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
            if (currentPos.Y <= 0)
            {
                yAccel = jumpAccel;
                bounce = 2;
            }
        }

        public void barf()
        {
            Console.WriteLine("Barf>> " + activeBonus);
            gameEngine.playerBarfsBonus(this);
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

            if (bounce >= 0 && currentPos.Y <= 0)
            {
                yAccel = bounceAccel[bounce];
                bounce--;
            }

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

                if (newState.IsKeyDown(Keys.LeftControl) && !oldKeyboardState.IsKeyDown(Keys.LeftControl))
                    this.barf();

                if (newState.IsKeyDown(Keys.RightControl) && !oldKeyboardState.IsKeyDown(Keys.RightControl))
                    this.barf();

                if (newState.IsKeyDown(Keys.D) && !oldKeyboardState.IsKeyDown(Keys.D))
                {
                    rotateActionsLeft();
                }
                if (newState.IsKeyDown(Keys.A) && !oldKeyboardState.IsKeyDown(Keys.A))
                {
                    rotateActionsRight();
                }

                
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
                if (newState.Buttons.A == ButtonState.Pressed &&
                    oldGamePadState.Buttons.A == ButtonState.Released)
                    this.jump();
                if (newState.Buttons.Y == ButtonState.Pressed &&
                    oldGamePadState.Buttons.Y == ButtonState.Released)
                    this.barf();
                if (newState.Buttons.B == ButtonState.Pressed &&
                    oldGamePadState.Buttons.B == ButtonState.Released)
                    this.rotateActionsRight();
                if (newState.Buttons.X == ButtonState.Pressed &&
                    oldGamePadState.Buttons.X == ButtonState.Released)
                    this.rotateActionsLeft();
                oldGamePadState = newState;
            }
        }

        private void rotateActionsRight()
        {
            this.activeBonus--;
            if (activeBonus < 0)
                activeBonus = Player.Bonus.MOVE_RIGHT;
        }

        private void rotateActionsLeft()
        {
            this.activeBonus++;
            if (activeBonus == Player.Bonus.LAST)
                activeBonus = Player.Bonus.SCALE_UP;
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

        internal Bonus getBonus()
        {
            return activeBonus;
        }
        public Player.Bonus getActiveBonus()
        {
            return activeBonus;
        }
    }

}
