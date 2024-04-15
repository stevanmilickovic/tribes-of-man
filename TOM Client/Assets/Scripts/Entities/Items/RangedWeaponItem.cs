using UnityEngine;

public class RangedWeaponItem : Item
{
    public int damage;
    public Sprite projectileSprite;

    public RangedWeaponItem(int _id, string _name, bool _stackable, Sprite _sprite, int _damage, Sprite _projectileSprite) : base(Type.Weapon, _id, _name, _stackable, _sprite)
    {
        damage = _damage;
        projectileSprite = _projectileSprite;
    }

}
