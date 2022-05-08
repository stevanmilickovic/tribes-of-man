using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Equipment : Inventory
{

    public Equipment(ItemObject[] _slots, Item.Type[] _itemTypes) : base(_slots, _itemTypes) { }

}
