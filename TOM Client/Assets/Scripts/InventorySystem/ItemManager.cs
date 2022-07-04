using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager singleton;

    public static ItemManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(ItemManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Dictionary<int, Item> items;
    public Dictionary<string, Item> itemsByName;

    private void Awake()
    {
        Singleton = this;
        items = new Dictionary<int, Item>();
        itemsByName = new Dictionary<string, Item>();
    }

    public void InitializeItems()
    {
        AddToolItem(0, "stick", false, "stick", 5);
        AddItem(1, Item.Type.Resource, "wood", true, "wood");
        AddWeaponItem(2, "sword", false, "sword", 10);
        AddFoodItem(3, "berries", "berries 1");
    }

    private Item AddItem(int id, Item.Type type, string name, bool isStackable, string spriteName)
    {
        Item newItem = new Item(type, id, name, isStackable, GetSprite(spriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private WeaponItem AddWeaponItem(int id, string name, bool isStackable, string spriteName, int damage)
    {
        WeaponItem newItem = new WeaponItem(id, name, isStackable, GetSprite(spriteName), damage);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private ToolItem AddToolItem(int id, string name, bool isStackable, string spriteName, int effect)
    {
        ToolItem newItem = new ToolItem(id, name, isStackable, GetSprite(spriteName), effect);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private Item AddFoodItem(int id, string name, string spriteName)
    {
        Item newItem = new Item(Item.Type.Food, id, name, true, GetSprite(spriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    public Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }
}
