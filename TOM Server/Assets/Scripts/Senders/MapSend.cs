using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class MapSend
{

    public static void SendRelevantChunks(Player player, ushort clientId)
    {
        Chunk chunk = player.currentChunk;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int chunkX = chunk.x + x;
                int chunkY = chunk.y + y;
                if (chunkX > -1 && chunkY > -1)
                {
                    SendChunkMessage(clientId, MapManager.Singleton.map.chunks[chunkX, chunkY]);
                }
            }
        }
    }

    public static void SendAllChunks(ushort clientId, Map map)
    {
        foreach (Chunk chunk in map.chunks)
        {
            SendChunkMessage(clientId, chunk);
        }
    }

    public static void SendChunkMessage(ushort clientId, Chunk chunk)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.chunk);
        MessageExtentions.Add(message, chunk);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendTileMessage(Tile tile)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.tile);
        MessageExtentions.Add(message, tile);

        foreach (Player player in tile.chunk.playersInRange)
        {
            NetworkManager.Singleton.Server.Send(message, player.currentClientId);
        }
    }

}
