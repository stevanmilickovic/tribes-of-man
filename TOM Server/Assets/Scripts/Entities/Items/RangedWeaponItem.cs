using UnityEngine;

public class RangedWeaponItem : Item
{
    public int damage;

    public RangedWeaponItem(int _id, string _name, bool _stackable, int _damage) : base(Type.Weapon, _id, _name, _stackable)
    {
        damage = _damage;
    }

}
