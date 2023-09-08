using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CraftingService
{

    public static Dictionary<(Item, Item), (Item, int)> recipes = new Dictionary<(Item, Item), (Item, int)>();

    public static void InitializeRecipes()
    {
        AddRecipe("stick", "stick", "wood", 1);
        AddRecipe("wood", "wood", "stick", 1);
    }

    private static void AddRecipe(string item1Name, string item2Name, string resultName, int resultAmount)
    {
        Item item1 = ItemService.itemsByName[item1Name];
        Item item2 = ItemService.itemsByName[item2Name];
        Item result = ItemService.itemsByName[resultName];
        recipes.Add((item1, item2), (result, resultAmount));
    }

    public static void Craft(Inventory inventory, int absoluteSlot, int tileX, int tileY)
    {
        int slot = InventoryUtil.GetSlotNumber(absoluteSlot);
        ItemObject item = inventory.slots[slot];
        Tile tile = MapManager.Singleton.tiles[(tileX, tileY)];

        if (tile.itemObject != null && item != null && GetRecipe(item, tile.itemObject) != null)
        {

            if (item.item == ItemService.itemsByName["stick"] && tile.itemObject.item == ItemService.itemsByName["stick"])
            {
                tile.itemObject = null;
                inventory.ReduceSlotAmount(slot);
                tile.structureObject = new StructureObject(StructureService.structuresByName["Frame"]);
                MapManager.Singleton.UpdateTileItemAndStructure(tile);
            }
            else
            {
                ItemObject craftedItem = GetRecipe(item, tile.itemObject);
                if (tile.itemObject.amount == 1)
                {
                    tile.itemObject = craftedItem;
                    inventory.ReduceSlotAmount(slot);
                }
                else
                {
                    Tile droppedTile = MapManager.Singleton.DropItem(tileX, tileY, craftedItem);
                    if (droppedTile != null)
                    {
                        tile.itemObject.amount -= 1;
                        inventory.ReduceSlotAmount(slot);
                        MapManager.Singleton.UpdateTileItemAndStructure(droppedTile);
                    }
                }
            }
            MapManager.Singleton.UpdateTileItemAndStructure(tile);
        }

        InventoryManager.CreateInventoryState(NetworkManager.Singleton.ServerTick, absoluteSlot, 0, 0, true, InventoryManager.lastAddedState);
        InventorySender.SendCraftItemsMessage(absoluteSlot, tileX, tileY);
    }

    public static ItemObject GetRecipe(ItemObject item1, ItemObject item2)
    {
        if (RecipeExists(item1, item2))
        {
            (Item item, int amount) result = recipes[(item1.item, item2.item)];
            return new ItemObject(result.item, result.amount);
        }
        else if(RecipeExists(item2, item1))
        {
            (Item item, int amount) result = recipes[(item2.item, item1.item)];
            return new ItemObject(result.item, result.amount);
        }
        return null;
    }

    public static bool RecipeExists(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item2.item)))
        {
            return true;
        }
        return false;
    }
}
