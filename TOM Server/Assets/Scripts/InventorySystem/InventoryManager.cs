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

    public static void MoveItems(ushort fromClientId, int slot1, int amount, int slot2)
    {
        if (slot1 == slot2)
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId);
            PlayerSend.SendPlayerEquipment(PlayerManager.Singleton.playersByClientId[fromClientId]);
            return;
        }

        Inventory inventory1 = InventoryUtil.GetInventory(slot1, fromClientId);
        Inventory inventory2 = InventoryUtil.GetInventory(slot2, fromClientId);

        ItemObject item1 = inventory1.slots[InventoryUtil.GetSlotNumber(slot1)];
        ItemObject item2 = inventory2.slots[InventoryUtil.GetSlotNumber(slot2)];

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId);
            return;
        }

        if (amount == 0)
        {
            if (item1 != null && item2 != null && item1.item == item2.item && item1.item.stackable)
            {
                item2.amount += item1.amount;
                inventory1.slots[InventoryUtil.GetSlotNumber(slot1)] = null;
            }
            else
            {
                inventory1.slots[InventoryUtil.GetSlotNumber(slot1)] = item2;
                inventory2.slots[InventoryUtil.GetSlotNumber(slot2)] = item1;
            }
        }
        else if (amount == 1)
        {
            if (item2 == null)
            {
                inventory1.ReduceSlotAmount(InventoryUtil.GetSlotNumber(slot1));
                inventory2.slots[InventoryUtil.GetSlotNumber(slot2)] = new ItemObject(item1.item, 1);
            }
            else if (item2.item == item1.item && item2.item.stackable)
            {
                inventory1.ReduceSlotAmount(InventoryUtil.GetSlotNumber(slot1));
                item2.amount += 1;
            }
        }
        else if (amount == (int)Mathf.Floor(item1.amount / 2))
        {
            if (item2 == null)
            {
                inventory2.slots[InventoryUtil.GetSlotNumber(slot2)] = new ItemObject(item1.item, (int)Mathf.Floor(item1.amount / 2));
                inventory1.ReduceSlotAmountByNumber(InventoryUtil.GetSlotNumber(slot1), (int)Mathf.Floor(item1.amount / 2));
            }
            else if (item2.item == item1.item && item2.item.stackable)
            {
                item2.amount += (int)Mathf.Floor(item1.amount / 2);
                inventory1.ReduceSlotAmountByNumber(InventoryUtil.GetSlotNumber(slot1), (int)Mathf.Floor(item1.amount / 2));
            }
        }

        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId);
        PlayerSend.SendPlayerEquipment(PlayerManager.Singleton.playersByClientId[fromClientId]);
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
        Crafting.Craft(clientId, slot, tileX, tileY);
    }

    public static void BuildStructure(ushort clientId, int slot, int tileX, int tileY)
    {
        Inventory inventory = PlayerManager.Singleton.playersByClientId[clientId].inventory;
        bool buildSuccessful = Building.Build(PlayerManager.Singleton.playersByClientId[clientId].inventory.slots[slot], MapUtil.GetTile(tileX, tileY));
        if (buildSuccessful)
            inventory.ReduceSlotAmount(slot);
        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId);
    }

}
