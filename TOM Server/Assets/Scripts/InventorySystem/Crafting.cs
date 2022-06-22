using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Crafting
{
    public static Dictionary<(Item, Item), (Item, int)> recipes = new Dictionary<(Item, Item), (Item, int)>();

    public static void InitializeRecipes()
    {
        AddRecipe("stick", "stick", "wood", 1);
        AddRecipe("wood", "wood", "stick", 1);
    }

    private static void AddRecipe(string item1name, string item2name, string resultItem, int resultAmount)
    {
        Item item1 = ItemManager.Singleton.itemsByName[item1name];
        Item item2 = ItemManager.Singleton.itemsByName[item2name];
        Item result = ItemManager.Singleton.itemsByName[resultItem];
        recipes.Add((item1, item2), (result, resultAmount));
    }

    public static void Craft(ushort clientId, int slot, int tileX, int tileY)
    {
        Player player = PlayerManager.Singleton.playersByClientId[clientId];
        ItemObject item = InventoryUtil.GetItemObjectFromPlayer(player, slot);
        Tile tile = MapManager.Singleton.map.tiles[tileX, tileY];
        Tile droppedTile = null;

        if (tile.itemObject != null && item != null && GetRecipe(item, tile.itemObject) != null)
        {
            if (IsFrameRecipe(tile.itemObject, item))
            {
                tile.itemObject = null;
                tile.SpawnStructure(StructureManager.Singleton.structuresByName["Frame"]);
            }
            else
            {
                ItemObject craftedObject = GetRecipe(item, tile.itemObject);
                if (tile.itemObject.amount == 1)
                {
                    tile.itemObject = craftedObject;
                    InventoryUtil.GetPlayerInventoryBySlot(player, slot).ReduceSlotAmount(InventoryUtil.GetRelativeSlotNumber(slot));
                }
                else
                {
                    droppedTile = MapManager.Singleton.DropItem(tileX, tileY, craftedObject);
                    if (droppedTile != null)
                    {
                        tile.itemObject.amount -= 1;
                        InventoryUtil.GetPlayerInventoryBySlot(player, slot).ReduceSlotAmount(InventoryUtil.GetRelativeSlotNumber(slot));
                    }
                }
            }
        }

        if (droppedTile != null)
            MapSend.SendTileMessage(droppedTile);
        MapSend.SendTileMessage(tile);
        PlayerSend.SendInventoryMessage(player, clientId);
    }

    private static ItemObject GetRecipe(ItemObject item1, ItemObject item2)
    {
        if (RecipeExists(item1, item2))
        {
            (Item item, int amount) result = recipes[(item1.item, item2.item)];
            return new ItemObject(result.item, result.amount);
        }
        else if (RecipeExists(item2, item1))
        {
            (Item item, int amount) result = recipes[(item2.item, item1.item)];
            return new ItemObject(result.item, result.amount);
        }
        return null;
    }

    private static bool RecipeExists(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item2.item)))
        {
            return true;
        }
        return false;
    }

    private static bool IsFrameRecipe(ItemObject item1, ItemObject item2)
    {
        if (item1.item == ItemManager.Singleton.itemsByName["stick"] && item2.item == ItemManager.Singleton.itemsByName["stick"])
            return true;
        return false;
    }
}
