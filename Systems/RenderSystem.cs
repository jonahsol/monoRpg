using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rpg;

using Eid = System.UInt16;

namespace EntityComponentSystem
{
    /// <summary>
    /// Render system renders all entities with a render component, at position specified by
    /// position component of respective entities.
    /// </summary>
    public static class RenderSystem
    {
        public static void Draw(EntityComponentStorage ecs)
        {
            Component positionComponent;
            Component renderComponent;
            Texture2D renderSpriteSheet = null;
            Rectangle renderRect = new Rectangle(0, 0, 0, 0);
            Vector2 renderToPos = new Vector2(0, 0);
            float renderRot = 0;

            // render all entities with a position component
            foreach (Eid renderEid in ecs.ComponentEids[typeof(RenderComponent)])
            {
                renderComponent = ecs.Entities[renderEid].Components[typeof(RenderComponent)];
                positionComponent = ecs.Entities[renderEid].Components[typeof(PositionComponent)];

                // need position & render components
                if (positionComponent != null && renderComponent != null)
                {
                    renderToPos = ((PositionComponent)positionComponent).Position;

                    renderSpriteSheet = ((RenderComponent)renderComponent).SpriteSheet;
                    renderRect = ((RenderComponent)renderComponent).SpriteLocRect;
                    renderRot = ((RenderComponent)renderComponent).SpriteRotation;
                    //                spriteBatch.Draw(tilesetTxtr, mapTileLoc, tilesetRect, Color.White, 
                    //                                            rotation, origin, 1.0f, SpriteEffects.None, 0f);
                    //RPGGame.spriteBatch.Draw(renderSpriteSheet, 
                    //                            renderToPos, 
                    //                            renderRect, 
                    //                            Color.White
                    //                        );
                    RPGGame.spriteBatch.Draw(renderSpriteSheet, (renderToPos + (RPGGame.SpriteDimensions / 2)), renderRect,
                        Color.White, renderRot, RPGGame.SpriteOriginOfRotation, 1.0f, SpriteEffects.None, 0f);//
                }
            }
        }
    }
}
