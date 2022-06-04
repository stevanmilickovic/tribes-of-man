using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{

    public int damage;

    public WeaponItem(int _id, string _name, bool _stackable, int _damage) : base(Type.Weapon, _id, _name, _stackable)
    {
        damage = _damage;
    }
}
