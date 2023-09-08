using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapUtil
{

    public static Chunk GetChunk(int x, int y)
    {
        if (!MapManager.Singleton.chunks.ContainsKey(((int)Mathf.Floor((float)x / MapManager.CHUNK_SIZE), (int)Mathf.Floor((float)y / MapManager.CHUNK_SIZE)))) return null;
        return MapManager.Singleton.chunks[((int)Mathf.Floor((float)x / MapManager.CHUNK_SIZE), (int)Mathf.Floor((float)y / MapManager.CHUNK_SIZE))];
    }

    public static Chunk GetChunk(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        return GetChunk(x, y);
    }

    public static Chunk GetChunk(Tile tile)
    {
        return GetChunk(tile.x, tile.y);
    }

    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return new ArraySegment<T>(array, offset, length)
                    .ToArray();
    }

}
