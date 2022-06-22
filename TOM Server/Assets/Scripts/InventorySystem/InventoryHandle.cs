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

    [MessageHandler((ushort)ClientToServerId.swap)]
    public static void PlayerItemSwap(ushort fromClientId, Message message)
    {
        int slot1 = message.GetInt();
        int slot2 = message.GetInt();
        InventoryManager.SwapItems(fromClientId, slot1, slot2);
    }

    [MessageHandler((ushort)ClientToServerId.drop)]
    public static void PlayerItemDrop(ushort fromClientId, Message message)
    {
        int slot = message.GetInt();
        InventoryManager.DropItem(fromClientId, slot);
    }

    [MessageHandler((ushort)ClientToServerId.craft)]
    public static void PlayerCraft(ushort fromClientId, Message message)
    {
        int slot = message.GetInt();
        int tileX = message.GetInt();
        int tileY = message.GetInt();
        InventoryManager.CraftItems(fromClientId, slot, tileX, tileY);
    }

    [MessageHandler((ushort)ClientToServerId.build)]
    public static void PlayerBuild(ushort fromClientId, Message message)
    {
        int slot = message.GetInt();
        int tileX = message.GetInt();
        int tileY = message.GetInt();
        InventoryManager.BuildStructure(fromClientId, slot, tileX, tileY);
    }

}
