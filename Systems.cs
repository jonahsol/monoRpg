using System.Collections.Generic;
using System;
using Eid = System.UInt16;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Rpg
{
    namespace EntityComponentSystem
    {
        /// Systems summary - dependencies.


        /// <summary>
        /// Collision system handles all in game collisions.
        /// 
        /// TODO: Add events handling
        /// </summary>
        public static class CollisionSystem
        {
            public static void Update(Game game, EntityComponentStorage ecs, GameTime gameTime)
            {
                List<Eid> collisionCandidates = ecs.ComponentEids[
                                                    ecs.ComponentCids[typeof(CollisionComponent)]];

                for (int i = 0; i < collisionCandidates.Count; i++)
                {
                    for (int j = i + 1; j < collisionCandidates.Count; j++)
                    {
                        if (EntitiesCollide(game, ecs, ecs.Entities[collisionCandidates[i]],
                                                       ecs.Entities[collisionCandidates[j]]))
                        {
                            System.Diagnostics.Debug.WriteLine("yay");
                        }                
                    }
                }
            }

            /// <summary>
            /// Given two entities, return true if entities' collision boxes intersect.
            /// 
            /// Bounding box used in collisions is derived from collision & position components. 
            /// Top left corner of bounding box is given by sum of 
            /// <see cref="PositionComponent.Position"/> & 
            /// <see cref="CollisionComponent.ColBoxOffset"/>
            /// Size of bounding box is given by <see cref="CollisionComponent.ColBoxSize"/>
            /// </summary>
            private static bool EntitiesCollide(Game game, EntityComponentStorage ecs, Entity entity1, Entity entity2)
            {
                CollisionComponent col1 = ((CollisionComponent)entity1.Components[ecs.ComponentCids[typeof(CollisionComponent)]]);
                CollisionComponent col2 = ((CollisionComponent)entity2.Components[ecs.ComponentCids[typeof(CollisionComponent)]]);
                PositionComponent pos1 = ((PositionComponent)entity1.Components[ecs.ComponentCids[typeof(PositionComponent)]]);
                PositionComponent pos2 = ((PositionComponent)entity2.Components[ecs.ComponentCids[typeof(PositionComponent)]]);

                // need position and collision components
                if (col1 != null && pos1 != null && col2 != null && pos2 != null)
                {
                    DrawTestRectangle(game, pos1, col1);
                    DrawTestRectangle(game, pos2, col2);

                    // Collision occurs -
                    // (r1.x2 > r2.x1 && r1.x1 < r2.x2 && r1.y1 > r2.y2 && r1.y1 < r2.y2)
                    return (((pos1.Position.X + col1.ColBoxOffset.X + col1.ColBoxSize.X) >= (pos2.Position.X + col2.ColBoxOffset.X)) &&
                             ((pos1.Position.X + col1.ColBoxOffset.X) <= (pos2.Position.X + col2.ColBoxOffset.X + col2.ColBoxSize.X)) &&
                             ((pos1.Position.Y + col1.ColBoxOffset.Y + col1.ColBoxSize.Y) >= (pos2.Position.Y + col2.ColBoxOffset.Y)) &&
                             ((pos1.Position.Y + col1.ColBoxOffset.Y) <= (pos2.Position.Y + col2.ColBoxOffset.Y + col2.ColBoxSize.Y))
                            );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Sorry Sir! Null component reference " +
                                                       "in collision system.");
                    return false;
                }
            }
            
            // super not well done atm - find way to do better or something - need to be able to see hit boxes. MAYBE FIX BY NOT USING VECTORS IN DrawLine
            private static void DrawTestRectangle(Game game, PositionComponent posComp, CollisionComponent colComp)
            {
                Vector2 _topLeft = posComp.Position + colComp.ColBoxOffset; 
                Vector2 _topRight = posComp.Position + colComp.ColBoxOffset + new Vector2(colComp.ColBoxSize.X, 0);
                Vector2 _botLeft = posComp.Position + colComp.ColBoxOffset + new Vector2(0, colComp.ColBoxSize.Y);
                Vector2 _botRight = posComp.Position + colComp.ColBoxOffset + colComp.ColBoxSize;

                DrawLine(game, _topLeft, _topRight);
                DrawLine(game, _topLeft, _botLeft);
                DrawLine(game, _topRight, _botRight);
                DrawLine(game, _botLeft, _botRight);
            }

            private static void DrawLine(Game game, Vector2 start, Vector2 end)
            {
                Texture2D t = new Texture2D(game.GraphicsDevice, 1, 1);
                t.SetData<Color>(new Color[] { Color.Red });

                Vector2 edge = end - start;
                float angle = (float)Math.Atan2(edge.Y, edge.X);

                RPGGame.spriteBatch.Draw(t,
                                        new Rectangle(// rectangle defines shape of line and position of start of line
                                            (int)start.X,
                                            (int)start.Y,
                                            (int)edge.Length(), //sb will strech the texture to fill this rectangle
                                            1), //width of line, change this to make thicker line
                                        null,
                                        Color.Red, //colour of line
                                        angle,     //angle of line (calulated above)
                                        new Vector2(0, 0), // point in line about which to rotate
                                        SpriteEffects.None,
                                        0);
            }
        }
            
        /// <summary>
        /// Camera system updates and provides access to <see cref="CurTransformMatrix"/>.
        /// This is passed to Monogame call <see cref="SpriteBatch.Begin())"/>, manipulating game
        /// viewport.
        /// </summary>
        public static class CameraSystem
        {
            // camera follows a entity - set in <see cref="CameraComponent"> constructor
            public static Eid? CurCameraEid { get; set; }

            // Entity with camera component should have position and movement components also
            private static CameraComponent _cameraComponent;

            /// <summary>
            /// Returns transform matrix determined by <see cref="_cameraComponent"/> data. Current
            /// '_cameraComponent' is a component of entity <see cref="CurCameraEid"/>.
            /// </summary>
            public static Matrix CurTransformMatrix
            {
                get
                {
                    if (_cameraComponent != null)
                    {
                        return Matrix.Identity 
                               *
                               Matrix.CreateTranslation(_cameraComponent.Origin.X,
                                                                       _cameraComponent.Origin.Y, 0) 
                               *
                               Matrix.CreateTranslation(-_cameraComponent.Position.X,
                                                                    -_cameraComponent.Position.Y, 0)
                               *
                               Matrix.CreateScale(_cameraComponent.Scale)
                               ;
                    }
                    else
                    {
                        return Matrix.Identity;
                    }
                }
            }

            // for now, camera follows entity with eid <see cref="CurCameraEid"/>
            public static void Update(EntityComponentStorage ecs, GameTime gameTime)
            {
                if (CurCameraEid != null)
                {
                    _cameraComponent = (CameraComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[ecs.ComponentCids[typeof(CameraComponent)]]);
                    PositionComponent posComp = (PositionComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[ecs.ComponentCids[typeof(PositionComponent)]]);
                    MovementComponent moveComp = (MovementComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[ecs.ComponentCids[typeof(MovementComponent)]]);

                    if (_cameraComponent != null && posComp != null)
                    {
                        var moveFactor = moveComp.MoveSpeed *
                                                       (float)gameTime.ElapsedGameTime.TotalSeconds;

                        _cameraComponent.Position +=
                        new Vector3((posComp.Position.X - _cameraComponent.Position.X) *
                                                                                       moveFactor.X,
                                    (posComp.Position.Y - _cameraComponent.Position.Y) *
                                                                                       moveFactor.Y,
                                    0
                                   );
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Sorry Sir! Null component reference " +
                                                           "in camera system.");
                    }
                }
            }
        }
        
        /// <summary>
        /// Position system updates position of all entities with position and movement components.
        /// 
        /// Each frame, positionComponent.Position += delta * movementComponent.Velocity
        /// </summary>
        public static class PositionSystem
        {
            public static void Update(EntityComponentStorage ecs, GameTime gameTime)
            {
                PositionComponent posComp;
                MovementComponent moveComp;

                Vector2 oldPos;
                Vector2 newPos;
                
                // for all entities with a position component
                foreach (Eid posEid in ecs.ComponentEids[ecs.ComponentCids[typeof(PositionComponent)]])
                {
                    // position component of current entity changes according to that entities movement component
                    posComp = 
                        ((PositionComponent)ecs.Entities[posEid].Components[
                                                     ecs.ComponentCids[typeof(PositionComponent)]]);

                    // entity movement component stores velocity
                    moveComp = 
                        ((MovementComponent)ecs.Entities[posEid].Components[
                                                     ecs.ComponentCids[typeof(MovementComponent)]]);

                    if (posComp != null && moveComp != null)
                    {   
                        // new position += delta * speed
                        oldPos = posComp.Position;
                        newPos = oldPos + 
                                    Vector2.Multiply(moveComp.Velocity, 
                                                  (float)gameTime.ElapsedGameTime.TotalMilliseconds
                                                    );
                        posComp.Position = newPos;
                    }
                }
            }
        }
        
        /// <summary>
        /// Render system renders all entities with a render component, at position specified by
        /// position component of respective entities.
        /// </summary>
        public static class RenderSystem
        {
            public static void Draw(EntityComponentStorage ecs)
            { 
                Component renderComponent;
                Component positionComponent;
                Texture2D renderSpriteSheet = null;
                Rectangle renderRect = new Rectangle(0, 0, 0, 0);
                Vector2 renderToPos = new Vector2(0, 0);

                // render all entities with a position component
                foreach (Eid renderEid in ecs.ComponentEids[ecs.ComponentCids[typeof(RenderComponent)]])
                {
                    renderComponent = ecs.Entities[renderEid].Components[
                                                        ecs.ComponentCids[typeof(RenderComponent)]];
                    positionComponent = ecs.Entities[renderEid].Components[
                                                      ecs.ComponentCids[typeof(PositionComponent)]];

                    // need position & render components
                    if (positionComponent != null && renderComponent != null)
                    {
                        renderToPos = ((PositionComponent)positionComponent).Position;

                        renderSpriteSheet = ((RenderComponent)renderComponent).SpriteSheet;
                        renderRect = ((RenderComponent)renderComponent).SpriteLocRect;
                        
                        RPGGame.spriteBatch.Draw(renderSpriteSheet, 
                                                 renderToPos, 
                                                 renderRect, 
                                                 Color.White
                                                );
                    }
                }
            }
        }

        /// <summary>
        /// Movement system stores possible entity movement directions.
        /// </summary>
        public static class MovementSystem
        {
            public struct MoveVectors
            {
                public static Vector2 UP = new Vector2(0, -0.01f);
                public static Vector2 DOWN = new Vector2(0, 0.01f);
                public static Vector2 LEFT = new Vector2(-0.01f, 0);
                public static Vector2 RIGHT = new Vector2(0.01f, 0);
                public static Vector2 UPRIGHT = new Vector2(-0.01f, 0.01f);
                public static Vector2 UPLEFT = new Vector2(-0.01f, -0.01f);
                public static Vector2 DOWNRIGHT = new Vector2(0.01f, 0.01f);
                public static Vector2 DOWNLEFT = new Vector2(-0.01f, -0.01f);
            }
        }

        /// <summary>
        /// Input system acts on first Entity with a <see cref="InputComponent"/>.
        /// 
        /// This Entity has its <see cref="MovementComponent"/> modified appropriately 
        /// according to <paramref name="keyState"/>.
        /// </summary>
        public static class InputSystem
        {
            public static void Update(EntityComponentStorage ecs, KeyboardState keyState)
            {
                // "Keys" is a monogame enum
                Keys[] pressedKeys = keyState.GetPressedKeys();

                // grab first entity with input component.
                // input component contains no data, just indicates which Entity should be acted
                // upon by InputSystem
                Eid inputEid = ecs.ComponentEids[
                                                ecs.ComponentCids[typeof(InputComponent)]][0];

                // this entity must have a movement component
                MovementComponent movementComponent = 
                                    ((MovementComponent)ecs.Entities[inputEid].
                                    Components[ecs.ComponentCids[typeof(MovementComponent)]]);
                if (movementComponent != null)
                {
                    // reset velocity of current entity
                    movementComponent.Velocity = Vector2.Zero;

                    // delegate modifies position component of the entity with input component
                    Action<Vector2> addToInputEntVelocity;
                    addToInputEntVelocity = value => ((MovementComponent)ecs.Entities[inputEid].
                        Components[ecs.ComponentCids[typeof(MovementComponent)]]).Velocity += value;

                    // appropriately modify position component of input entity
                    foreach (Keys pressedKey in pressedKeys)
                    {
                        switch (pressedKey)
                        {
                            case Keys.W:
                                addToInputEntVelocity(MovementSystem.MoveVectors.UP * 
                                                                       movementComponent.MoveSpeed);
                                break;

                            case Keys.A:
                                addToInputEntVelocity(MovementSystem.MoveVectors.LEFT *
                                                                       movementComponent.MoveSpeed);
                                break;

                            case Keys.S:
                                addToInputEntVelocity(MovementSystem.MoveVectors.DOWN *
                                                                       movementComponent.MoveSpeed);
                                break;

                            case Keys.D:
                                addToInputEntVelocity(MovementSystem.MoveVectors.RIGHT *
                                                                       movementComponent.MoveSpeed);
                                break;
                        }
                    }
                }
            }
        }
    }
}

