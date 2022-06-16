using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class PlayerSend
{

    public static void SendBasicRelevantInformation(Player player, ushort clientId)
    {
        foreach (Chunk chunk in player.chunksInRange)
        {
            MapSend.SendChunkMessage(clientId, chunk);
            foreach (Player playerInChunk in chunk.players)
            {
                if (playerInChunk != player)
                    SendSpawnPlayerMessage(playerInChunk, clientId);
            }
        }
    }

    public static void SendRelevantPlayerPosition(Player player, ushort clientId)
    {

        foreach (Chunk chunk in player.chunksInRange)
        {
            foreach (Player playerInChunk in chunk.players)
                SendPlayerPosition(playerInChunk, clientId);
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
