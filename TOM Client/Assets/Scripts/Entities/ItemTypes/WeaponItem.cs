using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{

    public int damage;
    public MeleeAttackTypes MeleeAttackType;
    public float dash;

    public WeaponItem(int _id, string _name, bool _stackable, Sprite _sprite, int _damage, MeleeAttackTypes _meleeAttackType, float _dash) : base(Type.Weapon, _id, _name, _stackable, _sprite)
    {
        damage = _damage;
        MeleeAttackType = _meleeAttackType;
        dash = _dash;
    }
}
