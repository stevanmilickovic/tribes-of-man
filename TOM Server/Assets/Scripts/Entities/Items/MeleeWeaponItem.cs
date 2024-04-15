
public class MeleeWeaponItem : Item
{

    public int damage;
    public MeleeAttackTypes meleeAttackType;
    public float dash;

    public MeleeWeaponItem(int _id, string _name, bool _stackable, int _damage, MeleeAttackTypes _meleeAttackType, float _dash) : base(Type.Weapon, _id, _name, _stackable)
    {
        damage = _damage;
        meleeAttackType = _meleeAttackType;
        dash = _dash;
    }
}
