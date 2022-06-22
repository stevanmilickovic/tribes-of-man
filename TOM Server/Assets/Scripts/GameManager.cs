using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        ItemManager.Singleton.InitializeItems();
        Crafting.InitializeRecipes();
        StructureManager.Singleton.InitializeStructures();
        Building.InitializeRecipes();
        MapManager.Singleton.InitializeMap();
    }
}
