using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
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
