using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryUtil
{
    public static Inventory GetInventory(int i, ushort fromClientId)
    {
        if (i < 9)
            return PlayerManager.Singleton.playersByClientId[fromClientId].inventory;
        else if (i < 12)
            return PlayerManager.Singleton.playersByClientId[fromClientId].clothes;
        else
            return PlayerManager.Singleton.playersByClientId[fromClientId].tools;
    }

    public static int GetSlotNumber(int i)
    {
        if (i < 9)
            return i;
        else if (i < 12)
            return i - 9;
        else
            return i - 12;
    }

    public static ItemObject GetItemObjectFromPlayer(Player player, int slot)
    {
        if (slot < 9)
            return player.inventory.slots[GetRelativeSlotNumber(slot)];
        if (slot < 12)
            return player.clothes.slots[GetRelativeSlotNumber(slot)];
        if (slot < 14)
            return player.tools.slots[GetRelativeSlotNumber(slot)];
        return null;
    }

    public static Inventory GetPlayerInventoryBySlot(Player player, int slot)
    {
        if (slot < 9)
            return player.inventory;
        if (slot < 12)
            return player.clothes;
        if (slot < 14)
            return player.tools;
        return null;
    }

    public static int GetRelativeSlotNumber(int slot)
    {
        if (slot < 9)
            return slot;
        if (slot < 12)
            return slot - 9;
        if (slot < 14)
            return slot - 12;
        return -1;
    }
}
