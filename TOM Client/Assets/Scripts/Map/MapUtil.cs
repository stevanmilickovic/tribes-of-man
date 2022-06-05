using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtil
{

    public static Chunk GetChunk(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        return MapManager.Singleton.chunks[((int)Mathf.Floor((float)x / 10), (int)Mathf.Floor((float)y / 10))];
    }

    public static Chunk GetChunk(int x, int y)
    {
        return MapManager.Singleton.chunks[((int)Mathf.Floor((float)x / 10), (int)Mathf.Floor((float)y / 10))];
    }

}
