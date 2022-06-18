using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    public void ExecuteHit(Vector2 direction, Player player)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (player.equipmentType == Player.EquipmentType.Armed)
            {
                ExecuteAttack(collider, player);
            }
        }
    }

    private void ExecuteAttack(Collider2D collider, Player player)
    {
        if (collider.gameObject.tag == "Player" && collider.gameObject != transform.parent.transform.parent.gameObject)
        {
            Player hitPlayer = collider.gameObject.GetComponent<Player>();
            hitPlayer.health -= CalculateDamage(player);
        }
        if (collider.gameObject.tag == "Structure")
        {
            StructureObject hitStructure = collider.gameObject.GetComponent<StructureObject>();
            hitStructure.TakeDamage(CalculateDamage(player));
        }
    }

    private int CalculateDamage(Player player)
    {
        int damage = 10;
        Equipment tools = player.tools;
        foreach (ItemObject slot in tools.slots)
        {
            if(slot != null && slot.item.type == Item.Type.Weapon)
            {
                WeaponItem weapon = (WeaponItem)slot.item;
                damage += weapon.damage;
            }
        }

        return damage;
    }

}
