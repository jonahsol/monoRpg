using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Eid = System.UInt16;
using EntityComponentSystem;

namespace Rpg
{
    /// <summary>
    /// Defines a variety of tools useful for development.
    /// </summary>
    public static class DevUtils
    {
        public static void DrawFpsCounter(Vector2 pos, SpriteFont spriteFont, GameTime gameTime)
        {
            if (spriteFont != null)
            {
                float fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                RPGGame.spriteBatch.DrawString(spriteFont, 
                                                String.Format("{0:f1}", fps), pos, Color.Yellow);
            }
        }
            
        /// <summary>
        /// Components collision and position form an entity's collision box. Method draws all
        /// collision boxes.
        /// </summary>
        /// <param name="ecs"></param>
        public static void DrawCollisionBoxes(EntityComponentStorage ecs)
        {
            foreach (Eid eid in ecs.ComponentEids[typeof(CollisionComponent)])
            {
                CollisionComponent colComp = 
                    (CollisionComponent)ecs.Entities[eid].Components[typeof(CollisionComponent)];
                PositionComponent posComp;
               
                if (ecs.Entities[eid].Components.TryGetValue(typeof(PositionComponent), 
                                                                        out Component getPosComp))
                {
                    posComp = (PositionComponent)getPosComp;

                    if (posComp != null && colComp != null)
                    {
                        DrawCollisionBox(posComp, colComp);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the collision hit box formed by an entities collision and position components.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="posComp"></param>
        /// <param name="colComp"></param>
        private static void DrawCollisionBox(PositionComponent posComp,
                                                                    CollisionComponent colComp)
        {
            // grab corners of hit box

            Vector2 _topLeft = posComp.Position + colComp.ColBoxOffset;
            Vector2 _topRight =
                posComp.Position + colComp.ColBoxOffset + new Vector2(colComp.ColBoxSize.X, 0);
            Vector2 _botLeft =
                posComp.Position + colComp.ColBoxOffset + new Vector2(0, colComp.ColBoxSize.Y);
            Vector2 _botRight = posComp.Position + colComp.ColBoxOffset + colComp.ColBoxSize;

            // draw lines b/w corners

            DrawLine(_topLeft, _topRight);
            DrawLine(_topLeft, _botLeft);
            DrawLine(_topRight, _botRight);
            DrawLine(_botLeft, _botRight);
        }

        /// <summary>
        /// Draw line between two vectors.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private static void DrawLine(Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            RPGGame.spriteBatch.Draw(RPGGame.UtilTexture,  // 1x1 blank texture
                                // rectangle defines shape of line and line starting position
                                // sb will stretch the texture to fill this rectangle
                                new Rectangle(
                                    (int)start.X,
                                    (int)start.Y,
                                    (int)edge.Length(), 
                                    1), //width of line, change this to make thicker line
                                null,
                                Color.Red, //colour of line
                                angle,     //angle of line (calulated above)
                                new Vector2(0, 0), // point in line about which to rotate
                                SpriteEffects.None,
                                0);
        }
    }
}
