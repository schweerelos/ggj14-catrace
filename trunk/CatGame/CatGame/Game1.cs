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
        Model cube;
        Ramp ramp;
        KeyboardState oldState = Keyboard.GetState();
        const int numPlayers = 1;
        Player[] players = new Player[numPlayers];
        const double newObstacleThreshold = 1900;
        private double elapsedSinceLastObstacle;
        List<Obstacle> obstacles = new List<Obstacle>();

        Random randomSource = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1024;
            graphics.PreferredBackBufferWidth = 1680;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            ramp = new Ramp();
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
            for (int i = 0; i < numPlayers; i++)
            {
                players[i] = new Player(cube,PlayerIndex.One,true);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            cube = Content.Load<Model>("cube");
            // Turn on default lighting in the BasicEffects used by the model
            foreach (ModelMesh mesh in cube.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            ramp.LoadContent(Content);
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

            foreach (Obstacle o in obstacles)
            {
                o.progress(delta * 10);
            }

            // Update the obstacles
            elapsedSinceLastObstacle += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedSinceLastObstacle >= newObstacleThreshold)
            {
                // make a new obstacle
                obstacles.Add(new Obstacle(cube, randomSource.Next(0,6)));
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
            Matrix world = Matrix.Identity;
            //world *= Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalMilliseconds / 1000f);
            Matrix view = Matrix.CreateLookAt(new Vector3(3, 3, 3), new Vector3(3,0,-5), Vector3.Forward);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 4f / 3f, 1, 1000);

            ramp.Draw(view, projection);

            foreach (Obstacle o in obstacles)
            {
                o.draw(view, projection);
            }

            foreach (Player p in players) {
                p.draw(world,view,projection);
            }

            base.Draw(gameTime);
        }
    }
}
