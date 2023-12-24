using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Player : MonoBehaviour
{

    public enum EquipmentType
    {
        Unarmed,
        Armed,
        HoldingTool,
        Other
    }

    private Interpolator interpolator;

    public int id;
    public string username;
    public Vector2 position;
    public Equipment clothes;
    public Equipment tools;

    public GameObject nameText;

    public GameObject leftArm;
    public GameObject rightArm;
    public EquipmentType equipmentType;

    public bool isChargingAttack = false;
    public static int CHARGE_TIME_IN_TICKS = 16;
    public Attack attack;

    public Player(int _id, string _username)
    {
        id = _id;
        username = _username;
        try
        {
            interpolator = gameObject.GetComponent<Interpolator>();
        }
        catch (Exception) { }
        equipmentType = EquipmentType.Unarmed;
    }

    private void FixedUpdate()
    {
        if (isChargingAttack)
        {
            attack.remainingTicks--;
            if (attack.remainingTicks == 0) ExecuteMeleeAttack();
        }
    }

    public void FillPlayerData(PlayerPacket packet)
    {
        username = packet.username;
        id = packet.id;
        position = packet.position;
        clothes = packet.clothes;
        tools = packet.tools;

        nameText = transform.GetChild(0).gameObject;
        leftArm = transform.GetChild(1).transform.GetChild(4).transform.GetChild(0).gameObject;
        rightArm = transform.GetChild(1).transform.GetChild(5).transform.GetChild(0).gameObject;
        nameText.GetComponent<TextMeshPro>().text = packet.username;
        CheckEquipment();
    }

    public void Move(ushort tick, Vector2 position)
    {
        this.position = position;
        interpolator = gameObject.GetComponent<Interpolator>();
        interpolator.NewUpdate(tick, position);
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

        if (tools.slots[0] != null)
        {
            leftArm.GetComponent<SpriteRenderer>().sprite = tools.slots[0].item.sprite;
        }
        else
        {
            leftArm.GetComponent<SpriteRenderer>().sprite = null;
        }
        if (tools.slots[1] != null)
        {
            rightArm.GetComponent<SpriteRenderer>().sprite = tools.slots[1].item.sprite;
        }
        else
        {
            rightArm.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void StartMeleeAttack(MeleeAttackTypes type, Vector2 direction, ushort tick)
    {
        ItemObject weaponObject = tools.GetMainWeapon();
        WeaponItem weapon = (WeaponItem)weaponObject.item;

        attack = new Attack(true, false, type, direction, CHARGE_TIME_IN_TICKS, weapon.dash);
        isChargingAttack = true;
    }

    public void ExecuteMeleeAttack()
    {
        Dash();
        PlayerAnimator playerAnimator = gameObject.GetComponent<PlayerAnimator>();
        playerAnimator.ExecuteMeleeAttack(attack.meleeType, attack.direction);
    }

    private void Dash()
    {
        transform.position = transform.position + new Vector3(attack.direction.x, attack.direction.y, 0) * attack.dash;
    }
}

public struct Attack
{

    public Attack(bool isMelee, bool isRanged, MeleeAttackTypes meleeType, Vector2 direction, int remainingTicks, float dash)
    {
        this.isMelee = isMelee;
        this.isRanged = isRanged;
        this.meleeType = meleeType;
        this.direction = direction;
        this.remainingTicks = remainingTicks;
        this.dash = dash;
    }

    public bool isMelee;
    public bool isRanged;
    public MeleeAttackTypes meleeType;
    public Vector2 direction;
    public int remainingTicks;
    public float dash;
}
