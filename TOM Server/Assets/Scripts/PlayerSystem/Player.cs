using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum EquipmentType
    {
        Unarmed,
        Armed,
        HoldingTool,
        Other
    }

    public int id;
    public ushort currectClientId;
    public string playerName;
    public int health;
    public GameObject pivot;
    public Hit hit;
    public Inventory inventory;
    public Equipment clothes;
    public Equipment tools;
    public EquipmentType equipmentType;

    public Player(int _id, ushort _currectClientId, string _playerName)
    {
        id = _id;
        currectClientId = _currectClientId;
        playerName = _playerName;
    }

    public void Attack(Vector2 direction)
    {
        hit.gameObject.SetActive(true);
        hit.ExecuteHit(direction, this);
    }

    public void CheckEquipment()
    {
        equipmentType = EquipmentType.Unarmed;
        foreach (ItemObject slot in tools.slots)
        {
            if (slot != null && slot.item.type == Item.Type.Weapon)
            {
                equipmentType = EquipmentType.Armed;
                break;
            }
            if (slot != null && slot.item.type == Item.Type.Tool)
            {
                equipmentType = EquipmentType.HoldingTool;
            }
        }
    }
}
