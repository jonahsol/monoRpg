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
        /// Collision system handles all in game collisions. A collision invokes an entities
        /// <see cref="CollisionComponent.CollisionHandler">.
        /// </summary>
        public static class CollisionSystem
        {              
            public static void Update(EntityComponentStorage ecs, GameTime gameTime)
            {
                List<Eid> collisionCandidates = ecs.ComponentEids[typeof(CollisionComponent)];

                for (int i = 0; i < collisionCandidates.Count; i++)
                {
                    for (int j = i + 1; j < collisionCandidates.Count; j++)
                    {
                        if (EntitiesCollide(ecs, ecs.Entities[collisionCandidates[i]],
                                                       ecs.Entities[collisionCandidates[j]]))
                        {
                            ResolveCollision(ecs, ecs.Entities[collisionCandidates[i]],
                                                  ecs.Entities[collisionCandidates[j]]);
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
            private static bool EntitiesCollide(EntityComponentStorage ecs, Entity entity1, Entity entity2)
            {

                CollisionComponent col1;
                CollisionComponent col2;

                PositionComponent pos1;
                PositionComponent pos2;

                // both entities must have collision and position components - check for and assign

                bool hasColComps =
                   entity1.Components.TryGetValue(typeof(CollisionComponent), out Component getCol1)
                   & 
                   entity2.Components.TryGetValue(typeof(CollisionComponent), out Component getCol2);

                bool hasPosComps =
                   entity1.Components.TryGetValue(typeof(PositionComponent), out Component getPos1)
                   &
                   entity2.Components.TryGetValue(typeof(PositionComponent), out Component getPos2);
                
                if (hasPosComps && hasColComps)
                {
                    col1 = (CollisionComponent)getCol1;
                    col2 = (CollisionComponent)getCol2;

                    pos1 = (PositionComponent)getPos1;
                    pos2 = (PositionComponent)getPos2;

                    // need position and collision components
                    if (col1 != null && pos1 != null && col2 != null && pos2 != null)
                    {
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
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Invokes <see cref="CollisionComponent.CollisionHandler"> of entities involved 
            /// in collision.
            /// </summary>
            /// <param name="ecs"></param>
            /// <param name="entity1"></param>
            /// <param name="entity2"></param>
            public static void ResolveCollision(EntityComponentStorage ecs,
                                                                    Entity entity1, Entity entity2)
            {
                CollisionComponent collisionComponent1;
                CollisionComponent collisionComponent2;

                // create lazy <see cref="CollisionEventArgs">, to be passed to collision
                Lazy<CollisionEventArgs> e = new Lazy<CollisionEventArgs>(
                                        () => { return new CollisionEventArgs(entity1, entity2); });

                // attempt to invoke "OnCollision" of entity1
                if (entity1.Components.TryGetValue(typeof(CollisionComponent),
                                                                        out Component getColComp1))
                {
                    collisionComponent1 = (CollisionComponent)getColComp1;

                    if (collisionComponent1 != null)
                    {
                        collisionComponent1.OnCollision(e.Value);
                    }
                }

                // attempt to invoke "OnCollision" of entity2
                if (entity2.Components.TryGetValue(typeof(CollisionComponent),
                                                                        out Component getColComp2))
                {
                    collisionComponent2 = (CollisionComponent)getColComp2;

                    if (collisionComponent2 != null)
                    {
                        collisionComponent2.OnCollision(e.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Contains definitions for a variety of collision reactions methods & their delegates.
        /// 
        /// A collision reaction must have params - (object sender, CollisionEventArgs e), in order
        /// for it to be added to a collision components event handler,
        /// <see cref="CollisionComponent.CollisionHandler">.
        /// </summary>
        public static class CollisionReactionSystem
        {
            // assign delegates to collision reaction methods

            public static Action<object, CollisionEventArgs> ReturnsPositions =
                                                                            ReturnToPrevPositions;

            // define collision reaction methods

            /// <summary>
            /// Return both entities involved in collision to their previous position.
            /// </summary>
            /// <param name="sender"> Object raising CollisionEventHandler. </param>
            /// <param name="e"> 
            /// Entities involved in collision. <see cref="CollisionEventArgs">
            /// </param>
            public static void ReturnToPrevPositions(object sender, CollisionEventArgs e)
            {
                PositionComponent posComp1;
                PositionComponent posComp2;
                
                // return entity 1 to prev position
                if (e.entity1.Components.TryGetValue(typeof(PositionComponent), 
                                                                        out Component getPosComp1))
                {
                    posComp1 = (PositionComponent)getPosComp1;
                    posComp1.Position = posComp1.PrevPosition;
                }

                // return entity 2 to prev position
                if (e.entity2.Components.TryGetValue(typeof(PositionComponent), 
                                                                        out Component getPosComp2))
                {
                    posComp2 = (PositionComponent)getPosComp2;
                    posComp2.Position = posComp2.PrevPosition;
                }
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
                                          Components[typeof(CameraComponent)]);
                    PositionComponent posComp = (PositionComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[typeof(PositionComponent)]);
                    MovementComponent moveComp = (MovementComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[typeof(MovementComponent)]);

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
                PositionComponent posComp = null;
                MovementComponent moveComp = null;  // entity movement component stores velocity

                Vector2 oldPos;
                Vector2 newPos;

                // for all entities with a position component
                foreach (Eid posEid in ecs.ComponentEids[typeof(PositionComponent)])
                {
                    // for a given entity 'e', 'e's movement component changes according to its 
                    // position component
                    posComp =
                        ((PositionComponent)ecs.Entities[posEid].
                                                            Components[typeof(PositionComponent)]);

                    if (ecs.Entities[posEid].Components.TryGetValue(typeof(MovementComponent), 
                                                                        out Component getMoveComp))
                    {
                        // entity with position component also has a movement component
                        moveComp = (MovementComponent)getMoveComp;

                        if (posComp != null && moveComp != null)
                        {
                            // new position += delta * speed
                            
                            oldPos = posComp.Position;
                            newPos = oldPos +
                                        Vector2.Multiply(moveComp.Velocity,
                                                      (float)gameTime.ElapsedGameTime.TotalMilliseconds
                                                        );

                            // assign previous and current position to position component
                            posComp.PrevPosition = oldPos;
                            posComp.Position = newPos;
                        }
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
                Component positionComponent;
                Component renderComponent;
                Texture2D renderSpriteSheet = null;
                Rectangle renderRect = new Rectangle(0, 0, 0, 0);
                Vector2 renderToPos = new Vector2(0, 0);

                // render all entities with a position component
                foreach (Eid renderEid in ecs.ComponentEids[typeof(RenderComponent)])
                {
                    renderComponent = ecs.Entities[renderEid].Components[typeof(RenderComponent)];
                    positionComponent = ecs.Entities[renderEid].Components[typeof(PositionComponent)];

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
                Eid inputEid = ecs.ComponentEids[typeof(InputComponent)][0];

                // this entity must have a movement component
                MovementComponent movementComponent = 
                                    ((MovementComponent)ecs.Entities[inputEid].
                                    Components[typeof(MovementComponent)]);
                if (movementComponent != null)
                {
                    // reset velocity of current entity
                    movementComponent.Velocity = Vector2.Zero;

                    // delegate modifies position component of the entity with input component
                    Action<Vector2> addToInputEntVelocity;
                    addToInputEntVelocity = value => ((MovementComponent)ecs.Entities[inputEid].
                        Components[typeof(MovementComponent)]).Velocity += value;

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

