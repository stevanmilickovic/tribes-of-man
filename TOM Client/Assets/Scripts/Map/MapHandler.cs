using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class MapHandler
{

    [MessageHandler((ushort)ServerToClientId.chunk)]
    public static void CreateChunk(Message message)
    {
        Chunk chunk = MessageExtensions.GetChunk(message);
        MapManager.Singleton.CreateChunk(chunk);
    }

    [MessageHandler((ushort)ServerToClientId.tile)]
    public static void UpdateTile(Message message)
    {
        Tile tile = MessageExtensions.GetTile(message);
        MapManager.Singleton.UpdateTile(tile);
    }

}
