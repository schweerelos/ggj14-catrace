using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CatGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Ramp ramp;
        KeyboardState oldState = Keyboard.GetState();
        const int numPlayers = 1;
        Player[] players = new Player[numPlayers];
        const double newObstacleThreshold = 1500;
        private double elapsedSinceLastObstacle;
        List<Obstacle> obstacles = new List<Obstacle>();

        Random randomSource = new Random();
        private int BONUS_WHEEL = 128;
        Texture2D bonusWheel;
        private int HEART_SIZE = 32;
        private Texture2D heart;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1024;
            graphics.PreferredBackBufferWidth = 1680;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            ramp = new Ramp();
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new Player(PlayerIndex.One, true,this);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ramp.LoadContent(Content);
            Obstacle.StaticLoadContent(Content);
            for (int i = 0; i < numPlayers; i++)
            {
                players[i].LoadContent(Content);
            }
            bonusWheel = Content.Load<Texture2D>("BonusWheel");
            heart = Content.Load<Texture2D>("Heart");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
         {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            // Allows the game to exit
            KeyboardState newState = Keyboard.GetState();

            foreach (Player p in players)
                p.update(gameTime);
            
            
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Contains(Keys.Escape))
                this.Exit();
            
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
              //  this.Exit();

            // Move the obstacles
            List<Obstacle> obstaclesCopy = new List<Obstacle>();
            foreach (Obstacle o in obstacles)
            {
                o.Update(delta);
                if (o.hasReached(3))
                {
                    foreach (Player p in players)
                    {
                        // TODO figure out if player is jumping
                        if (o.covers(p.getLane()))
                        {
                            try
                            {
                                p.takeHit();
                            }
                            catch (OutOfLivesException oole)
                            {
                                Console.WriteLine("Player dead");
                                this.Exit();
                            }
                        }
                        else
                        {
                            p.incrementSurvivedObstacles();
                        }
                    }
                }
                else
                {
                    // Next round will only have those obstacles that aren't at the bottom yet
                    obstaclesCopy.Add(o);
                }
            }
            obstacles = obstaclesCopy;

            // Add new obstacles when needed
            elapsedSinceLastObstacle += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedSinceLastObstacle + (randomSource.NextDouble() * 800) >= newObstacleThreshold)
            {
                obstacles.Add(new Obstacle(randomSource.Next(0,6)));
                elapsedSinceLastObstacle = 0;
            }

            // TODO: Add your update logic here
            ramp.Update(delta);
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            //world *= Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalMilliseconds / 1000f);
            Matrix view = Matrix.CreateLookAt(new Vector3(3, 3, 3), new Vector3(3,0,-5), Vector3.Forward);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 4f / 3f, 1, 1000);
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            ramp.Draw(delta, view, projection);

            foreach (Obstacle o in obstacles)
            {
                o.Draw(delta, view, projection);
            }

            foreach (Player p in players) {
                p.Draw(delta,view,projection);
            }

            // Sprite mode
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            DrawGUI();
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        private void DrawGUI()
        {
            for (int i = 0; i < players.Length; i++)
            {
                // Player avatar
                int x = 10 + (i % 2) * GraphicsDevice.Viewport.Width / 2;
                int y = 10;
                if (i >= 2)
                    y += GraphicsDevice.Viewport.Height / 2;
                Rectangle position = new Rectangle(x, y, Player.AVATAR_SIZE, Player.AVATAR_SIZE);
                spriteBatch.Draw(players[i].GetTexture(), position, Color.White);

                // Draw hearts
                for (int j = 0; j < players[i].lives; j++ )
                {
                    Rectangle heartRect = new Rectangle(x + Player.AVATAR_SIZE + 10 + (j % 3) * HEART_SIZE, y + j / 3 * HEART_SIZE, HEART_SIZE, HEART_SIZE);
                    spriteBatch.Draw(heart, heartRect, Color.White);
                }

                // Draw bonus wheel
                Player.Bonus selectedBonus = players[i].getBonus();
                float rotation = (float) (((int) selectedBonus / (float) Player.Bonus.LAST) * Math.PI*2);
                Rectangle bonusRect = new Rectangle(x + BONUS_WHEEL/2, y + Player.AVATAR_SIZE + 10 + BONUS_WHEEL/2, BONUS_WHEEL, BONUS_WHEEL);
                spriteBatch.Draw(bonusWheel, bonusRect, null, Color.White, rotation, new Vector2(BONUS_WHEEL/2), SpriteEffects.None, 0);
            }
        }

        internal void playerBarfsBonus(Player player)
        {
            List<Obstacle> candidates = new List<Obstacle>();
            foreach (Obstacle o in obstacles)
            {
                if (o.covers(player.getLane()))
                {
                    candidates.Add(o);
                }
            }
            candidates.Sort((a, b) => a.distanceTravelled.CompareTo(b.distanceTravelled));
            Obstacle ob = candidates.Last();
            Console.WriteLine("Barf " + ob.distanceTravelled);
            ob.increaseSize();
        }
    }
}
