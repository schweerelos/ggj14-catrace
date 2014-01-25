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
        const int numPlayers = 4;
        Player[] players;
        private double newObstacleThreshold = 1500;
        private double elapsedSinceLastObstacle;
        List<Obstacle> obstacles = new List<Obstacle>();

        Random randomSource = new Random();
        private int BONUS_WHEEL = 128;
        Texture2D bonusWheel;
        private int HEART_SIZE = 32;
        public static String[] CAT_NAMES = { "kitty", "tongue", "pirate", "grumpy" };
        private Texture2D heart;
        private Texture2D galaxy;
        private Texture2D cross;
        private Viewport[] viewports;
        private Viewport defaultViewport;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1680;// Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = 1024;// Window.ClientBounds.Height;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            ramp = new Ramp();
            players = new Player[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new Player((PlayerIndex) i-1 , i == 0, this, CAT_NAMES[i]);
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
            defaultViewport = GraphicsDevice.Viewport;
            viewports = new Viewport[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                int width = Window.ClientBounds.Width / 2;
                int height = Window.ClientBounds.Height / 2;
                if (numPlayers <= 2)
                    height *= 2;
                viewports[i] = new Viewport((i % 2) * width, (i / 2) * height, width, height);
            }
            if (numPlayers == 1)
                viewports[0] = defaultViewport;

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
            cross = Content.Load<Texture2D>("cross");
            galaxy = Content.Load<Texture2D>("galaxy");
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
                if (o.hasReached(0))
                {
                    if (!o.collisionTested)
                    {
                        foreach (Player p in players)
                        {
                            if (isCollision(o, p))
                            {
                               
                                    p.takeHit();
                                    
                            }
                            else
                            {
                                p.incrementSurvivedObstacles();
                            }
                        }
                        o.collisionTested = true;
                    }
                }
                else if (o.hasReached(-15))
                {
                    o.usingFinalState = true;
                }
                if (!o.hasReached(3))
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
                obstacles.Add(new Obstacle(randomSource.Next(0,7)));
                elapsedSinceLastObstacle = 0;
                newObstacleThreshold *= 0.995;
                //Console.WriteLine("obstacle threshold: " + newObstacleThreshold);
            }

            // TODO: Add your update logic here
            ramp.Update(delta);
            

            base.Update(gameTime);
        }
                
        private bool isCollision(Obstacle o, Player p)
        {
            BoundingBox obstacleBounds = o.getBoundingBox();
            BoundingBox playerBounds = new BoundingBox(Vector3.Transform(new Vector3(0, 0, 0), p.world), Vector3.Transform(new Vector3(0.9f, 0.9f, 0.9f), p.world));

            return (obstacleBounds.Intersects(playerBounds));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            // Draw each viewport
            for (int i = 0; i < players.Length; i++)
            {
                DrawViewport(i, delta);
            }

            GraphicsDevice.Viewport = defaultViewport;

            
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }

        private void DrawViewport(int i, float delta)
        {
            // Each Viewport for each player is shown
            GraphicsDevice.Viewport = viewports[i];

            // Draw stars
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Draw(galaxy, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // TODO: Add your drawing code here
            //world *= Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalMilliseconds / 1000f);
            Matrix view = Matrix.CreateLookAt(new Vector3(3, 3, 4), new Vector3(3, 0, -50), Vector3.Forward);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 1, 1000);

            ramp.Draw(delta, view, projection);

            foreach (Obstacle o in obstacles)
            {
                o.Draw(delta, view, projection);
            }

            players[i].Draw(delta, view, projection);
            
            // Sprite mode
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            DrawGUI(players[i]);
            spriteBatch.End();

        }

        private void DrawGUI(Player p)
        {
            
                // Player avatar
                int x = 10;
                int y = 10;
                
                Rectangle position = new Rectangle(x, y, Player.AVATAR_SIZE, Player.AVATAR_SIZE);
                spriteBatch.Draw(p.GetTexture(), position, Color.White);
                

                // Draw hearts
                for (int j = 0; j < p.lives; j++ )
                {
                    Rectangle heartRect = new Rectangle(x + Player.AVATAR_SIZE + 10 + (j % 3) * HEART_SIZE, y + j / 3 * HEART_SIZE, HEART_SIZE, HEART_SIZE);
                    spriteBatch.Draw(heart, heartRect, Color.White);
                }

                // Draw bonus wheel
                float rotation = p.getBonusRotation();
                Rectangle bonusRect = new Rectangle(x + BONUS_WHEEL/2, y + Player.AVATAR_SIZE + 10 + BONUS_WHEEL/2, BONUS_WHEEL, BONUS_WHEEL);
                spriteBatch.Draw(bonusWheel, bonusRect, null, Color.White, rotation, new Vector2(BONUS_WHEEL/2), SpriteEffects.None, 0);

                // Draw cross if player is dead
                if (p.dead)
                {
                    Console.Write("Drawing cross");

                    Rectangle crossRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                    spriteBatch.Draw(cross, crossRect, Color.White);
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
            if (candidates.Count() == 0)
                return;

            candidates.Sort((a, b) => a.distanceTravelled.CompareTo(b.distanceTravelled));
            
            Obstacle ob = candidates.Last();
            
            switch (player.getActiveBonus())
            {
                case Player.Bonus.SCALE_UP:
                    ob.increaseSize(player);
                    break;
                case Player.Bonus.SCALE_DOWN:
                    ob.decreaseSize(player);
                    break;
                case Player.Bonus.MOVE_LEFT:
                    ob.moveLeft(player);
                    break;
                case Player.Bonus.MOVE_RIGHT:
                    ob.moveRight(player);
                    break;
                default:
                    Console.WriteLine("Barf fallthrough, player bonus state is wrong");
                    break;
            }
            
        }
    }
}
