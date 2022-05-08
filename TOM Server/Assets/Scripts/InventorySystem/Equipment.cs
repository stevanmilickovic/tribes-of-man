using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Equipment : Inventory
{
    public Equipment(int _id, int _slotNumber, Item.Type[] _itemTypes) : base(_id, _slotNumber, _itemTypes) { }

    public new void Test()
    {
        slots[0] = new ItemObject(ItemManager.Singleton.items[0], 1);
    }

}
