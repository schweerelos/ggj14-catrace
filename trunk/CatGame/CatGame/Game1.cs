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
        public enum State {INTRO, RUNNING, WINNER};

        State activeState = State.INTRO;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Ramp ramp;
        RainbowLighting rainbowLighting;
        KeyboardState oldState = Keyboard.GetState();
        const int numPlayers = 4;
        List<Player> players;
        private const double startingObstacleThreshold = 1500;
        private double newObstacleThreshold = startingObstacleThreshold;
        private double elapsedSinceLastObstacle;
        List<Obstacle> obstacles = new List<Obstacle>();

        Random randomSource = new Random();
        private int BONUS_WHEEL = 128;
        Texture2D bonusWheel;
        private int HEART_SIZE = 32;
        public static String[] CAT_NAMES = { "grumpy", "tongue", "pirate", "kitty" };
        private Texture2D heart;
        private Texture2D galaxy;
        private Texture2D cross;
        private Texture2D nyan;
        private Viewport[] viewports;
        private Viewport defaultViewport;
        IntroScreen intro;
        List<Player> deadPlayers;
        VictoryScreen winnerScreen;

        SoundEffect musicEffect;
        Song music;
        SpriteFont scoreFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1680;// Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = 1024;// Window.ClientBounds.Height;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            ramp = new Ramp();
            players = new List<Player>();
            deadPlayers = new List<Player>();
            rainbowLighting = new RainbowLighting();
            
            intro = new IntroScreen(this);
            activeState = State.INTRO;
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
            viewports = new Viewport[4];
            
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
            
            bonusWheel = Content.Load<Texture2D>("BonusWheel");
            heart = Content.Load<Texture2D>("Heart");
            cross = Content.Load<Texture2D>("cross");
            galaxy = Content.Load<Texture2D>("galaxy");
            nyan = Content.Load<Texture2D>("rainbow-kiwi");
            scoreFont = Content.Load<SpriteFont>("catfont");
            intro.LoadContent(Content);

            music = Content.Load<Song>("ForAGIng");
            //music = musicEffect.CreateInstance();
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

            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Contains(Keys.Escape))
                this.Exit();

            switch (activeState)
            {
                case State.INTRO:
                    intro.Update(gameTime);
                    break;
                case State.RUNNING:
                    updateGame(gameTime);
                    break;
                case State.WINNER:
                    winnerScreen.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void updateGame(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            // Allows the game to exit
            KeyboardState newState = Keyboard.GetState();

            foreach (Player p in players)
                p.update(gameTime);


            

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
                                    if (p.dead == true)
                                    {
                                        if (!deadPlayers.Contains(p))
                                            deadPlayers.Add(p);
                                        if (deadPlayers.Count() == players.Count())
                                        {
                                            int pNo = -1;
                                            for (int i = 0; i < players.Count(); i++)
                                            {
                                                if (players[i] == p)
                                                {
                                                    pNo = i;
                                                    break;
                                                }
                                            }
                                            winnerScreen = new VictoryScreen(this, pNo+1);
                                            MediaPlayer.Stop();
                                            
                                            activeState = State.WINNER;

                                        }
                                    }
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
                Obstacle newObst = new Obstacle(randomSource.Next(0, 7));
                newObst.LoadContent(Content);
                obstacles.Add(newObst);
                elapsedSinceLastObstacle = 0;
                if (newObstacleThreshold > 900)
                    newObstacleThreshold *= 0.995;
                //Console.WriteLine("obstacle threshold: " + newObstacleThreshold);
            }

            // TODO: Add your update logic here
            ramp.Update(delta);

            rainbowLighting.Update(delta);
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
            switch (activeState)
            {
                case State.INTRO:
                    GraphicsDevice.Viewport = defaultViewport;

                    intro.Draw(gameTime,spriteBatch, scoreFont);

                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                    
                    break;
                case State.RUNNING:
                    drawRunning(gameTime);
                    break;
                case State.WINNER:
                    GraphicsDevice.Viewport = defaultViewport;

                    winnerScreen.Draw(gameTime, spriteBatch, scoreFont);

                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                    break;
                    
            }

            base.Draw(gameTime);
        }

        private void drawRunning(GameTime gameTime)
        {

            // Draw each viewport
            for (int i = 0; i < players.Count(); i++)
            {
                DrawViewport(i, gameTime);
            }

            GraphicsDevice.Viewport = defaultViewport;


            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        private void DrawViewport(int i, GameTime gameTime)
        {
            // Each Viewport for each player is shown
            GraphicsDevice.Viewport = viewports[i];

            // Draw stars
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(galaxy, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(nyan, new Rectangle(GraphicsDevice.Viewport.Width / 2 + 20, GraphicsDevice.Viewport.Height / 2 - 15, nyan.Width / 2, nyan.Height / 2), Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // TODO: Add your drawing code here
            //world *= Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalMilliseconds / 1000f);
            Matrix view = Matrix.CreateLookAt(new Vector3(3, 3, 4), new Vector3(3, 0, -50), Vector3.Forward);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 1, 1000);

            ramp.Draw(gameTime, view, projection, rainbowLighting, players[i]);

            foreach (Obstacle o in obstacles)
            {
                o.Draw(gameTime, view, projection, rainbowLighting, players[i]);
            }

            players[i].Draw(gameTime, view, projection, rainbowLighting, players[i]);
            
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
            for (int j = 0; j < p.lives; j++)
            {
                Rectangle heartRect = new Rectangle(x + Player.AVATAR_SIZE + 10 + (j % 3) * HEART_SIZE, y + j / 3 * HEART_SIZE, HEART_SIZE, HEART_SIZE);
                spriteBatch.Draw(heart, heartRect, Color.White);
            }

            // Draw bonus wheel
            float rotation = p.getBonusRotation();
            Rectangle bonusRect = new Rectangle(x + BONUS_WHEEL / 2, y + Player.AVATAR_SIZE + 10 + BONUS_WHEEL / 2, BONUS_WHEEL, BONUS_WHEEL);
            spriteBatch.Draw(bonusWheel, bonusRect, null, Color.White, rotation, new Vector2(BONUS_WHEEL / 2), SpriteEffects.None, 0);

            // Draw score
            spriteBatch.DrawString(scoreFont, "Score: " + p.score, new Vector2(10, 20 + Player.AVATAR_SIZE + BONUS_WHEEL), Color.White);

            // Draw cross if player is dead
            if (p.dead)
            {
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

        internal Player registerPlayer(int controller, bool usesKeyboard)
        {
            players.Add(new Player((PlayerIndex)controller,usesKeyboard,this,CAT_NAMES[players.Count()]));
            Player p = players.Last();
            p.LoadContent(Content);
            return p;
            
        }

        internal void startGame()
        {
           
            // Nuke old intro screen with its state.
            intro = new IntroScreen(this);
            intro.LoadContent(Content);

            // reset obstacles state
            newObstacleThreshold = startingObstacleThreshold;
            elapsedSinceLastObstacle = 0;
            obstacles.Clear();

            for (int i = 0; i < players.Count(); i++)
            {
                int width = Window.ClientBounds.Width / 2;
                int height = Window.ClientBounds.Height / 2;
                if (players.Count() <= 2)
                    height *= 2;
                viewports[i] = new Viewport((i % 2) * width, (i / 2) * height, width, height);
            }
            if (players.Count() == 1)
                viewports[0] = defaultViewport;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
            
            activeState = State.RUNNING;
        }

        internal void restartGame()
        {
            players = new List<Player>();
            deadPlayers = new List<Player>();
            activeState = State.INTRO;
        }
    }
}
