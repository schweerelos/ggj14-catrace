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

        public Player(Model avatar, PlayerIndex playerIndex, bool usesKeyboard){
            this.avatar = avatar;
            lane = 3;
            this.playerIndex = playerIndex;
            this.usesKeyboard = usesKeyboard;

            if (this.usesKeyboard)
                oldKeyboardState = Keyboard.GetState();
            else
                oldGamePadState = GamePad.GetState(playerIndex);

        }

        public void moveLeft()
        {
            if (lane >= 1)
                lane--;
        }

        public void moveRight()
        {
            if (lane <= 5)
                lane++;
        }

        public void draw(Matrix world, Matrix view, Matrix projection)
        {
            world = Matrix.CreateTranslation(lane, 0, 0);
            avatar.Draw(world, view, projection);
        }


        public bool usesKeyboard { get; set; }

        public void update(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;
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
    }
}
