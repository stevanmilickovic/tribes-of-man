using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{

    int damage;

    public WeaponItem(int _id, string _name, bool _stackable, Sprite _sprite, int _damage) : base(Type.Weapon, _id, _name, _stackable, _sprite)
    {
        damage = _damage;
    }
}
