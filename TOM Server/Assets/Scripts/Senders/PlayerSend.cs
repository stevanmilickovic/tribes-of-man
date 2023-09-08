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
        foreach (Player playerInSight in player.currentChunk.playersInRange)
        {
            SendPlayerPosition(playerInSight, clientId);
        }
    }

    public static void SendInventoryMessage(Player player, ushort clientId, ushort tick)
    {
        if (!PlayerManager.Singleton.playersByClientId.ContainsKey(clientId)) return; //Return if player is offline 
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.playerInventory);
        message.Add(tick);
        MessageExtentions.Add(message, player.inventory);
        MessageExtentions.AddEquipment(message, player.clothes);
        MessageExtentions.AddEquipment(message, player.tools);
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
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendPlayerEquipment(Player player)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.playerEquipment);
        message.AddInt(player.id);
        MessageExtentions.AddEquipment(message, player.clothes);
        MessageExtentions.AddEquipment(message, player.tools);

        foreach (Player playerInSight in player.currentChunk.playersInRange)
        {
            if (playerInSight != player)
                NetworkManager.Singleton.Server.Send(message, playerInSight.currentClientId);
        }
    }

    public static void SendChargingMeleeAttackMessage(MeleeAttackTypes type, Vector2 direction, int fromPlayerId, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.chargingMeleeAttack);
        message.AddInt(fromPlayerId);
        message.AddInt((int)type);
        message.AddVector2(direction);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendExecutingMeleeAttackMessage(MeleeAttackTypes type, Vector2 direction, int fromPlayerId, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.executingMeleeAttack);
        message.AddInt(fromPlayerId);
        message.AddInt((int)type);
        message.AddVector2(direction);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }
}
