using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame
{
    public abstract class Entity
    {

        protected Vector2 _position;

        public virtual void Initialise()
        {
        }

        public virtual void Update(GameTime gameTime, KeyboardState keyState)
        {
        }

        public virtual void Draw(Texture2D entityText, SpriteBatch spriteBatch)
        {
        }

    }
}
