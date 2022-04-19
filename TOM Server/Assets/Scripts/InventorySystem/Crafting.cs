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

    private void AddRecipe(string item1name, string item2name, string resultItem, int resultAmount)
    {
        Item item1 = ItemManager.Singleton.itemsByName[item1name];
        Item item2 = ItemManager.Singleton.itemsByName[item2name];
        Item result = ItemManager.Singleton.itemsByName[resultItem];
        recipes.Add((item1, item2), (result, resultAmount));
    }

    public void Craft(ushort clientId, int slot, int tileX, int tileY)
    {
        Player player = PlayerManager.Singleton.playersByClientId[clientId];
        ItemObject item = player.inventory.slots[slot];
        Tile tile = MapManager.Singleton.map.tiles[tileX, tileY];
        Tile droppedTile = null;

        if (tile.itemObject != null && item != null && GetRecipe(item, tile.itemObject) != null)
        {
            ItemObject craftedObject = GetRecipe(item, tile.itemObject);
            if (tile.itemObject.amount == 1)
            {
                tile.itemObject = craftedObject;
                player.inventory.ReduceSlotAmount(slot);
            }
            else
            {
                droppedTile = MapManager.Singleton.DropItem(tileX, tileY, craftedObject);
                if(droppedTile != null)
                {
                    tile.itemObject.amount -= 1;
                    player.inventory.ReduceSlotAmount(slot);
                }
            }
        }

        if (droppedTile != null)
            MapManager.Singleton.SendTileMessage(droppedTile);
        MapManager.Singleton.SendTileMessage(tile);
        PlayerManager.Singleton.SendInventoryMessage(player, clientId);
    }

    private ItemObject GetRecipe(ItemObject item1, ItemObject item2)
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

    private bool RecipeExists(ItemObject item1, ItemObject item2)
    {
        if (recipes.ContainsKey((item1.item, item2.item)))
        {
            return true;
        }
        return false;
    }
}
