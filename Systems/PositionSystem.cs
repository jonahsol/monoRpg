using Microsoft.Xna.Framework;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
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
}
