using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class PlayerSend
{

    public static void SendAllObjectsInRange(Player player, ushort clientId)
    {
        foreach (Chunk chunk in player.chunksInRange)
        {
            MapSend.SendObjectsInChunk(chunk, clientId);
        }
    }

    public static void SendPositionOfPlayersInRange(Player player, ushort clientId)
    {
        foreach (Player playerInRange in player.currentChunk.playersInRange)
        {
            SendPlayerPosition(playerInRange, clientId);
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
        PlayerManager.Singleton.playersByClientId[clientId].UpdateEquipmentStatus();
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

    public static void SendChargingAttackMessage(Vector2 direction, int fromPlayerId, ushort clientId, ushort tick)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.chargingAttack);
        message.AddInt(fromPlayerId);
        message.AddVector2(direction);
        message.AddUShort(tick);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public static void SendExecutingAttackMessage(Vector2 direction, int fromPlayerId, ushort clientId, ushort tick)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.executingAttack);
        message.AddInt(fromPlayerId);
        message.AddVector2(direction);
        message.AddUShort(tick);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }
}
