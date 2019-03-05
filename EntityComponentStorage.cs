using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

using Eid = System.UInt16;

namespace EntityComponentSystem
{ 
    /// <summary>
    /// EntityComponentStorage stores a number of <see cref="Entity"/>. Each entity is assigned
    /// a unique id <see cref="Eid"/>, and has a list of <see cref="Component"/>. 
    /// 
    /// A component represents an entity possessing a particular aspect 
    /// (ie. behaviour/functionality). Component instances also store data pertaining to that 
    /// particular aspect.
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
        // Eid of a new entity assigned to current num. entities
        private int _curNumEnt;
            
        // dict creates reference from component type to a corresponding list, which stores
        // all Eid possessing an instance of that component type
        public Dictionary<Type, List<Eid>> ComponentEids { get; set; }

        public EntityComponentStorage()
        {

            Entities = new List<Entity>();

            _curNumEnt = 0;
            ComponentEids = new Dictionary<Type, List<Eid>>();
            // for all component types, add a mapping to list of entitities possessing component
            foreach (Type type in Assembly.GetAssembly(typeof(Component)).
                                    GetTypes().Where(t => t.IsClass && 
                                                    !t.IsAbstract && 
                                                    t.IsSubclassOf(typeof(Component))
                                                    )
                    )
            {
                ComponentEids.Add(type, new List<Eid>());
            }
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
            this.Entities.Insert(newEntity.Eid, newEntity);
            _curNumEnt++;  // increment num ents., this will be the Eid of next entity

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
                this.Entities[entityEid].Components[component.GetType()] = component;
                this.ComponentEids[component.GetType()].Add(entityEid);
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
        public Dictionary<Type, Component> Components { get; private set; }
        public string name;

        public Entity(Eid Eid, string name, params Component[] components) : this()
        {
            this.Eid = Eid;
            this.name = name;
                
            Components = new Dictionary<Type, Component>();
        }

    }
}

    


		