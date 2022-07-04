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
            if (player.equipmentType == Player.EquipmentType.HoldingTool)
            {
                ExecuteBuild(collider, player);
            }
        }
    }

    private void ExecuteBuild(Collider2D collider, Player player)
    {
        if (collider.gameObject.tag == "Structure")
        {
            StructureObject hitStructure = collider.gameObject.GetComponent<StructureObject>();
            hitStructure.MakeBuildingProgress(CalculateBuildingProgress(player, hitStructure));
            PlayerSend.SendInventoryMessage(player, player.currentClientId, 0);
        }
    }

    private int CalculateBuildingProgress(Player player, StructureObject structure)
    {
        int progress = 0;
        int slotWithItemNeeded = player.inventory.GetSlotThatContainsItem(structure.structure.item);
        if (slotWithItemNeeded != -1 && (structure.collapsed || structure.health < structure.structure.maxHealth))
        {
            progress = 20;
            player.inventory.ReduceSlotAmount(slotWithItemNeeded);
        }
        return progress;
    }

    private void ExecuteAttack(Collider2D collider, Player player)
    {
        if (collider.gameObject.tag == "Player" && collider.gameObject != transform.parent.transform.parent.gameObject)
        {
            Player hitPlayer = collider.gameObject.GetComponent<Player>();
            hitPlayer.TakeDamage(CalculateDamage(player));
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
