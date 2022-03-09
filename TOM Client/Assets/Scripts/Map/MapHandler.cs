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

}
