using RiptideNetworking;
using Unity;
using UnityEngine;

public static class PlayerHandler
{
    
    [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
    public static void SpawnPlayer(Message message)
    {
        int id = message.GetInt();
        string username = message.GetString();
        Vector2 position = message.GetVector2();

        int health = message.GetInt();
        int hunger = message.GetInt();

        Equipment clothes = MessageExtensions.GetClothes(message);
        Equipment tools = MessageExtensions.GetTools(message);

        PlayerManager.Singleton.SpawnPlayer(id, username, position, health, hunger, clothes, tools);
    }

    [MessageHandler((ushort)ServerToClientId.yourPlayerId)]
    public static void PlayerId(Message message)
    {
        PlayerManager.Singleton.SetMyId(message.GetInt());
    }

    [MessageHandler((ushort)ServerToClientId.playerPosition)]
    public static void PlayerPosition(Message message)
    {
        int id = message.GetInt();
        ushort tick = message.GetUShort();
        Vector2 position = message.GetVector2();

        int health = message.GetInt();
        int hunger = message.GetInt();

        PlayerManager.Singleton.UpdatePlayerPosition(id, tick, position);
        PlayerManager.Singleton.UpdatePlayerHealth(id, health);
        PlayerManager.Singleton.UpdatePlayerHunger(id, hunger);
    }

}
