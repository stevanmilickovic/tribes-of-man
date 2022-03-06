using RiptideNetworking;
using Unity;
using UnityEngine;

public static class NewBehaviourScript
{
    
    [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
    public static void SpawnPlayer(Message message)
    {
        int id = message.GetInt();
        string username = message.GetString();
        Vector2 position = message.GetVector2();
        PlayerManager.Singleton.SpawnPlayer(id, username, position);
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
        Vector2 position = message.GetVector2();
        PlayerManager.Singleton.UpdatePlayerPosition(id, position);
    }

}
