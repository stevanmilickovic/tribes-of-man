using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtil
{

    public static Chunk GetChunk(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        return MapManager.Singleton.map.chunks[(int)Mathf.Floor((float)x / Map.CHUNK_SIZE), (int)Mathf.Floor((float)y / Map.CHUNK_SIZE)];
    }

    public static Chunk GetChunk(int x, int y)
    {
        if (x < 0 || y < 0 || x >= MapManager.Singleton.mapWidth / Map.CHUNK_SIZE || y >= MapManager.Singleton.mapHeight / Map.CHUNK_SIZE) return null;
        return MapManager.Singleton.map.chunks[x, y];
    }

    public static Tile GetTile(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        return MapManager.Singleton.map.tiles[x, y];
    }

    public static Tile GetTile(int x, int y)
    {
        return MapManager.Singleton.map.tiles[x, y];
    }

    public static bool IsWithinRange(int x, int y, Vector2 position)
    {
        if (Mathf.Abs(x - (int)position.x) > 1 || Mathf.Abs(y - (int)position.y) > 1)
            return false;
        return true;
    }

}
