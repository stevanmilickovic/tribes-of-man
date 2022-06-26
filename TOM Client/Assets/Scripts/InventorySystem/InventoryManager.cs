using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class InventoryManager
{

    public static Dictionary<ushort, InventoryState> inventoryStates = new Dictionary<ushort, InventoryState>();
    public static InventoryState lastAddedState;

    public static void MoveItemsAndParseState(ushort tick, int absoluteSlot1, int amount, int absoluteSlot2, bool drop)
    {
        MoveItems(absoluteSlot1, amount, absoluteSlot2, drop);
        CreateInventoryState(tick, absoluteSlot1, amount, absoluteSlot2, drop, lastAddedState);
        lastAddedState = inventoryStates[tick];
    }

    public static void MoveItems(int absoluteSlot1, int amount, int absoluteSlot2, bool drop)
    {

        int slot1 = InventoryUtil.GetSlotNumber(absoluteSlot1);
        int slot2 = InventoryUtil.GetSlotNumber(absoluteSlot2);

        Inventory inventory1 = InventoryUtil.GetInventory(absoluteSlot1);
        Inventory inventory2 = InventoryUtil.GetInventory(absoluteSlot2);

        ItemObject item1 = inventory1.slots[slot1];
        ItemObject item2 = inventory2.slots[slot2];

        if (drop)
        {
            if (amount == 0)
                inventory1.slots[slot1] = null;
            else
                inventory1.ReduceSlotAmountByNumber(slot1, amount);
            return;
        }

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
        {
            DisplayInventory.Singleton.UpdateInventory();
            return;
        }

        if (amount == 0)
        {
            if (item1 != null && item2 != null && item1.item == item2.item && item2.item.stackable)
            {
                item2.amount += item1.amount;
                inventory1.slots[slot1] = null;
            }
            else
            {
                inventory1.slots[slot1] = item2;
                inventory2.slots[slot2] = item1;
            }
        }
        else if (amount == 1)
        {
            if (item2 == null)
            {
                inventory1.ReduceSlotAmount(slot1);
                inventory2.slots[slot2] = new ItemObject(item1.item, 1);
            }
            else if (item2.item == item1.item && item2.item.stackable)
            {
                inventory1.ReduceSlotAmount(slot1);
                item2.amount += 1;
            }
        }
        else if (amount == (int)Mathf.Floor(item1.amount / 2))
        {
            if (item2 == null)
            {
                inventory2.slots[slot2] = new ItemObject(item1.item, (int)Mathf.Floor(item1.amount / 2));
                inventory1.ReduceSlotAmountByNumber(slot1, (int)Mathf.Floor(item1.amount / 2));
            }
            else if (item2.item == item1.item && item2.item.stackable)
            {
                item2.amount += (int)Mathf.Floor(item1.amount / 2);
                inventory1.ReduceSlotAmountByNumber(slot1, (int)Mathf.Floor(item1.amount / 2));
            }
        }
    }

    public static void CreateInventoryState(ushort tick, int slot1, int amount, int slot2, bool drop, InventoryState previousInventoryState)
    {
        List<ItemState> items = new List<ItemState>();
        foreach (ItemObject itemObject in PlayerManager.Singleton.myPlayer.inventory.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }
        foreach (ItemObject itemObject in PlayerManager.Singleton.myPlayer.clothes.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }
        foreach (ItemObject itemObject in PlayerManager.Singleton.myPlayer.tools.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }

        InventoryState newState = new InventoryState(tick ,items, slot1, amount, slot2, drop, previousInventoryState);
        if (lastAddedState != null) lastAddedState.nextInventoryState = newState;
        if (inventoryStates.ContainsKey(tick)) inventoryStates.Remove(tick);
        inventoryStates.Add(tick, newState);
    }

    public static void UpdateInventoryIfNecessary(ushort tick, Inventory inventory, Equipment clothes, Equipment tools)
    {
        MyPlayer myPlayer = PlayerManager.Singleton.myPlayer;
        if (!inventoryStates.ContainsKey(tick) && lastAddedState == null || tick == 0)
        {
            myPlayer.inventory = inventory;
            myPlayer.clothes = clothes;
            myPlayer.tools = tools;
            DisplayInventory.Singleton.UpdateInventory();
            return;
        }
        if (IsValidState(tick, inventory, clothes, tools))
        {
            InventoryState state = inventoryStates[tick];
            if (state == lastAddedState) lastAddedState = null;
            inventoryStates.Remove(tick);
            return;
        }
        myPlayer.inventory = inventory;
        myPlayer.clothes = clothes;
        myPlayer.tools = tools;
        if(inventoryStates[tick].nextInventoryState != null)
            Reconceliate(inventoryStates[tick].nextInventoryState, inventoryStates[tick]);

        if (inventoryStates[tick] == lastAddedState) lastAddedState = null;
        inventoryStates.Remove(tick);

        DisplayInventory.Singleton.UpdateInventory();
    }

    public static void Reconceliate(InventoryState state, InventoryState previousState)
    {
        MoveItems(state.slot1, state.amount, state.slot2, state.drop);
        CreateInventoryState(state.tick, state.slot1, state.amount, state.slot2, state.drop, previousState);
        if (state.nextInventoryState != null) Reconceliate(state.nextInventoryState, inventoryStates[state.tick]);
        else lastAddedState = inventoryStates[state.tick];
    }

    public static bool IsValidState(ushort tick, Inventory inventory, Inventory clothes, Inventory tools)
    {
        List<ItemState> items = new List<ItemState>();
        foreach (ItemObject itemObject in inventory.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }
        foreach (ItemObject itemObject in clothes.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }
        foreach (ItemObject itemObject in tools.slots)
        {
            items.Add(new ItemState(itemObject != null ? itemObject.item.id : -1, itemObject != null ? itemObject.amount : 0));
        }
        InventoryState state = inventoryStates[tick];

        for (int i = 0; i < 13; i++)
        {
            if (items[i].id != state.items[i].id || items[i].amount != state.items[i].amount) return false;
        }
        return true;
    }

}

public class InventoryState
{
    public ushort tick;
    public List<ItemState> items;
    public int slot1;
    public int amount;
    public int slot2;
    public bool drop;
    public bool pickup;
    public InventoryState previousInventoryState;
    public InventoryState nextInventoryState;

    public InventoryState(ushort _tick, List<ItemState> _items, int _slot1, int _amount, int _slot2, bool _drop, InventoryState _previousInventoryState)
    {
        tick = _tick;
        items = _items;
        slot1 = _slot1;
        amount = _amount;
        slot2 = _slot2;
        drop = _drop;
        previousInventoryState = _previousInventoryState;
    }
}

public class ItemState
{
    public int id;
    public int amount;

    public ItemState(int _id, int _amount)
    {
        id = _id;
        amount = _amount;
    }
}
