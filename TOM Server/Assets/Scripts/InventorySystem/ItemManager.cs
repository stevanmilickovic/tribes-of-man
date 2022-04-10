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

    public Dictionary<int, Item> items = new Dictionary<int, Item>();
    public Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

    private void Awake()
    {
        Singleton = this;
        InitializeItems();
    }

    private void InitializeItems()
    {
        AddItem(Item.Type.Weapon, 0, "stick", false);
        AddItem(Item.Type.Resource, 1, "wood", true);
    }

    private void AddItem(Item.Type type, int id, string name, bool stackable)
    {
        Item newItem = new Item(type, id, name, stackable);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }
}
