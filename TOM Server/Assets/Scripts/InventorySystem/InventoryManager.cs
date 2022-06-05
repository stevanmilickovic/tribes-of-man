using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryManager
{

    public static void HandlePickupRequest(ushort clientId, int x, int y)
    {
        Tile tile = MapManager.Singleton.map.tiles[x, y];
        if (tile.itemObject != null)
        {
            if (PlayerManager.Singleton.playersByClientId[clientId].inventory.Add(tile.itemObject))
                tile.itemObject = null;
        }
        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId);
        MapSend.SendTileMessage(tile);
    }

    public static void SwapItems(ushort fromClientId, int slot1, int slot2)
    {
        Inventory inventory1 = InventoryUtil.GetInventory(slot1, fromClientId);
        Inventory inventory2 = InventoryUtil.GetInventory(slot2, fromClientId);

        ItemObject item1 = inventory1.slots[InventoryUtil.GetSlotNumber(slot1)];
        ItemObject item2 = inventory2.slots[InventoryUtil.GetSlotNumber(slot2)];

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId);
            return;
        }

        inventory1.slots[InventoryUtil.GetSlotNumber(slot1)] = item2;
        inventory2.slots[InventoryUtil.GetSlotNumber(slot2)] = item1;

        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId);
    }

    public static void DropItem(ushort clientId, int slot)
    {
        Player player = PlayerManager.Singleton.playersByClientId[clientId];
        ItemObject itemObject = InventoryUtil.GetItemObjectFromPlayer(player, slot);
        if (itemObject == null)
            return;

        Tile tile = MapManager.Singleton.DropItem((int)player.gameObject.transform.position.x, (int)player.gameObject.transform.position.y, itemObject);

        if (tile != null)
        {
            EmptyPlayerInventorySlot(player, slot);
            MapSend.SendTileMessage(tile);
        }
        PlayerSend.SendInventoryMessage(player, clientId);
    }

    private static void EmptyPlayerInventorySlot(Player player, int slot)
    {
        if (slot < 9)
        {
            player.inventory.slots[InventoryUtil.GetRelativeSlotNumber(slot)] = null;
            return;
        }
        if (slot < 12)
        {
            player.clothes.slots[InventoryUtil.GetRelativeSlotNumber(slot)] = null;
            return;
        }
        if (slot < 14)
        {
            player.tools.slots[InventoryUtil.GetRelativeSlotNumber(slot)] = null;
            return;
        }
    }

    public static void CraftItems(ushort clientId, int slot, int tileX, int tileY)
    {
        Crafting.Singleton.Craft(clientId, slot, tileX, tileY);
    }

}
