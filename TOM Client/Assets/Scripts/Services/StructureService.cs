using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StructureService
{

    public static Dictionary<int, Structure> structures = new Dictionary<int, Structure>();
    public static Dictionary<string, Structure> structuresByName = new Dictionary<string, Structure>();

    public static void InitializeStructures()
    {
        AddStructure(0, Structure.Type.Natural, Structure.SizeType.Medium, "Tree", "wood", 10, 30, "tree", "rubble");
        AddStructure(1, Structure.Type.Manmade, Structure.SizeType.Large, "Wood Wall", "wood", 50, 100, "wood_wall", "rubble");
        AddStructure(2, Structure.Type.Manmade, Structure.SizeType.Large, "Frame", "stick", 1, 1, "frame", "rubble");
        AddStructure(3, Structure.Type.Natural, Structure.SizeType.Small, "Bush", "stick", 5, 0, "bush", "rubble");
        AddStructure(4, Structure.Type.Natural, Structure.SizeType.Small, "Berries", "stick", 5, 0, "bush", "rubble");
    }

    private static void AddStructure(int id, Structure.Type type, Structure.SizeType sizeType, string name, string itemName, int maxHealth, int maxBrokenHealth, string spriteName, string collapsedSpriteName)
    {
        structures.Add(id, new Structure(type, sizeType, id, name, ItemService.itemsByName[itemName], maxHealth, maxBrokenHealth, GetSprite(spriteName), GetSprite(collapsedSpriteName)));
        structuresByName.Add(name, structures[id]);
    }

    public static Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }

}
