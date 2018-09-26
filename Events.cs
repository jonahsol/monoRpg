using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpg
{
    namespace EntityComponentSystem
    {
        /// <summary>
        /// Event args. passed to event handler <see cref="CollisionComponent.CollisionHandler">.
        /// </summary>
        public class CollisionEventArgs : EventArgs
        {
            public Entity entity1;
            public Entity entity2;

            public CollisionEventArgs(Entity entity1, Entity entity2)
            {
                this.entity1 = entity1;
                this.entity2 = entity2;
            }
        }
    }
}
