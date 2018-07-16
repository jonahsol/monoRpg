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
        /// 
        /// Each component class is assigned a unique Cid, which is
        /// stored in an EntityComponentStorage property, <see cref="ComponentCids"/>.
        /// This assignment is performed in 'Component' constructor - all inheriting classes 
        /// must call 'base()'.
        /// </summary>
        public abstract class Component
        {
            protected Component(EntityComponentStorage ecs)
            {
                // assign a component to a unique Cid, if this has not already been done
                if (!ecs.ComponentCids.ContainsKey(this.GetType()))
                {
                    ecs.ComponentCids.Add(this.GetType(), 
                                          (Cid)(EntityComponentStorage.NumComponents - 
                                          ecs.NumComponentsAdded - 1)
                                         );
                    ecs.NumComponentsAdded++;
                }
            }
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
                                   Vector3 origin, Vector3 position, float scale) : base(ecs)
            {
                this.Origin = origin / scale;  // since transform matrix multiplied by scale
                this.Position = position;
                this.Scale = scale;

                CameraSystem.CurCameraEid = curCameraEid;
            }
        }

        /// <summary>
        /// Does not store data - indicates which entity or entities should be operated upon by
        /// <see cref="InputSystem"/>
        /// </summary>
        public class InputComponent : Component
        {
            public InputComponent(EntityComponentStorage ecs) : base(ecs) {}
        }
        

        /// <summary>
        /// Stores position of an entity.
        /// </summary>
        public class PositionComponent : Component
        {
            public Vector2 Position { get; set; }

            public PositionComponent(EntityComponentStorage ecs, Vector2 position) : base(ecs)
            {
                this.Position = position;
            }
        }

        /// <summary>
        /// For a given entity, RenderComponent stores sprite sheet and location of entity's sprite
        /// in the sprite sheet.
        /// </summary>
        public class RenderComponent : Component
        {
            public Texture2D SpriteSheet { get; private set; }
            public Rectangle Rect { get; private set; }

            public RenderComponent(EntityComponentStorage ecs, 
                                   Texture2D spriteSheet, 
                                   Rectangle rect
                                   ) : base(ecs)
            {
                this.SpriteSheet = spriteSheet;
                this.Rect = rect;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CollisionComponent : Component
        {
            public CollisionComponent(EntityComponentStorage ecs) : base(ecs)
            {
            
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HealthComponent : Component
        {
            int CurHealth { get; set; }
            int FullHealth { get; set; }

            public HealthComponent(EntityComponentStorage ecs) : base(ecs)
            {
            
            }
        }

        /// <summary>
        /// Stores an entities velocity and movespeed.
        /// </summary>
        public class MovementComponent : Component
        {
            public Vector2 Velocity { get; set; }
            public Vector2 MoveSpeed { get; set; }
            
            public MovementComponent(
                   EntityComponentStorage ecs, Vector2 velocity, Vector2 moveSpeed) :
                   base(ecs)
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


