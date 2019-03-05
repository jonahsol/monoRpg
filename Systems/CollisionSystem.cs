using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
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
                for (int j = 0; j < collisionCandidates.Count; j++)
                {
                    Entity iEntity = ecs.Entities[collisionCandidates[i]];
                    Entity jEntity = ecs.Entities[collisionCandidates[i]];

                    if (EntitiesCollide(ecs, iEntity, jEntity) &&
                        EntityTypeCollide((CollisionComponent)iEntity.Components[typeof(CollisionComponent)],
                                          (CollisionComponent)jEntity.Components[typeof(CollisionComponent)])
                       )
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

                // only check for collision between two entities if their types have a collision 
                // reaction
                if (true)
                {
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
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool EntityTypeCollide(CollisionComponent col1, CollisionComponent col2)
        {
            return true;
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
}
