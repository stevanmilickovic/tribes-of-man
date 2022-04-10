using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    public readonly int id;
    public readonly int slotNumber;
    public ItemObject[] slots;

    public Inventory(int _id, int _slotNumber)
    {
        id = _id;
        slotNumber = _slotNumber;
        slots = new ItemObject[slotNumber];
    }

    public void Test()
    {
        slots[0] = new ItemObject(ItemManager.Singleton.items[0], 1);
        slots[1] = new ItemObject(ItemManager.Singleton.items[0], 1);
        slots[2] = new ItemObject(ItemManager.Singleton.items[1], 2);
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

    public void ReduceSlotAmount(int slot)
    {
        ItemObject itemObject = slots[slot];
        itemObject.amount -= 1;
        
        if(itemObject.amount <= 0)
        {
            slots[slot] = null;
        }
    }

}
