using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Eid = System.UInt16;
using Cid = System.UInt16;

namespace Rpg
{

    namespace EntityComponentSystem
    {
        /// <summary>
        /// Base class for all components, <see cref="EntityComponentStorage"/> for more info.
        /// </summary>
        public abstract class Component
        {
        }

        /// <summary>
        /// Camera component stores a number of manipulations that may affect game viewport - camera
        /// system generates a transform matrix from CameraComponent data, which is passed 
        /// to Monogame call <see cref="SpriteBatch.Begin())"/>.
        /// </summary>
        public class CameraComponent : Component
        {
            public float Scale { get; set; }
            // position of camera is relative to origin 
            public Vector3 Origin { get; set; }
            public Vector3 Position { get; set; }
            
            public CameraComponent(EntityComponentStorage ecs, Eid curCameraEid,
                                   Vector3 origin, Vector3 position, float scale)
            {
                this.Origin = origin / scale;  // since transform matrix multiplied by scale
                this.Position = position;
                this.Scale = scale;

                CameraSystem.CurCameraEid = curCameraEid;
            }
        }

        /// <summary>
        /// Does not store data - indicates which entity or entities should be operated upon by
        /// <see cref="InputSystem"/>.
        /// </summary>
        public class InputComponent : Component
        {
            public InputComponent(EntityComponentStorage ecs) {}
        }
        
        /// <summary>
        /// Stores position of an entity.
        /// </summary>
        public class PositionComponent : Component
        {
            public Vector2 Position { get; set; }
            // position of entity during previous frame
            public Vector2 PrevPosition { get; set; }  

            public PositionComponent(EntityComponentStorage ecs, Vector2 position)
            {
                this.Position = position;
                this.PrevPosition = position;
            }
        }

        /// <summary>
        /// For a given entity, RenderComponent stores sprite sheet image and location of entity's 
        /// sprite in the sprite sheet.
        /// </summary>
        public class RenderComponent : Component
        {
            public Texture2D SpriteSheet { get; private set; }
            public Rectangle SpriteLocRect { get; private set; }

            public RenderComponent(EntityComponentStorage ecs, Texture2D spriteSheet,  
                                                                        Rectangle spriteLocRect)
            {
                this.SpriteSheet = spriteSheet;
                this.SpriteLocRect = spriteLocRect;
            }
        }

        /// <summary>
        /// Stores location of an entity's collision bounding box, as well as the event handler
        /// for when that entity is involved in a collision.
        /// </summary>
        public class CollisionComponent : Component
        {
            // top left corner of collision bounding box is sum of entity's position & ColBoxOffset
            public Vector2 ColBoxOffset { get; set; }
            // size of collision bounding box
            public Vector2 ColBoxSize { get; set; }

            // invoked when entity involved in a collision
            public event EventHandler<CollisionEventArgs> CollisionHandler;

            public CollisionComponent(EntityComponentStorage ecs, 
                                      Vector2 colBoxOffset, 
                                      Vector2 colBoxSize, 
                                      params Action<object, CollisionEventArgs>[] collisionReactions
                                     )
            {
                this.ColBoxOffset = colBoxOffset;
                this.ColBoxSize = colBoxSize;

                AddCollisionReactions(collisionReactions);
            }

            /// <summary>
            /// Invokes instance event handler <see cref="CollisionHandler">.
            /// </summary>
            /// <param name="e"></param>
            public virtual void OnCollision(CollisionEventArgs e)
            {

                // q mark short circuits Invoke call when 'CollisionHandler' null
                this.CollisionHandler?.Invoke(this, e);
            }

            /// <summary>
            /// Add methods to instance event handler <see cref="CollisionHandler">.
            /// </summary>
            /// <param name="collisionReactions"></param>
            public void AddCollisionReactions(Action<object, CollisionEventArgs>[] 
                                                                                collisionReactions)
            {
                foreach (Action<object, CollisionEventArgs> collisionReaction in collisionReactions)
                {
                    // invoke w/o args creates partially applied function from collisionReaction
                    this.CollisionHandler += collisionReaction.Invoke;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HealthComponent : Component
        {
            int CurHealth { get; set; }
            int FullHealth { get; set; }

            public HealthComponent(EntityComponentStorage ecs)
            {
            
            }
        }

        /// <summary>
        /// Stores an entity's velocity and movespeed.
        /// </summary>
        public class MovementComponent : Component
        {
            public Vector2 Velocity { get; set; }
            public Vector2 MoveSpeed { get; set; }
            
            public MovementComponent(EntityComponentStorage ecs, Vector2 velocity, 
                                                                                Vector2 moveSpeed)
            {
                this.Velocity = velocity;
                this.MoveSpeed = moveSpeed;
            }

            public MovementComponent(EntityComponentStorage ecs) 
                   : this(ecs, new Vector2(0, 0), new Vector2(1, 1))
            {}
        }
    }
}


