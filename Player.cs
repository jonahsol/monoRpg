using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestGame
{
    class Player : Entity, IMoveable
    {
        Vector2 _movementSpeed;

        public Player(float x, float y)
        {
            _position = new Vector2(x, y);
            _movementSpeed = new Vector2(0.1f, 0.1f);
        }

        public override void Update(GameTime gametime, KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.W))
            {
                MoveUp(gametime);
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                MoveDown(gametime);
            }
            else if (keyState.IsKeyDown(Keys.A))
            {
                MoveLeft(gametime);
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                MoveRight(gametime);
            }
        }

        public override void Draw(Texture2D entityText, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(entityText, _position, new Rectangle(4 * 16, 8 * 16, 16, 16), Color.White);
        }

        public void MoveUp(GameTime gametime)
        {
            _position.Y -= (float)gametime.ElapsedGameTime.TotalMilliseconds * _movementSpeed.Y;
        }

        public void MoveDown(GameTime gametime)
        {
            _position.Y += (float)gametime.ElapsedGameTime.TotalMilliseconds * _movementSpeed.Y;
        }

        public void MoveLeft(GameTime gametime)
        {
            _position.X -= (float)gametime.ElapsedGameTime.TotalMilliseconds * _movementSpeed.X;
        }

        public void MoveRight(GameTime gametime)
        {
            _position.X += (float)gametime.ElapsedGameTime.TotalMilliseconds * _movementSpeed.X;
        }
    }
}
