using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class InventoryHandle
{

    [MessageHandler((ushort)ClientToServerId.pickup)]
    public static void PlayerPickup(ushort fromClientId, Message message)
    {
        int x = message.GetInt();
        int y = message.GetInt();
        InventoryManager.HandlePickupRequest(fromClientId, x, y);
    }

    [MessageHandler((ushort)ClientToServerId.moveItems)]
    public static void PlayerItemMove(ushort fromClientId, Message message)
    {
        ushort tick = message.GetUShort();
        int slot1 = message.GetInt();
        int amount = message.GetInt();
        int slot2 = message.GetInt();
        InventoryManager.MoveItems(fromClientId, tick, slot1, amount, slot2);
    }

    [MessageHandler((ushort)ClientToServerId.drop)]
    public static void PlayerItemDrop(ushort fromClientId, Message message)
    {
        ushort tick = message.GetUShort();
        int slot = message.GetInt();
        int amount = message.GetInt();
        InventoryManager.DropItem(fromClientId, tick, slot, amount);
    }

    [MessageHandler((ushort)ClientToServerId.craft)]
    public static void PlayerCraft(ushort fromClientId, Message message)
    {
        ushort tick = message.GetUShort();
        int slot = message.GetInt();
        int tileX = message.GetInt();
        int tileY = message.GetInt();
        InventoryManager.CraftItems(fromClientId, tick, slot, tileX, tileY);
    }

    [MessageHandler((ushort)ClientToServerId.build)]
    public static void PlayerBuild(ushort fromClientId, Message message)
    {
        ushort tick = message.GetUShort();
        int slot = message.GetInt();
        int tileX = message.GetInt();
        int tileY = message.GetInt();
        InventoryManager.BuildStructure(fromClientId, tick, slot, tileX, tileY);
    }

}
