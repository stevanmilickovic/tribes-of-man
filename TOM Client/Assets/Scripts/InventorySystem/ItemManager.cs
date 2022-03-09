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

    private void Awake()
    {
        Singleton = this;
        items = new Dictionary<int, Item>();
        InitializeItems();
    }

    private void InitializeItems()
    {
        items.Add(0, new Item(Item.Type.Weapon, 0, "stick", false));
        items.Add(1, new Item(Item.Type.Resource, 1, "wood", true));
    }
}
