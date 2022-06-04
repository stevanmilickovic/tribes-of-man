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
        PlayerManager.Singleton.PlayerInput(fromClientId, inputs);
    }

    [MessageHandler((ushort)ClientToServerId.pickup)]
    public static void PlayerPickup(ushort fromClientId, Message message)
    {
        int x = message.GetInt();
        int y = message.GetInt();
        PlayerManager.Singleton.Pickup(fromClientId, x, y);
    }

    [MessageHandler((ushort)ClientToServerId.swap)]
    public static void PlayerItemSwap(ushort fromClientId, Message message)
    {
        int slot1 = message.GetInt();
        int slot2 = message.GetInt();
        PlayerManager.Singleton.SwapItems(fromClientId, slot1, slot2);
    }

    [MessageHandler((ushort)ClientToServerId.drop)]
    public static void PlayerItemDrop(ushort fromClientId, Message message)
    {
        int slot = message.GetInt();
        PlayerManager.Singleton.DropItem(fromClientId, slot);
    }

    [MessageHandler((ushort)ClientToServerId.craft)]
    public static void PlayerCraft(ushort fromClientId, Message message)
    {
        int slot = message.GetInt();
        int tileX = message.GetInt();
        int tileY = message.GetInt();
        PlayerManager.Singleton.CraftItems(fromClientId, slot, tileX, tileY);
    }

    [MessageHandler((ushort)ClientToServerId.attack)]
    public static void PlayerAttack(ushort fromClientId, Message message)
    {
        Vector2 direction = message.GetVector2();
        ushort tick = message.GetUShort();
        PlayerManager.Singleton.PlayerAttack(fromClientId, direction, tick);
    }

}
