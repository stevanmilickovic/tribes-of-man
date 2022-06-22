using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Linq;

public class DisplayInventory : MonoBehaviour
{

    private static DisplayInventory singleton;

    public static DisplayInventory Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(DisplayInventory)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private MouseItem mouseItem = new MouseItem();

    private Inventory inventory;
    private Equipment clothes;
    private Equipment tools;
    public GameObject slot;
    private GameObject[] slots = new GameObject[14];
    public int numberOfAwaitedResponses = 0;

    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEMS;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEMS;

    private void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        CreateDisplay();
    }

    public void UpdateInventory()
    {
        if (numberOfAwaitedResponses <= 1)
        {
            inventory = PlayerManager.Singleton.myPlayer.inventory;
            for (int i = 0; i < 9; i++)
            {
                UpdateSlotDisplay(slots[i], inventory.slots[i]);
            }
            clothes = PlayerManager.Singleton.myPlayer.clothes;
            for (int i = 9; i < 12; i++)
            {
                UpdateSlotDisplay(slots[i], clothes.slots[i-9]);
            }
            tools = PlayerManager.Singleton.myPlayer.tools;
            for (int i = 12; i < 14; i++)
            {
                UpdateSlotDisplay(slots[i], tools.slots[i-12]);
            }
        }
        numberOfAwaitedResponses--;
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverSlot = obj;
    }
    public void OnExit(GameObject obj)
    {
        mouseItem.hoverSlot = null;
    }
    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        ItemObject itemObject = GetItem(obj);
        if (itemObject != null)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = itemObject.item.sprite;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseObject;
        mouseItem.slot = obj;
    }
    public void OnDragEnd(GameObject obj)
    {
        Tile tile = GetTile();

        if (mouseItem.hoverSlot)
        {
            TemporarilySwapSlots(mouseItem.slot, mouseItem.hoverSlot);
        }
        else if (tile != null && tile.itemObject != null)
        {
            AttemptCraftItems(mouseItem.slot, tile);
        }
        else if (tile != null && tile.structureObject != null)
        {
            AttemptBuildItems(mouseItem.slot, tile);
        }
        else
        {
            TemporarilyDropItem(mouseItem.slot);
        }
        Destroy(mouseItem.obj);
        mouseItem.slot = null;
        PlayerManager.Singleton.myPlayer.CheckEquipment();
    }
    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void TemporarilySwapSlots(GameObject slot1, GameObject slot2)
    {
        ItemObject item1 = GetItem(slot1);
        ItemObject item2 = GetItem(slot2);

        int i1 = GetIndex(slot1);
        int i2 = GetIndex(slot2);

        Inventory inventory1 = GetInventory(i1);
        Inventory inventory2 = GetInventory(i2);

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
            return;

        inventory1.slots[GetSlotNumber(i1)] = item2;
        inventory2.slots[GetSlotNumber(i2)] = item1;

        UpdateSlotDisplay(slot1, item2);
        UpdateSlotDisplay(slot2, item1);

        PlayerManager.Singleton.SwapItems(GetIndex(mouseItem.slot), GetIndex(mouseItem.hoverSlot));
    }

    public void TemporarilyDropItem(GameObject slot)
    {
        int i = GetIndex(slot);
        GetInventory(i).slots[GetSlotNumber(i)] = null;
        UpdateSlotDisplay(slot, null);
        PlayerManager.Singleton.DropItem(GetIndex(slot));
    }

    public void AttemptCraftItems(GameObject slot, Tile tile)
    {
        int i = GetIndex(slot);
        Crafting.Singleton.Craft(GetInventory(i), GetSlotNumber(i), tile.x, tile.y);
    }

    public void AttemptBuildItems(GameObject slot, Tile tile)
    {
        int i = GetIndex(slot);
        bool buildingSuccessful = Building.Build(GetItem(slot), tile);
        if (buildingSuccessful)
        {
            GetInventory(i).ReduceSlotAmount(GetSlotNumber(i));
            PlayerManager.Singleton.Build(i, tile.x, tile.y);
        }
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < 9; i++)
        {
            var obj = Instantiate(slot, Vector2.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            slots[i] = obj;
        }

        for (int i = 9; i < 12; i++)
        {
            var obj = Instantiate(slot, Vector2.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            slots[i] = obj;
        }

        for (int i = 12; i < 14; i++)
        {
            var obj = Instantiate(slot, Vector2.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            slots[i] = obj;
        }
    }

    public void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }

    public Inventory GetInventory(int i)
    {
        if (i < 9)
            return inventory;
        else if (i < 12)
            return clothes;
        else
            return tools;
    }

    public Equipment GetEquipment(int i)
    {
        Debug.Log("Getting equipment");
        if (i < 12)
            return clothes;
        else
            return tools;
    }

    public ItemObject GetItem(GameObject obj)
    {
        int i = GetIndex(obj);
        return GetInventory(i).slots[GetSlotNumber(i)];
    }

    public int GetSlotNumber(int i) //Returns exact slot number of needed inventory or equipment
    {
        if (i < 9)
            return i;
        else if (i < 12)
            return i-9;
        else
            return i-12;
    }

    public int GetIndex(GameObject slot) //Returns index of object in main slots[] array
    {
        return Array.IndexOf(slots, slot);
    }

    public Tile GetTile()
    {
        Vector3 mousePosition = PlayerManager.Singleton.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        int tileX = (int)(mousePosition.x) % 50;
        int tileY = (int)(mousePosition.y) % 50;
        Tile tile = null;
        if (tileX >= 0 && tileY >= 0)
            tile = MapManager.Singleton.tiles[(tileX, tileY)];

        return tile;
    }

    public void UpdateSlotDisplay(GameObject slot, ItemObject item)
    {
        if (item != null)
        {
            slot.GetComponent<Image>().color = Color.white;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = (item.amount == 1) ? string.Empty : item.amount.ToString();
            slot.GetComponent<Image>().sprite = item.item.sprite;
        }
        else
        {
            slot.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            slot.GetComponent<Image>().sprite = null;
            slot.GetComponent<Image>().color = Color.clear;
        }
    }

}

public class MouseItem
{
    public GameObject obj;
    public GameObject slot;
    public GameObject hoverSlot;
}
