using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    private static Crafting singleton;

    public static Crafting Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(Crafting)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    public Dictionary<(Item, Item), (Item, int)> recipes;

    private void Awake()
    {
        recipes = new Dictionary<(Item, Item), (Item, int)>();
        Singleton = this;
    }

    public void InitializeRecipes()
    {
        AddRecipe("stick", "stick", "wood", 1);
        AddRecipe("wood", "wood", "stick", 1);
    }

    private void AddRecipe(string item1Name, string item2Name, string resultName, int resultAmount)
    {
        Item item1 = ItemManager.Singleton.itemsByName[item1Name];
        Item item2 = ItemManager.Singleton.itemsByName[item2Name];
        Item result = ItemManager.Singleton.itemsByName[resultName];
        recipes.Add((item1, item2), (result, resultAmount));
    }

    public void Craft(Inventory inventory, int absoluteSlot, int tileX, int tileY)
    {
        int slot = InventoryUtil.GetSlotNumber(absoluteSlot);
        ItemObject item = inventory.slots[slot];
        Tile tile = MapManager.Singleton.tiles[(tileX, tileY)];

        if (tile.itemObject != null && item != null && GetRecipe(item, tile.itemObject) != null)
        {

            if (item.item == ItemManager.Singleton.itemsByName["stick"] && tile.itemObject.item == ItemManager.Singleton.itemsByName["stick"])
            {
                tile.itemObject = null;
                inventory.ReduceSlotAmount(slot);
                tile.structureObject = new StructureObject(StructureManager.Singleton.structuresByName["Frame"]);
                MapManager.Singleton.UpdateTile(tile);
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
                        MapManager.Singleton.UpdateTile(droppedTile);
                    }
                }
            }
            MapManager.Singleton.UpdateTile(tile);
        }

        InventoryManager.CreateInventoryState(NetworkManager.Singleton.ServerTick, absoluteSlot, 0, 0, true, InventoryManager.lastAddedState);
        PlayerManager.Singleton.CraftItems(absoluteSlot, tileX, tileY);
    }

    public ItemObject GetRecipe(ItemObject item1, ItemObject item2)
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

    public bool RecipeExists(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item2.item)))
        {
            return true;
        }
        return false;
    }
}
