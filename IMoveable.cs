using Microsoft.Xna.Framework;

namespace TestGame
{
    public interface IMoveable
    {
        void MoveUp(GameTime gametime);
        void MoveDown(GameTime gametime);
        void MoveLeft(GameTime gametime);
        void MoveRight(GameTime gametime);
    }
}
