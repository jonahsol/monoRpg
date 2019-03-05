using Microsoft.Xna.Framework;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
    /// <summary>
    /// Movement system stores possible entity movement directions.
    /// </summary>
    public static class MovementSystem
    {
        public struct MoveVectors
        {
            public static Vector2 UP = new Vector2(0, -0.01f);
            public static Vector2 DOWN = new Vector2(0, 0.01f);
            public static Vector2 LEFT = new Vector2(-0.01f, 0);
            public static Vector2 RIGHT = new Vector2(0.01f, 0);
            public static Vector2 UPRIGHT = new Vector2(-0.01f, 0.01f);
            public static Vector2 UPLEFT = new Vector2(-0.01f, -0.01f);
            public static Vector2 DOWNRIGHT = new Vector2(0.01f, 0.01f);
            public static Vector2 DOWNLEFT = new Vector2(-0.01f, -0.01f);
        }
    }
}
