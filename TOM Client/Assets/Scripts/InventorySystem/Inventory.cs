using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    public readonly int id;
    public readonly int slotNumber;
    public ItemObject[] slots;

    public Inventory(int _slotNumber)
    {
        slotNumber = _slotNumber;
        slots = new ItemObject[slotNumber];
    }

    public Inventory(ItemObject[] _slots)
    {
        slots = _slots;
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

}
