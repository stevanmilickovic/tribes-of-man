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

    public Inventory(int _id, int _slotNumber)
    {
        id = _id;
        slotNumber = _slotNumber;
        slots = new ItemObject[slotNumber];
    }

    public Inventory(int _id, int _slotNumber, Item.Type[] _itemTypes)
    {
        id = _id;
        slotNumber = _slotNumber;
        slots = new ItemObject[slotNumber];
        itemTypes = _itemTypes;
    }

    public void Test()
    {
        slots[0] = new ItemObject(ItemService.items[0], 1);
        slots[1] = new ItemObject(ItemService.items[0], 1);
        slots[2] = new ItemObject(ItemService.items[1], 2);
        slots[3] = new ItemObject(ItemService.items[2], 1);
        slots[4] = new ItemObject(ItemService.items[3], 2);
        slots[5] = new ItemObject(ItemService.items[4], 1);
    }

    public bool Add(ItemObject itemObject)
    {
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
        
        if(itemObject.amount <= 0)
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

    public void DropItem(int x, int y, int slotNumber, int amount)
    {
        if (slots[slotNumber] == null)
            return;

        if (MapManager.Singleton.DropItem(x, y, slots[slotNumber], amount) != null)
        {
            slots[slotNumber] = null;
        }
    }

    public int GetSlotThatContainsItem(Item item)
    {
        for (int i = 0; i < 9; i++)
        {
            if (slots[i] != null && slots[i].item == item) return i;
        }
        return -1;
    }

}
