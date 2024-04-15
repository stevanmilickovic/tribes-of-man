using RiptideNetworking;
using Unity;
using UnityEngine;

public static class PlayerHandler
{
    
    [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
    public static void SpawnPlayer(Message message)
    {
        Debug.Log("Received spawn player");
        int id = message.GetInt();
        string username = message.GetString();
        Vector2 position = message.GetVector2();

        Equipment clothes = MessageExtensions.GetClothes(message);
        Equipment tools = MessageExtensions.GetTools(message);

        PlayerPacket packet = new PlayerPacket(id, username, position, clothes, tools);

        PlayerManager.Singleton.SpawnPlayer(new PlayerPacket(id, username, position, clothes, tools));
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

        PlayerPacket packet = new PlayerPacket(id, position);

        PlayerManager.Singleton.UpdatePlayerPosition(id, tick, position);
    }

    [MessageHandler((ushort)ServerToClientId.chargingAttack)]
    public static void PlayerChargingAttack(Message message)
    {
        int fromPlayerId = message.GetInt();
        Vector2 direction = message.GetVector2();
        ushort tick = message.GetUShort();

        PlayerManager.Singleton.ChargePlayerAttack(fromPlayerId, direction, tick);
    }

    [MessageHandler((ushort)ServerToClientId.executingAttack)]
    public static void PlayerExecutingAttack(Message message)
    {
        int fromPlayerId = message.GetInt();
        Vector2 direction = message.GetVector2();
        ushort tick = message.GetUShort();

        PlayerManager.Singleton.ExecutePlayerAttack(fromPlayerId, direction);
    }

}
