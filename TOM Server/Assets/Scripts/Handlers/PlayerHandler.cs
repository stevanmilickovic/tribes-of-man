using RiptideNetworking;
using UnityEngine;

public static class PlayerHandler
{

    [MessageHandler((ushort)ClientToServerId.join)]
    public static void PlayerJoined(ushort fromClientId, Message message)
    {
        PlayerManager.Singleton.PlayerJoined(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    public static void PlayerInput(ushort fromClientId, Message message)
    {
        bool[] inputs = message.GetBools();
        PlayerManager.Singleton.HandlePlayerInput(fromClientId, inputs);
    }

    [MessageHandler((ushort)ClientToServerId.meleeAttack)]
    public static void PlayerMeleeAttack(ushort fromClientId, Message message)
    {
        Vector2 direction = message.GetVector2();
        ushort tick = message.GetUShort();
        PlayerManager.Singleton.HandlePlayerMeleeAttack(fromClientId, direction, tick);
    }

}
