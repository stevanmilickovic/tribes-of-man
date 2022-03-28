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
    public Dictionary<(Item, int, Item, int), (Item, int)> recipes;

    private void Start()
    {
        recipes = new Dictionary<(Item, int, Item, int), (Item, int)>();
        InitializeRecipes();
        Singleton = this;
    }

    private void InitializeRecipes()
    {
        AddRecipe(0, 1, 0, 1, 1, 1); //Stick + Stick = Wood
    }

    private void AddRecipe(int item1Id, int item1Amount, int item2Id, int item2Amount, int resultId, int resultAmount)
    {
        Item item1 = ItemManager.Singleton.items[item1Id];
        Item item2 = ItemManager.Singleton.items[item2Id];
        Item result = ItemManager.Singleton.items[resultId];
        recipes.Add((item1, item1Amount, item2, item2Amount), (result, resultAmount));
    }

    public void Craft(int slot, int tileX, int tileY)
    {
        Player player = PlayerManager.Singleton.myPlayer;
        Inventory inventory = PlayerManager.Singleton.myPlayer.inventory;
        ItemObject item = inventory.slots[slot];
        Tile tile = MapManager.Singleton.tiles[(tileX, tileY)];

        if (tile.itemObject != null && item != null)
        {
            if (RecipeExists(item, tile.itemObject))
            {
                tile.itemObject = GetRecipe(item, tile.itemObject);
                inventory.slots[slot] = null;
                MapManager.Singleton.UpdateTile(tile);
            }
            else if (RecipeExists(tile.itemObject, item)) // Inverse order for recipe key
            {
                tile.itemObject = GetRecipe(tile.itemObject, item);
                inventory.slots[slot] = null;
                MapManager.Singleton.UpdateTile(tile);
            }
        }

        PlayerManager.Singleton.CraftItems(slot, tileX, tileY);
    }

    public ItemObject GetRecipe(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item1.amount, item2.item, item2.amount)))
        {
            (Item item, int amount) result = recipes[(item1.item, item1.amount, item2.item, item2.amount)];
            return new ItemObject(result.item, result.amount);
        }
        return null;
    }

    public bool RecipeExists(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item1.amount, item2.item, item2.amount)))
        {
            return true;
        }
        return false;
    }
}
