using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        /// Create entities from map editor tiles and objects.
        /// </summary>
        /// <param name="ecs"></param>
        /// <param name="map"></param>
        /// <param name="tilesetTxtr"></param>
        public static void AddTiledMapEntities(EntityComponentStorage ecs, TmxMap map, Texture2D tilesetTxtr)
        {
            // sprite sheet specs
            int? tilesetNumCols = map.Tilesets[0].Columns;
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;

            // declare objects for render components
            Vector2 mapLoc = new Vector2();
            // tiles rotate around their centre
            Vector2 origin = new Vector2(tileWidth / 2, tileWidth / 2);
            float rotation = 0f;

            // add tmx objects as entities with position and render components, as well as 
            // other components determined by the object type
            for (int iObjectGroup = 0; iObjectGroup < map.ObjectGroups.Count; iObjectGroup++)
            {
                for (int iObject = 0; iObject < map.ObjectGroups[iObjectGroup].Objects.Count; iObject++)
                {
                    TmxObject obj = map.ObjectGroups[iObjectGroup].Objects[iObject];

                    // it appeared that: obj.Tile.X == obj.Tile.Y != obj.Y; causing entities to be
                    // rendered in the wrong position. 'createComponentsFromTile' modified to 
                    // optionally accept position arguments
                    (RenderComponent renderComp, PositionComponent posComp) =
                        createComponentsFromTile(obj.Tile, (int)obj.X, (int)obj.Y);


                    if (obj.Type == "Collision")
                    {
                        Entity newEnt = ecs.AddEntity(renderComp, posComp,
                                new CollisionComponent(ecs, Vector2.Zero, new Vector2(tileWidth, tileHeight)));
                    }
                    else
                    {
                        Entity newEnt = ecs.AddEntity(renderComp, posComp);
                    }
                }
            }

            // add tmx tiles (just sprite with location) as entities with position and render 
            // components
            for (int iLayer = 0; iLayer < map.Layers.Count; iLayer++)
            {
                Collection<TmxLayerTile> curLayer = map.Layers[iLayer].Tiles;

                for (var curTile = 0; curTile < curLayer.Count; curTile++)
                {
                    // gid identifies sprite location in sheet eg. gid = 1 is top left tile of sprite sheet
                    if (curLayer[curTile].Gid != 0)
                    {
                        (RenderComponent renderComp, PositionComponent posComp) = 
                            createComponentsFromTile(curLayer[curTile], null, null);

                        if (renderComp != null && posComp != null)
                        {
                            ecs.AddEntity(renderComp, posComp);
                        }
                        
                    }
                }
            }

            // Create render and position components for a given tile. 
            //
            // If arguments passed to x & y are non-null, then they will be used for the position, 
            // component. Otherwise parameter position from parameter 'tile' will be used.
            // x & y arguments should be in pixels.
            (RenderComponent, PositionComponent) createComponentsFromTile(TmxLayerTile tile, 
                int? x, int? y)
            {
                // gid identifies sprite location in sheet eg. gid = 1 is top left tile of sprite sheet
                if (tile.Gid != 0)
                {
                    if (tile.DiagonalFlip)
                    {
                        rotation = (float)(-90 * (Math.PI / 180));
                    }
                    else
                    {
                        rotation = 0;
                    }

                    if (tile.HorizontalFlip)
                    {
                        rotation += (float)(180 * (Math.PI / 180));
                    }

                    if (tile.VerticalFlip)
                    {
                        //rotation -= (float)(180 * (Math.PI / 180));
                    }

                    // calculate location of sprite in sprite sheet
                    int curTileImgOffset = tile.Gid - 1;
                    int tilesetLocX = (int)(curTileImgOffset % tilesetNumCols);
                    int tilesetLocY = (int)(curTileImgOffset / tilesetNumCols);
                    Rectangle tilesetRect = new Rectangle(tilesetLocX * tileWidth,
                                                          tilesetLocY * tileHeight,
                                                          tileWidth,
                                                          tileHeight
                                                         );

                    // optionally accept x & y args.
                    if (x != null && y != null)
                    {
                        mapLoc.X = (float)x;
                        mapLoc.Y = (float)y;
                    }
                    else
                    {
                        // multiply since 'TmxLayerTile' x & y properties are tile positions, 
                        // not pixels
                        mapLoc.X = tile.X * tileWidth;
                        mapLoc.Y = tile.Y * tileHeight;
                    }
                 

                    return (new RenderComponent(ecs, tilesetTxtr, tilesetRect, rotation),
                            new PositionComponent(ecs, mapLoc)
                           );
                }
                else
                {
                    return (null, null);
                }
            }
        }
    }
}
