using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryManager
{

    public static void HandlePickupRequest(ushort clientId, int x, int y)
    {
        if (!MapUtil.IsWithinRange(x, y, PlayerManager.Singleton.playersByClientId[clientId].transform.position))
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId, 0);
            return;
        }

        Tile tile = MapManager.Singleton.map.tiles[x, y];
        if (tile.itemObject != null)
        {
            if (PlayerManager.Singleton.playersByClientId[clientId].inventory.Add(tile.itemObject))
                tile.itemObject = null;
        }
        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId, 0);
        MapSend.SendTileMessage(tile);
    }

    public static void MoveItems(ushort fromClientId, ushort tick, int slot1, int amount, int slot2)
    {
        if (slot1 == slot2)
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId, tick);
            PlayerSend.SendPlayerEquipment(PlayerManager.Singleton.playersByClientId[fromClientId]);
            return;
        }

        Inventory inventory1 = InventoryUtil.GetInventory(slot1, fromClientId);
        Inventory inventory2 = InventoryUtil.GetInventory(slot2, fromClientId);

        ItemObject item1 = inventory1.slots[InventoryUtil.GetSlotNumber(slot1)];
        ItemObject item2 = inventory2.slots[InventoryUtil.GetSlotNumber(slot2)];

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId, tick);
            PlayerSend.SendPlayerEquipment(PlayerManager.Singleton.playersByClientId[fromClientId]);
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

        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[fromClientId], fromClientId, tick);
        PlayerSend.SendPlayerEquipment(PlayerManager.Singleton.playersByClientId[fromClientId]);
    }

    public static void DropItem(ushort clientId, ushort tick, int slot, int amount)
    {
        Player player = PlayerManager.Singleton.playersByClientId[clientId];
        ItemObject itemObject = InventoryUtil.GetItemObjectFromPlayer(player, slot);
        Inventory inventory = InventoryUtil.GetInventory(slot, clientId);
        int relativeSlotNumber = InventoryUtil.GetSlotNumber(slot);
        amount = amount == 0 ? itemObject.amount : amount;
        if (itemObject == null)
            return;

        Tile tile = MapManager.Singleton.DropItem((int)player.gameObject.transform.position.x, (int)player.gameObject.transform.position.y, itemObject, amount);

        if (tile != null)
        {
            inventory.ReduceSlotAmountByNumber(relativeSlotNumber, amount);
            MapSend.SendTileMessage(tile);
        }
        PlayerSend.SendInventoryMessage(player, clientId, tick);
    }

    public static void CraftItems(ushort clientId, ushort tick, int slot, int tileX, int tileY)
    {
        Crafting.Craft(clientId, tick, slot, tileX, tileY);
    }

    public static void BuildStructure(ushort clientId, ushort tick, int slot, int tileX, int tileY)
    {

        if (!MapUtil.IsWithinRange(tileX, tileY, PlayerManager.Singleton.playersByClientId[clientId].transform.position))
        {
            PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId, tick);
            MapSend.SendTileMessage(MapUtil.GetTile(tileX, tileY));
            return;
        }

        Inventory inventory = PlayerManager.Singleton.playersByClientId[clientId].inventory;
        bool buildSuccessful = Building.Build(PlayerManager.Singleton.playersByClientId[clientId].inventory.slots[slot], MapUtil.GetTile(tileX, tileY));
        if (buildSuccessful)
            inventory.ReduceSlotAmount(slot);
        PlayerSend.SendInventoryMessage(PlayerManager.Singleton.playersByClientId[clientId], clientId, tick);
    }

}
