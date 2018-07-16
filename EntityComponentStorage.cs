using System.Collections.Generic;
using System;
using Eid = System.UInt16;
using Cid = System.UInt16;

namespace Rpg
{
    namespace EntityComponentSystem
    {
        /// <summary>
        /// EntityComponentStorage stores a number of <see cref="Entity"/>. Each entity is assigned
        /// a unique id <see cref="Eid"/>, and has a list of <see cref="Component"/>. 
        /// 
        /// Each component type has a unique id <see cref="Cid"/>. A component represents an entity 
        /// possessing a particular aspect (ie. behaviour/functionality). Component instances also 
        /// store data pertaining to that particular aspect.
        /// 
        /// A system provides the logical implementation for a given aspect. Usually, a given system
        /// will act upon all entities that have a corresponding component. 
        /// eg. <see cref="PositionSystem"/> acts upon all entities with a 
        /// <see cref="PositionComponent"/>
        /// Note that this is not necessarily the case - systems and 
        /// components need not have a one to one interaction relationship.
        public class EntityComponentStorage
        {
            // a number of in game entities
            public List<Entity> Entities { get; set; }
            // counter for adding entities
            private int _curNumEnt;

            // array of lists with each array index corresponding to a components Cid, and list at 
            // that element containing all Eid possessing an instance of that component
            public List<Eid>[] componentEids;
            public static int NumComponents { get; set; } = 7;                                       /// TODO: some fancy reflection to automate this
            public int NumComponentsAdded { get; set; } = 0;

            public Dictionary<Type, Cid> ComponentCids;

            public EntityComponentStorage()
            {
                Entities = new List<Entity>();
                _curNumEnt = 0;

                componentEids = new List<Eid>[EntityComponentStorage.NumComponents];
                for (int i = 0; i < EntityComponentStorage.NumComponents; i++)
                {
                    componentEids[i] = new List<Eid>();
                }

                ComponentCids = new Dictionary<Type, Eid>(); // possibly implement a weak table instead? possibly have this in EntCompStor instead? maybe more elegant to have simple static vars?
            }

            /// <summary>
            /// Create a new <see cref="Entity"/>, and insert it into <see cref="this.Entities">, at 
            /// index specified by the Eid of the inserted entity. Unique Eid assigned to new entity 
            /// using counter <see cref="this._curNumEnt">.
            /// </summary>
            public Entity AddEntity(params Component[] components)
            {
                // create new Entity and add it to list of Entities
                Entity newEntity = new Entity((Eid)_curNumEnt, null, components);
                Entities.Insert(newEntity.Eid, newEntity);
                _curNumEnt++;

                AddComponentsToEntity(newEntity.Eid, components);

                return newEntity;
            }

            /// <summary>
            /// Add components specified by <paramref name="components"/> to entity with 
            /// Eid <paramref name="entityEid"/>.
            /// </summary>
            /// <param name="entityEid"> Eid of entity to add components to. </param>
            /// <param name="components"> Components to add to entityEid. </param>
            public void AddComponentsToEntity(Eid entityEid, params Component[] components)
            {
                foreach (Component component in components)
                {
                    Entities[entityEid].
                            Components[this.ComponentCids[component.GetType()]] = component;
                    this.componentEids[this.ComponentCids[component.GetType()]].Add(entityEid);
                }
            }
        }

        /// <summary>
        /// Entity is a number "Eid", with a string "name", and a list which may be populated with 
        /// components. Constructor fills this list with null elements.
        /// 
        /// All operations relating to an Entity should be performed by an EntityComponentStorage.
        /// </summary>
        public struct Entity
        {
            public Eid Eid { get; set; }
            public List<Component> Components { get; private set; }
            public string name;

            public Entity(Eid Eid, string name, params Component[] components) : this()
            {
                this.Eid = Eid;
                this.name = name;

                // initialise components list to contain all null elements
                Components = new List<Component>();
                for (int i = 0; i < EntityComponentStorage.NumComponents; i++)
                {
                    this.Components.Add(null);
                }
            }

        }
    }

    

}
		