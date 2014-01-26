using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CatGame
{
    class VictoryScreen
    {

        Game1 gameEngine;
        GamePadState[] oldGamePadStates = new GamePadState[4];
        KeyboardState oldKeyboardState;
        int winningPlayer = 0;
        List <Player> players;
        float scale = startingScale;
        const float startingScale = 5;
        //Texture2D splash;
        float direction;
        
        public VictoryScreen(Game1 gameEngine, int winningPlayer)
        {
            this.gameEngine = gameEngine;
            for (int i = 0; i < 4; i++)
            {
                oldGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
            oldKeyboardState = Keyboard.GetState();
            
            players = new List<Player>();
            this.winningPlayer = winningPlayer;
            direction = 1;

        }

        public void LoadContent(ContentManager content)
        {

            //splash = content.Load<Texture2D>("splash");
        }



        public void Draw(GameTime gameTime,SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

            String playerAnnounce = "Player " + winningPlayer + " wins!";

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                 
            Vector2 textPos = new Vector2(300,400);
            Vector2 textOrigin = new Vector2(0, 0);
            

            spriteBatch.DrawString(spriteFont, playerAnnounce, textPos, Color.White, 0, textOrigin, scale, SpriteEffects.None, 0);
                       
            scale += direction * 0.02f;
            if (scale > 5 || scale < 3)
            {
                 direction *= -1;
            }

            
            spriteBatch.End();

        }


        public void Update(GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
      
                    if ((!newKeyboardState.IsKeyDown(Keys.Space) && oldKeyboardState.IsKeyDown(Keys.Space)) ||
                        (!newKeyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyDown(Keys.Enter)) )
                    {
                       gameEngine.restartGame();
                    }
                    
                    for (int i = 0; i < 4; i++)
                    {
                        GamePadState newGamePadState = GamePad.GetState((PlayerIndex)i);
                        if (newGamePadState.IsConnected && newGamePadState.Buttons.Start == ButtonState.Released &&
                            oldGamePadStates[i].Buttons.Start == ButtonState.Pressed)
                        {
                            gameEngine.restartGame();
                        }
                        oldGamePadStates[i] = newGamePadState;
                    }
                    
                    
                    oldKeyboardState = newKeyboardState;

        }
    }
}
