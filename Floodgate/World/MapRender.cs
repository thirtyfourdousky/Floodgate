using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Floodgate.World;

internal class MapRender
{


    public void RenderWIP()
    {
        Vector2 min = new(float.MinValue, float.MinValue);
        Vector2 max = new(float.MaxValue, float.MaxValue);

        Dictionary<string, Rect> imageMeta = new();

        Dictionary<string, RoomInfo> rooms = new();

        rooms["gw_a22"] = new(@"E:\SteamLibrary\steamapps\common\Rain World\RainWorld_Data\StreamingAssets\world\gw-rooms\gw_a22.txt");

        foreach (var room in rooms)
        {
            Vector2 pos = room.Value.position;
            if (pos.x - room.Value.Width < max.x)
            {
                max.x = pos.x - room.Value.Width;
            }
            if (pos.x + room.Value.Width > min.x)
            {
                min.x = pos.x + room.Value.Width;
            }
            if (pos.y - room.Value.Height < max.y)
            {
                max.y = pos.y - room.Value.Width;
            }
            if (pos.y + room.Value.Width > min.y)
            {
                min.y = pos.y + room.Value.Width;
            }
        }

        int num = (int)(min.y - max.y) + 20;
        Texture2D mapTexture = new Texture2D((int)(min.x - max.x) + 20, num * 3);

        for (int i = 0; i < mapTexture.width; i++)
        {
            for (int j = 0; j < mapTexture.height; j++)
            {
                mapTexture.SetPixel(i, j, new(0f, 1f, 0f));
            }
        }
        foreach (var room in rooms)
        {
            IntVector2 intVector = IntVector2.FromVector2(room.Value.position - max);

            int someimportantnumber = 10;

            int layeredPos = room.Value.Layer * num + 10;

            imageMeta[room.Key] = new Rect(new Vector2((float)(intVector.x + someimportantnumber), (float)(intVector.y + layeredPos)), new Vector2((float)room.Value.Width, (float)room.Value.Height));

            for (int x = 0; x < room.Value.Width; x++)
            {
                for (int y = 0; y < room.Value.Height; y++)
                {
                    if (intVector.x + x + someimportantnumber >= 0 && intVector.x + x + someimportantnumber < mapTexture.width &&
                        intVector.y + y + layeredPos >= 0 && intVector.y + y + layeredPos < mapTexture.height)
                    {
                        Color pixelCoolr = Color.black; //this is the tile color

                        //insert tile logic

                        mapTexture.SetPixel(intVector.x + x + someimportantnumber, intVector.y + y + layeredPos, pixelCoolr);
                    }
                }
            }
        }
    }

    public class RoomInfo
    {
        public Room.Tile[,] Tiles;
        public Vector2 position;
        public int Width;
        public int Height;
        public int Layer;

        public RoomInfo(string path)
        {
            string[] roomInfo = File.ReadAllLines(path);

            string[] roomMeta = roomInfo[1].Split('|');

            Width = int.Parse(roomMeta[0].Split('*')[0]);
            Height = int.Parse(roomMeta[0].Split('*')[1]);

            string[] tilesInfo = roomInfo[11].Split('|');

            Tiles = new Room.Tile[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Tiles[i, j] = new(i, j, Room.Tile.TerrainType.Air, false, false, false, 0, 0);
                }
            }

            IntVector2 pos = new IntVector2(0, Height - 1);

            for (int i = 0; i < tilesInfo.Length - 1; i++)
            {
                string[] tileInfo = tilesInfo[i].Split(',');
                Tiles[pos.x, pos.y].Terrain = (Room.Tile.TerrainType)int.Parse(tileInfo[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

                pos.y--;
                if (pos.y < 0)
                {
                    pos.x++;
                    pos.y = Height - 1;
                }
            }

            position = Vector2.zero;
        }
    }
}