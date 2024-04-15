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
            if (attack.remainingTicks == 0) ExecuteAttack();
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
        MeleeWeaponItem weapon = (MeleeWeaponItem)weaponObject.item;

        attack = new Attack(weapon, direction, CHARGE_TIME_IN_TICKS);
        isChargingAttack = true;
    }

    public void ExecuteAttack()
    {
        isChargingAttack = false;

        if (attack.item is MeleeWeaponItem)
        {
            ExecuteMeleeAttack();
        }
        else if (attack.item is RangedWeaponItem)
        {
            ExecuteRangedAttack();
        }
    }

    public void ExecuteRangedAttack()
    {
        GameObject projectile = Instantiate(PlayerManager.Singleton.projectilePrefab);
        projectile.transform.right = attack.direction;
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        RangedWeaponItem item = (RangedWeaponItem)tools.GetMainWeapon().item;
        projectileScript.ConfigureProjectile(10, item.damage, item.projectileSprite, gameObject);
    }

    public void ExecuteMeleeAttack()
    {
        Dash();
        PlayerAnimator playerAnimator = gameObject.GetComponent<PlayerAnimator>();
        playerAnimator.ExecuteMeleeAttack(((MeleeWeaponItem)attack.item).meleeAttackType, attack.direction);
    }

    private void Dash()
    {
        transform.position = transform.position + new Vector3(attack.direction.x, attack.direction.y, 0) * ((MeleeWeaponItem)attack.item).dash;
    }
}

public struct Attack
{

    public Attack(Item item, Vector2 direction, int remainingTicks)
    {
        this.item = item;
        this.direction = direction;
        this.remainingTicks = remainingTicks;
    }

    public Item item;
    public Vector2 direction;
    public int remainingTicks;
}
