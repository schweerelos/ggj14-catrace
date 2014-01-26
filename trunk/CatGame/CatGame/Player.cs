using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace CatGame
{
    class Player : ThreeDObject
    {
        public enum Bonus { SCALE_UP, MOVE_LEFT, SCALE_DOWN, MOVE_RIGHT, LAST };
        public static Color[] BONUS_COLORS = { Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.White };
        public static Color[] PLAYER_COLORS = { Color.Cyan, Color.LawnGreen, Color.Violet, Color.Yellow };
        Bonus activeBonus = Bonus.SCALE_UP;
        public Bonus prevBonus = Bonus.SCALE_UP;

        public PlayerIndex playerIndex;
        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;
        private Vector3 currentPos;
        private Vector3 startingPos;
        private Vector3 targetPos;
        private Game1 gameEngine;
        private int bounce;

        private float speed = 7f;
        private int startBounce = 2;
        private float yAccel = 0;
        private const float gravity = 20.82f;
        private const float jumpAccel = 9.82f;
        private float[] bounceAccel = new float[4]{1,2,4,6};
        
        public const int AVATAR_SIZE = 128;
        private Texture2D texture;
        private String textureFile;
        
        
        private float turnTime = 0;
        public const float BEAT_FREQ = 60f/130;
        public const float OFFSET = .15f;
        public const float BOUNCE_AMOUNT = .2f;

        public Player(PlayerIndex playerIndex, bool usesKeyboard, Game1 gameEngine, String textureFile) : base("cat") {

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

            if (targetPos.X >= 1 && (currentPos.Y == 0 || bounce < startBounce))
                targetPos.X--;
            
        }

        public void moveRight()
        {
            startingPos = currentPos;

            if (targetPos.X <= 5  && (currentPos.Y == 0 || bounce < startBounce))
                targetPos.X++;
        }

        public void jump()
        {
            if (currentPos.Y <= 0)
            {
                yAccel = jumpAccel;
                bounce = startBounce;
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
            if (!dead)
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

            if (turnTime > 0)
                turnTime = MathHelper.Clamp(turnTime - delta / 0.1f, 0, 4);
            if (turnTime < 0)
                turnTime = MathHelper.Clamp(turnTime + delta / 0.1f, -4, 0);

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
                
                if (newState.DPad.Left == ButtonState.Pressed &&
                    oldGamePadState.DPad.Left == ButtonState.Released)
                    this.moveLeft();
                if (newState.DPad.Right == ButtonState.Pressed &&
                    oldGamePadState.DPad.Right == ButtonState.Released)
                    this.moveRight();
                if (newState.Buttons.A == ButtonState.Pressed &&
                    oldGamePadState.Buttons.A == ButtonState.Released)
                    this.jump();
                if (newState.Buttons.X == ButtonState.Pressed &&
                    oldGamePadState.Buttons.X == ButtonState.Released)
                    this.barf();
                if (newState.Buttons.RightShoulder == ButtonState.Pressed &&
                    oldGamePadState.Buttons.RightShoulder == ButtonState.Released)
                    this.rotateActionsLeft();
                if (newState.Buttons.LeftShoulder == ButtonState.Pressed &&
                    oldGamePadState.Buttons.LeftShoulder == ButtonState.Released)
                    this.rotateActionsRight();
                oldGamePadState = newState;
            }
        }

        private void rotateActionsRight()
        {
            this.activeBonus--;
            if (activeBonus < 0)
                activeBonus = Player.Bonus.MOVE_RIGHT;
            turnTime += MathHelper.TwoPi / (int)Player.Bonus.LAST;
        }

        private void rotateActionsLeft()
        {
            this.activeBonus++;
            if (activeBonus == Player.Bonus.LAST)
                activeBonus = Player.Bonus.SCALE_UP;
            turnTime -= MathHelper.TwoPi / (int)Player.Bonus.LAST;
        }

        internal Texture2D GetTexture()
        {
            return texture;
        }

        internal void takeHit()
        {
            if (dead)
                return;

            if (lives <= 1)
            {
                this.dead = true;
            }
            lives--;
        }

        internal void incrementSurvivedObstacles()
        {
            if (!dead)
                score++;
        }



        public int score { get; set; }

        public int lives { get; set; }


        internal float getLane()
        {
            return currentPos.X;
        }

        public Player.Bonus getActiveBonus()
        {
            return activeBonus;
        }

        internal float getBonusRotation()
        {
            return (float)(((int)activeBonus / (float)Player.Bonus.LAST) * Math.PI * 2) + turnTime;
        }

        public bool dead { get; set; }

        public override void SetEffect(Matrix view, Matrix projection, RainbowLighting lighting, BasicEffect effect, Player activePlayer, GameTime gameTime)
        {
            base.SetEffect(view, projection, lighting, effect, activePlayer, gameTime);
            effect.DirectionalLight2.Enabled = true;
            effect.DirectionalLight2.Direction = Vector3.Down;
            effect.DirectionalLight2.DiffuseColor = Vector3.One * .5f;
            effect.SpecularPower = 1000;
            if ((int) playerIndex >= -1)
                effect.AmbientLightColor = PLAYER_COLORS[(int) playerIndex + 1].ToVector3();

        }
    }

}
