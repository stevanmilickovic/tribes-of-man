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
    public GameObject slot;
    private GameObject[] slots = new GameObject[9];
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
                ItemObject obj = inventory.slots[i];
                UpdateSlotDisplay(slots[i], obj);
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
        ItemObject itemObject = inventory.slots[GetIndex(obj)];
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
        Vector3 mousePosition = PlayerManager.Singleton.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 chunk = new Vector2((int)(mousePosition.x / 50), (int)(mousePosition.y / 50));
        int tileX = (int)(mousePosition.x) % 50;
        int tileY = (int)(mousePosition.y) % 50;
        Tile tile = MapManager.Singleton.tiles[(tileX, tileY)];

        if (mouseItem.hoverSlot)
        {
            TemporarilySwapSlots(mouseItem.slot, mouseItem.hoverSlot);
            PlayerManager.Singleton.SwapItems(GetIndex(mouseItem.slot), GetIndex(mouseItem.hoverSlot));
        }
        else if(tile.itemObject != null)
        {
            AttemptCraftItems(mouseItem.slot, tile);
        }
        else
        {
            TemporarilyDropItem(mouseItem.slot);
            PlayerManager.Singleton.DropItem(GetIndex(mouseItem.slot));
        }
        Destroy(mouseItem.obj);
        mouseItem.slot = null;
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
        ItemObject obj1 = inventory.slots[GetIndex(slot1)];
        ItemObject obj2 = inventory.slots[GetIndex(slot2)];

        inventory.slots[GetIndex(slot1)] = obj2;
        inventory.slots[GetIndex(slot2)] = obj1;

        UpdateSlotDisplay(slot1, obj2);

        UpdateSlotDisplay(slot2, obj1);
    }

    public void TemporarilyDropItem(GameObject slot)
    {
        ItemObject obj = inventory.slots[GetIndex(slot)];
        inventory.slots[GetIndex(slot)] = null;
        UpdateSlotDisplay(slot, null);
    }

    public void AttemptCraftItems(GameObject slot, Tile tile)
    {
        Crafting.Singleton.Craft(GetIndex(slot), tile.x, tile.y);
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

    public int GetIndex(GameObject slot)
    {
        return Array.IndexOf(slots, slot);
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
