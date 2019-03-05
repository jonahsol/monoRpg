using System;
using System.Collections.Generic;
using System.Linq;
using Eid = System.UInt16;

namespace EntityComponentSystem
{
    /// <summary>
    /// Contains definitions for a variety of collision reactions methods & their delegates.
    /// 
    /// A collision reaction must have params - (object sender, CollisionEventArgs e), in order
    /// for it to be added to a collision components event handler,
    /// <see cref="CollisionComponent.CollisionHandler">.
    /// </summary>
    public static class CollisionReactionSystem
    {
        // assignment of delegates to collision reactions

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
}
