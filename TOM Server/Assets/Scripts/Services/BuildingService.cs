using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingService
{
    public static Dictionary<(Item, Structure), Structure> buildingRecipes = new Dictionary<(Item, Structure), Structure>();

    public static void InitializeRecipes()
    {
        AddRecipe("Frame", "wood", "Wood Wall");
    }

    private static void AddRecipe(string originalStructureName, string itemName, string resultStructureName)
    {
        Item item = ItemService.itemsByName[itemName];
        Structure originalStructure = StructureManager.Singleton.structuresByName[originalStructureName];
        Structure resultStructure = StructureManager.Singleton.structuresByName[resultStructureName];
        buildingRecipes.Add((item, originalStructure), resultStructure);
    }

    public static bool Build(ItemObject itemObject, Tile tile)
    {
        if (!RecipeExists(itemObject.item, tile.structureObject.structure)) return false;

        Structure structure = GetRecipe(itemObject, tile.structureObject);
        tile.DestroyStructure();
        tile.SpawnStructure(structure);
        MapSend.SendTileMessage(tile);

        return true;
    }

    private static bool RecipeExists(Item item, Structure structure)
    {
        return buildingRecipes.ContainsKey((item, structure));
    }

    private static Structure GetRecipe(ItemObject itemObject, StructureObject structureObject)
    {
        return buildingRecipes[(itemObject.item, structureObject.structure)];
    }
}
