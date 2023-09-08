using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class PlayerUtil
{
    public static int CalculateDamage(Player player)
    {
        int damage = 10;
        Equipment tools = player.tools;
        foreach (ItemObject slot in tools.slots)
        {
            if (slot != null && slot.item.type == Item.Type.Weapon)
            {
                WeaponItem weapon = (WeaponItem)slot.item;
                damage += weapon.damage;
            }
        }

        return damage;
    }
}
