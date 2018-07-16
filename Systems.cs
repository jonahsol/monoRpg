using System.Collections.Generic;
using System;
using Eid = System.UInt16;
using Cid = System.UInt16;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Rpg
{
    namespace EntityComponentSystem
    {
        /// Systems summary - dependencies.



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
            private static PositionComponent _positionComponent;
            private static MovementComponent _movementComponent;

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
                    _positionComponent = (PositionComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[ecs.ComponentCids[typeof(PositionComponent)]]);
                    _movementComponent = (MovementComponent)(ecs.Entities[(int)CurCameraEid].
                                          Components[ecs.ComponentCids[typeof(MovementComponent)]]);

                    if ((_cameraComponent != null) && (_positionComponent != null))
                    {
                        var moveFactor = _movementComponent.MoveSpeed *
                                                       (float)gameTime.ElapsedGameTime.TotalSeconds;

                        _cameraComponent.Position +=
                        new Vector3((_positionComponent.Position.X - _cameraComponent.Position.X) *
                                                                                       moveFactor.X,
                                    (_positionComponent.Position.Y - _cameraComponent.Position.Y) *
                                                                                       moveFactor.Y,
                                    0
                                   );
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Sorry Sir! Null component reference");
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
                Vector2 oldPos;
                Vector2 newPos;
                
                // for all entities with a position component
                foreach (Eid posEid in ecs.componentEids[ecs.ComponentCids[typeof(PositionComponent)]])
                {
                    // position component of current entity changes according to that entities movement component
                    PositionComponent posComp = 
                        ((PositionComponent)ecs.Entities[posEid].Components[
                                                     ecs.ComponentCids[typeof(PositionComponent)]]);

                    // entity movement component stores velocity
                    MovementComponent moveComp = 
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
                foreach (Eid renderEid in ecs.componentEids[ecs.ComponentCids[typeof(RenderComponent)]])
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
                        renderRect = ((RenderComponent)renderComponent).Rect;
                        
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
                Eid inputEid = ecs.componentEids[
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

