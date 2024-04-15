using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemService
{

    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    public static Dictionary<string, Item> itemsByName = new Dictionary<string, Item>();

    public static void InitializeItems()
    {
        AddToolItem(0, "stick", false, "stick", 5);
        AddItem(1, Item.Type.Resource, "wood", true, "wood");
        AddMeleeWeaponItem(2, "sword", false, "sword", 10, MeleeAttackTypes.Sword, 2f);
        AddFoodItem(3, "berries", "berries 1");
        AddRangedWeaponItem(4, "bow", false, "bow", 8, "arrow");
    }

    private static Item AddItem(int id, Item.Type type, string name, bool isStackable, string spriteName)
    {
        Item newItem = new Item(type, id, name, isStackable, GetSprite(spriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private static MeleeWeaponItem AddMeleeWeaponItem(int id, string name, bool isStackable, string spriteName, int damage, MeleeAttackTypes meleeAttackType, float dash)
    {
        MeleeWeaponItem newItem = new MeleeWeaponItem(id, name, isStackable, GetSprite(spriteName), damage, meleeAttackType, dash);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private static RangedWeaponItem AddRangedWeaponItem(int id, string name, bool isStackable, string spriteName, int damage, string projectileSpriteName)
    {
        RangedWeaponItem newItem = new RangedWeaponItem(id, name, isStackable, GetSprite(spriteName), damage, GetSprite(projectileSpriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private static ToolItem AddToolItem(int id, string name, bool isStackable, string spriteName, int effect)
    {
        ToolItem newItem = new ToolItem(id, name, isStackable, GetSprite(spriteName), effect);
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    private static Item AddFoodItem(int id, string name, string spriteName)
    {
        Item newItem = new Item(Item.Type.Food, id, name, true, GetSprite(spriteName));
        items.Add(id, newItem);
        itemsByName.Add(name, newItem);
        return newItem;
    }

    public static Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }
}
