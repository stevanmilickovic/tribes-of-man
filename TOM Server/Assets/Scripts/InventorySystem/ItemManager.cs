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
    }

    public void InitializeItems()
    {
        AddToolItem(0, "stick", false, 5);
        AddItem(1, Item.Type.Resource, "wood", true);
        AddWeaponItem(2, "sword", false, 10);
    }

    private void AddItem(int id, Item.Type type, string name, bool stackable)
    {
        Item newItem = new Item(type, id, name, stackable);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private void AddWeaponItem(int id, string name, bool isStackable, int damage)
    {
        WeaponItem newItem = new WeaponItem(id, name, isStackable, damage);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private void AddToolItem(int id, string name, bool isStackable, int effect)
    {
        ToolItem newItem = new ToolItem(id, name, isStackable, effect);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }
}
