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
        AddItem(0, Item.Type.Weapon, "stick", false, "stick");
        AddItem(1, Item.Type.Resource, "wood", false, "wood");
    }

    private Item AddItem(int id, Item.Type type, string name, bool isStackable, string spriteName)
    {
        Item newItem = new Item(type, id, name, isStackable, GetSprite(spriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    public Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }
}
