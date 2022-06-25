using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory
{

    public readonly int id;
    public readonly int slotNumber;
    public ItemObject[] slots;
    public Item.Type[] itemTypes;

    public Inventory(int _slotNumber)
    {
        slotNumber = _slotNumber;
        slots = new ItemObject[slotNumber];
    }

    public Inventory(ItemObject[] _slots, Item.Type[] _itemTypes)
    {
        slots = _slots;
        itemTypes = _itemTypes;
    }

    public Inventory(ItemObject[] _slots)
    {
        slots = _slots;
    }

    public bool Add(ItemObject itemObject)
    {

        if (!CanAddItem(itemObject))
            return false;

        if (itemObject.item.stackable)
        {
            for (int i = 0; i < slotNumber; i++)
            {
                if (slots[i] != null)
                {
                    if (slots[i].item == itemObject.item)
                    {
                        slots[i].amount += itemObject.amount;
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < slotNumber; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = itemObject;
                return true;
            }
        }

        return false;
    }

    public bool CanAddItem(ItemObject itemObject)
    {
        if (itemObject == null)
            return true;
        if (itemTypes == null)
            return true;
        return Array.IndexOf(itemTypes, itemObject.item.type) > -1;
    }

    public void ReduceSlotAmount(int slot)
    {
        ItemObject itemObject = slots[slot];
        itemObject.amount -= 1;

        if (itemObject.amount <= 0)
        {
            slots[slot] = null;
        }
    }

    public void ReduceSlotAmountByNumber(int slot, int amount)
    {
        ItemObject itemObject = slots[slot];
        itemObject.amount -= amount;

        if (itemObject.amount <= 0)
        {
            slots[slot] = null;
        }
    }
}
