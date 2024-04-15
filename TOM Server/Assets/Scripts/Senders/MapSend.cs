using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class MapSend
{

    public static void SendSeed(ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.seed);
        message.AddInt(MapManager.Singleton.mapWidth);
        message.AddInt(MapManager.Singleton.mapHeight);
        message.AddInt(MapManager.Singleton.seed);
        message.AddFloat(MapManager.Singleton.noiseScale);
        message.AddInt(MapManager.Singleton.octaves);
        message.AddFloat(MapManager.Singleton.persistence);
        message.AddFloat(MapManager.Singleton.lacunarity);
        message.AddVector2(MapManager.Singleton.offset);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendObjectsInChunk(Chunk chunk, ushort clientId)
    {
        foreach (Tile tile in chunk.tiles)
        {
            if (tile.itemObject != null || tile.structureObject != null)
            {
                SendTileMessage(tile, clientId);
            }
        }
        foreach (Player playerInChunk in chunk.players)
        {
            if (playerInChunk.currentClientId != clientId)
                PlayerSend.SendSpawnPlayerMessage(playerInChunk, clientId);
        }
    }

    public static void SendTileMessage(Tile tile, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.tile);
        MessageExtentions.Add(message, tile);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendTileMessageToPlayersInRange(Tile tile)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.tile);
        MessageExtentions.Add(message, tile);

        foreach (Player player in tile.chunk.playersInRange)
        {
            NetworkManager.Singleton.Server.Send(message, player.currentClientId);
        }
    }

}
