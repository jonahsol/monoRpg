using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TiledSharp;


using EntityComponentSystem;

namespace Rpg
{
    /// <summary> Class <c>TiledMapReader</c> generates game map model from a .tmx file created 
    /// using the program 'Tiled'. Uses <see href='https://github.com/marshallward/TiledSharp'/>.
    /// </summary>
    class TiledMapReader
    {

        /// <summary>
        /// Add tiles param 'map' (read from a .tmx) to param 'ecs', where each tile is an Entity 
        /// with relevant components.
        /// </summary>
        /// <param name="entCollection"></param>
        /// <param name="map"></param>
        /// <param name="tilesetTxtr"></param>
        public static void AddTiledMapEntities(EntityComponentStorage ecs, TmxMap map, Texture2D tilesetTxtr)
        {
            var entCollection = new List<Entity>();

            int? tilesetNumCols = map.Tilesets[0].Columns;
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;

            Vector2 mapTileLoc = new Vector2();
            // tiles rotate around their centre
            Vector2 origin = new Vector2(tileWidth / 2, tileWidth / 2);                                                        ///////////// can remove this
            float rotation = 0f;

            for (int iLayer = 0; iLayer < map.Layers.Count; iLayer++)
            {
                Collection<TmxLayerTile> curLayer = map.Layers[iLayer].Tiles;

                for (var curTile = 0; curTile < curLayer.Count; curTile++)
                {
                    // gid identifies tile number eg. gid = 1 is top left tile of sprite sheet
                    if (curLayer[curTile].Gid != 0)
                    {
                        if (curLayer[curTile].DiagonalFlip)
                        {
                            rotation = (float)(-90 * (Math.PI / 180));
                        }
                        else
                        {
                            rotation = 0;
                        }

                        if (curLayer[curTile].HorizontalFlip)
                        {
                            rotation += (float)(180 * (Math.PI / 180));
                        }

                        if (curLayer[curTile].VerticalFlip)
                        {
                            //rotation -= (float)(180 * (Math.PI / 180));
                        }

                        // calculate location of sprite in sprite sheet
                        int curTileImgOffset = curLayer[curTile].Gid - 1;
                        int tilesetLocX = (int)(curTileImgOffset % tilesetNumCols);
                        int tilesetLocY = (int)(curTileImgOffset / tilesetNumCols);
                        Rectangle tilesetRect = new Rectangle(tilesetLocX * tileWidth,
                                                              tilesetLocY * tileHeight,
                                                              tileWidth,
                                                              tileHeight
                                                             );

                        // calculate in game position (first tile has position 0,0)
                        float x = (curTile % map.Width) * tileWidth;
                        float y = (float)(curTile / map.Height) * tileHeight;
                        mapTileLoc.X = x;
                        mapTileLoc.Y = y;

                        ecs.AddEntity(new RenderComponent(ecs, tilesetTxtr, tilesetRect, rotation),
                                      new PositionComponent(ecs, mapTileLoc),
                                      new CollisionComponent(ecs, Vector2.Zero, new Vector2(tileWidth, tileHeight)));

                        // render current tile to screen
                        //spriteBatch.Draw(tilesetTxtr, mapTileLoc, tilesetRect, Color.White,
                        //                            rotation, origin, 1.0f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        /// <summary> 
        /// Uses TiledSharp api to draw Tiled map specified by <paramref name="map"/>.
        /// </summary>
        /// <param name="map"> 
        /// Set of tiled grids, each tile has an id tag, which is a reference to 
        /// a sprite in <see paramref name="tilesetTxtr"/>
        /// </param>
        /// <param name="tilesetTxtr"> games spritesheet </param> 
        /// <param name="spriteBatch"> monogame api for rendering to screen</param>
        public static void DrawMap(TmxMap map, Texture2D tilesetTxtr, SpriteBatch spriteBatch)
        {
            int? tilesetNumCols = map.Tilesets[0].Columns;
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;

            Vector2 mapTileLoc = new Vector2();
            // tiles rotate around their centre
            Vector2 origin = new Vector2(tileWidth / 2, tileWidth / 2);
            float rotation = 0f;

            for (int iObjGroup = 0; iObjGroup < map.ObjectGroups.Count; iObjGroup++)
            {
                Collection<TmxObject> curObjGroup = map.ObjectGroups[iObjGroup].Objects;

                for (var curObj = 0; curObj < curObjGroup.Count; curObj++)
                {

                }
              

            }

            for (int iLayer = 0; iLayer < map.Layers.Count; iLayer++)
            {
                Collection<TmxLayerTile> curLayer = map.Layers[iLayer].Tiles;

                for (var curTile = 0; curTile < curLayer.Count; curTile++)
                {
                    // gid identifies tile number eg. gid = 1 is top left tile of sprite sheet
                    if (curLayer[curTile].Gid != 0)
                    {
                        if (curLayer[curTile].DiagonalFlip)
                        {
                            rotation = (float)(-90 * (Math.PI / 180));
                        }
                        else
                        {
                            rotation = 0;
                        }

                        if (curLayer[curTile].HorizontalFlip)
                        {
                            rotation += (float)(180 * (Math.PI / 180));
                        }

                        if (curLayer[curTile].VerticalFlip)
                        {
                            //rotation -= (float)(180 * (Math.PI / 180));
                        }

                        // calculate location of sprite in sprite sheet
                        int curTileImgOffset = curLayer[curTile].Gid - 1;
                        int tilesetLocX = (int)(curTileImgOffset % tilesetNumCols);
                        int tilesetLocY = (int)(curTileImgOffset / tilesetNumCols);
                        Rectangle tilesetRect = new Rectangle(tilesetLocX * tileWidth,
                                                              tilesetLocY * tileHeight,
                                                              tileWidth,
                                                              tileHeight
                                                             );

                        // calculate rectangle to display sprite on screen
                        float x = (curTile % map.Width) * tileWidth;
                        float y = (float)(curTile / map.Height) * tileHeight;
                        mapTileLoc.X = x;
                        mapTileLoc.Y = y;

                        // render current tile to screen
                        spriteBatch.Draw(tilesetTxtr, mapTileLoc, tilesetRect, Color.White,
                                                    rotation, origin, 1.0f, SpriteEffects.None, 0f);

                    }

                }

            }
        }

    }
}
