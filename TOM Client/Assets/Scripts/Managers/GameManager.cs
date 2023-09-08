using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        ItemService.InitializeItems();
        CraftingService.InitializeRecipes();
        StructureService.InitializeStructures();
        BuildingService.InitializeRecipes();
    }
}
