using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryUtil
{

    public static int GetSlotNumber(int i) //Returns exact slot number of needed inventory or equipment
    {
        if (i < 9)
            return i;
        else if (i < 12)
            return i - 9;
        else
            return i - 12;
    }

    public static Inventory GetInventory(int i)
    {
        if (i < 9)
            return PlayerManager.Singleton.myPlayer.inventory;
        else if (i < 12)
            return PlayerManager.Singleton.myPlayer.clothes;
        else
            return PlayerManager.Singleton.myPlayer.tools;
    }
}
