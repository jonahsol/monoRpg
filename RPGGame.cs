using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

/// 
/// R P G 
/// 

namespace Rpg
{
    public class RPGGame : Game
    {   
        // input

        public KeyboardState _keyboardState;
        
        // graphics 

        public GraphicsDeviceManager graphics;
        public static int _screenWidth = 1024;
        public static int _screenHeight = 1024;

        public static SpriteBatch spriteBatch;
        public TmxMap map;
        public Texture2D tilesetTxtr;

        // entity component system

        public EntityComponentSystem.EntityComponentStorage ecs;
        public EntityComponentSystem.Entity player;
        public EntityComponentSystem.Entity testEnemy;

        public RPGGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = _screenWidth;
            graphics.PreferredBackBufferHeight = _screenHeight;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Load initial non graphics etc. related content.
        /// </summary>
        protected override void Initialize()
        {
            // init input
            _keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            this.IsMouseVisible = true;

            // init ecs
            this.ecs = new EntityComponentSystem.EntityComponentStorage();
            this.player = ecs.AddEntity();
            this.testEnemy = ecs.AddEntity();

            // enumerate through monogame components (not used). call <see cref="LoadContent()"/>
            base.Initialize();
        }

        /// <summary>
        /// Load initial game content
        /// </summary>
        protected override void LoadContent()
        {
            // spritebatch used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load sprite sheet currently in use
            tilesetTxtr = Content.Load<Texture2D>("dungeon_test");

            // load tiled map data
            this.map = new TmxMap("Content/dungeon_test.tmx");

            // add test enemy

            ecs.AddComponentsToEntity(testEnemy.Eid,
                                      new EntityComponentSystem.RenderComponent(ecs,
                                                tilesetTxtr, new Rectangle(5 * 16, 9 * 16, 16, 16)),
                                      new EntityComponentSystem.PositionComponent(ecs,
                                                                             new Vector2(300, 300))
                                     );

            var testEnemyPos = ((EntityComponentSystem.PositionComponent)testEnemy.
                            Components[ecs.ComponentCids[
                            typeof(EntityComponentSystem.PositionComponent)]]).Position;

            ecs.AddComponentsToEntity(testEnemy.Eid, new EntityComponentSystem.CollisionComponent(ecs,
                                                            Vector2.Zero, new Vector2(16, 16)));
            
            // add player

            ecs.AddComponentsToEntity(player.Eid,
                                      new EntityComponentSystem.RenderComponent(ecs, 
                                                tilesetTxtr, new Rectangle(4 * 16, 8 * 16, 16, 16)),
                                      new EntityComponentSystem.PositionComponent(ecs, 
                                                                             new Vector2(200, 200)),
                                      new EntityComponentSystem.MovementComponent(ecs,
                                                                             new Vector2(0, 0),
                                                                             new Vector2(6, 6)),
                                      new EntityComponentSystem.InputComponent(ecs)
                                     );

            var playerPos = ((EntityComponentSystem.PositionComponent)player.
                            Components[ecs.ComponentCids[
                            typeof(EntityComponentSystem.PositionComponent)]]).Position;

            ecs.AddComponentsToEntity(player.Eid, 
                                      new EntityComponentSystem.CameraComponent(
                                                        ecs, 
                                                        player.Eid,
                                                        new Vector3(RPGGame._screenWidth / 2, 
                                                                      RPGGame._screenHeight / 2, 0),
                                                        new Vector3(playerPos.X, playerPos.Y, 0),
                                                        2.5f
                                                                               )
                                      );
            ecs.AddComponentsToEntity(player.Eid,
                                      new EntityComponentSystem.CollisionComponent(ecs, Vector2.Zero, new Vector2(16,16)));
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
            _keyboardState = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            EntityComponentSystem.InputSystem.Update(ecs, _keyboardState);
            EntityComponentSystem.PositionSystem.Update(ecs, gameTime);
            EntityComponentSystem.CameraSystem.Update(ecs, gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // draw game map and run render system
            spriteBatch.Begin(SpriteSortMode.Deferred, 
                              null, 
                              SamplerState.PointClamp, 
                              null,
                              null, 
                              null, 
                              EntityComponentSystem.CameraSystem.CurTransformMatrix);
            

            TiledMapReader.DrawMap(map, tilesetTxtr, spriteBatch);
            EntityComponentSystem.RenderSystem.Draw(ecs);
            EntityComponentSystem.CollisionSystem.Update(this, ecs, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
