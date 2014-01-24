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
        Model ramp;
        KeyboardState oldState = Keyboard.GetState();
        const int numPlayers = 1;
        Player[] players = new Player[numPlayers];
        List<Obstacle> obstacles = new List<Obstacle>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
                players[i] = new Player(cube);
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
            ramp = Content.Load<Model>("ramp");
            foreach (ModelMesh mesh in ramp.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
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
            // Allows the game to exit
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Left) && !oldState.IsKeyDown(Keys.Left))
                players[0].moveLeft();


            if (newState.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
                players[0].moveRight();

            float delta = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            oldState = newState;

            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Contains(Keys.Escape))
                this.Exit();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSalmon);

            // TODO: Add your drawing code here
            Matrix world = Matrix.Identity;
            //world *= Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalMilliseconds / 1000f);
            Matrix view = Matrix.CreateLookAt(new Vector3(3, 3, 3), new Vector3(3,0,-10), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 4f / 3f, 1, 1000);

            ramp.Draw(Matrix.CreateTranslation(3, -1, 5), view, projection);

            foreach (Player p in players) {
                p.draw(world,view,projection);
            }

            base.Draw(gameTime);
        }
    }
}
