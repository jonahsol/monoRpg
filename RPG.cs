using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;
using System.Collections;

/// 
/// R P G
///

namespace TestGame
{
    public class RPGGame : Game
    {
        
        public KeyboardState input;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TmxMap map;
        Texture2D tilesetTxtr;
   
        ArrayList entities;

        public RPGGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public Texture2D getTileset()
        {
            return tilesetTxtr;
        }

        /// <summary>
        /// Load initial non graphics etc. related content.  Calling base.Initialize 
        /// will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            input = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            this.IsMouseVisible = true;

            // initialise entities
            entities = new ArrayList();
            entities.Add(new Player(16 * 7, 16 * 12));

            base.Initialize();
        }

        /// <summary>
        /// Load initial game content
        /// </summary>
        protected override void LoadContent()
        {
            // tiled map data
            this.map = new TmxMap("Content/dungeon_test.tmx");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tilesetTxtr = Content.Load<Texture2D>("dungeon_test");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>s
        /// Update input & entities.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Entity entity in entities)
            {
                entity.Update(gameTime, input);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                                                Matrix.CreateScale(2.0f));

            // draw background tiles
            TiledMapReader.DrawMap(map, tilesetTxtr, spriteBatch);

            // draw sprites
            foreach (Entity entity in entities)
            {
                entity.Draw(tilesetTxtr, spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
