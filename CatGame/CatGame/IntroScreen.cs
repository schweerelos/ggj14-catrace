using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CatGame
{
    class IntroScreen
    {

        public enum IntroState { HELP, REGISTERING };

        Game1 gameEngine;
        GamePadState[] oldGamePadStates = new GamePadState[4];
        KeyboardState oldKeyboardState;
        IntroState state;
        List <Player> players;

        Texture2D splash;
        GraphicsDevice GraphicsDevice;
        
        public IntroScreen(Game1 gameEngine,GraphicsDevice GraphicsDevice)
        {
            this.gameEngine = gameEngine;
            for (int i = 0; i < 4; i++)
            {
                oldGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
            oldKeyboardState = Keyboard.GetState();
            state = IntroState.HELP;
            players = new List<Player>();
            this.GraphicsDevice = GraphicsDevice;
        }

        public void LoadContent(ContentManager content)
        {

            splash = content.Load<Texture2D>("splash");
        }



        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
       
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            Rectangle splashRect = new Rectangle(0, 0, 1680, 1024);
            spriteBatch.Draw(splash, splashRect, Color.White);

            spriteBatch.End();
        }


        public void Update(GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            switch (state)
            {
                case (IntroState.HELP):
                    
                    if ((newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space)) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && !oldKeyboardState.IsKeyDown(Keys.Enter)) )
                    {
                        state = IntroState.REGISTERING;
                    }
                    
                    for (int i = 0; i < 4; i++)
                    {
                        GamePadState newGamePadState = GamePad.GetState((PlayerIndex)i);
                        if (newGamePadState.IsConnected && newGamePadState.Buttons.Start == ButtonState.Pressed &&
                            oldGamePadStates[i].Buttons.Start == ButtonState.Released)
                        {
                            state = IntroState.REGISTERING;
                        }
                        oldGamePadStates[i] = newGamePadState;
                    }
                    break;
                case IntroState.REGISTERING:
                    
                    if ((newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space)) ||
                        (newKeyboardState.IsKeyDown(Keys.Enter) && !oldKeyboardState.IsKeyDown(Keys.Enter)) )
                    {
                        foreach (Player p in players)
                        {
                            if (p.usesKeyboard)
                            {
                                gameEngine.startGame();
                                return;
                            }
                        }
                        players.Add(gameEngine.registerPlayer(-1, true));
                    }
                    oldKeyboardState = newKeyboardState;

                    for (int i = 0; i < 4; i++)
                    {
                        GamePadState newGamePadState = GamePad.GetState((PlayerIndex)i);
                        if (newGamePadState.IsConnected && newGamePadState.Buttons.Start == ButtonState.Pressed &&
                            oldGamePadStates[i].Buttons.Start == ButtonState.Released)
                        {
                            foreach (Player p in players)
                            {
                                if ((int)p.playerIndex == i)
                                {
                                    gameEngine.startGame();
                                    return;
                                }
                            }
                            players.Add(gameEngine.registerPlayer(i, false));
                        }
                        oldGamePadStates[i] = newGamePadState;
                    }
                    break;
            }
            oldKeyboardState = newKeyboardState;
        }
    }
}
