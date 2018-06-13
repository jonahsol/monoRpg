using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
using System.Collections.ObjectModel;

namespace TestGame
{
    /// <summary> Class <c>TiledMapReader</c> generates game map model from a .tmx file created using the program
    /// 'Tiled'. Makes use of <see href='https://github.com/marshallward/TiledSharp'/>.
    /// </summary>
    
    class TiledMapReader
    {
        /// <summary> Uses TiledSharp API calls to draw the Tiled map specified by param </summary>
        /// <param name="map"> Set of tiled grids, each tile has an id tag, which is a reference to a sprite in <see param="tilesetTxtr"/> </param> 
        /// <param name="tilesetTxtr" games spritesheet
        /// <param name="spriteBatch" monogame object
        public static void DrawMap(TmxMap map, Texture2D tilesetTxtr, SpriteBatch spriteBatch)
        {
        
            int? tilesetNumCols = map.Tilesets[0].Columns;
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;

            for (int iLayer = 0; iLayer < map.Layers.Count; iLayer++)
            {
                Collection<TmxLayerTile> curLayer = map.Layers[iLayer].Tiles;

                for (var curTile = 0; curTile < curLayer.Count; curTile++)
                {
                    // gid identifies tile number eg. gid = 1 is top left tile
                    if (curLayer[curTile].Gid != 0)
                    {
                        // if gid is not one of our tiles, we have a rotated tile
                        if (curLayer[curTile].Gid > map.Tilesets[0].TileCount)
                        {
                            /// TO DO
                        }

                        // calculate rectangle denoting location of sprite in sprite sheet
                        int curTileImgOffset = curLayer[curTile].Gid - 1;
                        int tilesetLocX = (int)(curTileImgOffset % tilesetNumCols);
                        int tilesetLocY = (int)(curTileImgOffset / tilesetNumCols);
                        Rectangle tilesetRect = new Rectangle(tilesetLocX * tileWidth, tilesetLocY * tileHeight, tileWidth, tileHeight);

                        // calculate rectangle to display sprite on screen
                        float x = (curTile % tileWidth) * tileWidth;
                        float y = (float)(curTile / tileHeight) * tileHeight;
                        Rectangle displayRect = new Rectangle((int)x, (int)y, tileWidth, tileHeight);

                        // display
                        spriteBatch.Draw(tilesetTxtr, displayRect, tilesetRect, Color.White);

                    }

                }

            }
        }

    }
}
