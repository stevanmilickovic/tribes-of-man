using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemService
{
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    public static Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

    public static void InitializeItems()
    {
        AddToolItem(0, "stick", false, 5);
        AddItem(1, Item.Type.Resource, "wood", true);
        AddMeleeWeaponItem(2, "sword", false, 10, MeleeAttackTypes.Sword, 2f);
        AddFoodItem(3, "berries");
        AddRangedWeaponItem(4, "bow", false, 8);
    }

    private static void AddItem(int id, Item.Type type, string name, bool stackable)
    {
        Item newItem = new Item(type, id, name, stackable);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private static void AddMeleeWeaponItem(int id, string name, bool isStackable, int damage, MeleeAttackTypes meleeAttackType, float dash)
    {
        MeleeWeaponItem newItem = new MeleeWeaponItem(id, name, isStackable, damage, meleeAttackType, dash);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private static void AddRangedWeaponItem(int id, string name, bool isStackable, int damage)
    {
        RangedWeaponItem newItem = new RangedWeaponItem(id, name, isStackable, damage);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private static void AddToolItem(int id, string name, bool isStackable, int effect)
    {
        ToolItem newItem = new ToolItem(id, name, isStackable, effect);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }

    private static void AddFoodItem(int id, string name)
    {
        Item newItem = new Item(Item.Type.Food, id, name, true);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
    }
}
