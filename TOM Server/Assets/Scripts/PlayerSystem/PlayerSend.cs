using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class PlayerSend
{

    public static void SendBasicRelevantInformation(Player player, ushort clientId)
    {
        Chunk chunk = player.currentChunk;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int chunkX = chunk.x + x;
                int chunkY = chunk.y + y;
                if (chunkX > -1 && chunkX <= MapManager.Singleton.mapWidth/10 && chunkY > -1 && chunkY <= MapManager.Singleton.mapWidth/10)
                {
                    Chunk iterableChunk = MapManager.Singleton.map.chunks[chunkX, chunkY];
                    MapSend.SendChunkMessage(clientId, iterableChunk);
                    foreach (Player playerInChunk in iterableChunk.players)
                    {
                        if(playerInChunk != player)
                            SendSpawnPlayerMessage(playerInChunk, clientId);
                    }
                }
            }
        }
    }

    public static void SendRelevantPlayerPosition(Player player, ushort clientId)
    {
        Chunk chunk = player.currentChunk;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int chunkX = chunk.x + x;
                int chunkY = chunk.y + y;
                if (chunkX > -1 && chunkX <= MapManager.Singleton.mapWidth / 10 && chunkY > -1 && chunkY <= MapManager.Singleton.mapWidth / 10)
                {
                    Chunk iterableChunk = MapManager.Singleton.map.chunks[chunkX, chunkY];
                    foreach (Player playerInChunk in iterableChunk.players)
                    {
                        SendPlayerPosition(playerInChunk, clientId);
                    }
                }
            }
        }
    }

    public static void SendInventoryMessage(Player player, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.playerInventory);
        MessageExtentions.Add(message, player.inventory);
        NetworkManager.Singleton.Server.Send(message, clientId);
        PlayerManager.Singleton.playersByClientId[clientId].CheckEquipment();
    }

    public static void SendYourIdMessage(Player player, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.yourPlayerId);
        message.Add(player.id);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendSpawnPlayerMessageToAll(Player player)
    {
        NetworkManager.Singleton.Server.SendToAll(PackPlayerInMessage(player));
    }

    public static void SendSpawnPlayerMessage(Player player, ushort clientId)
    {
        NetworkManager.Singleton.Server.Send(PackPlayerInMessage(player), clientId);
    }

    public static Message PackPlayerInMessage(Player player)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.spawnPlayer);
        MessageExtentions.AddPlayer(message, player);

        return message;
    }

    public static void SendSpawnAllPlayers(ushort clientId)
    {
        foreach (Player player in PlayerManager.Singleton.playersById.Values)
        {
            SendSpawnPlayerMessage(player, clientId);
        }
    }

    public static void SpawnAllRelevantPlayers(Player player, ushort clientId)
    {

    }

    public static void SendPlayerPosition(Player player, ushort clientId)
    {

        if (NetworkManager.Singleton.CurrentTick % 2 != 0)
            return;

        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerPosition);
        message.AddInt(player.id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector2(new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y));
        message.AddInt(player.health);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }
}
