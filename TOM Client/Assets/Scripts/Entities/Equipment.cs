using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Equipment : Inventory
{

    public Equipment(ItemObject[] _slots, Item.Type[] _itemTypes) : base(_slots, _itemTypes) { }

    public ItemObject GetMainWeapon()
    {
        foreach (ItemObject slot in slots)
        {
            if (slot != null && slot.item.type == Item.Type.Weapon) return slot;
        }
        return null;
    }

}
