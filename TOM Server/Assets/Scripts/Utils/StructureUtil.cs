using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class StructureUtil
{

    public static int CalculateBuildingProgress(Player player, StructureObject structure)
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

}
