using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        ItemManager.Singleton.InitializeItems();
        Crafting.Singleton.InitializeRecipes();
        StructureManager.Singleton.InitializeStructures();
        MapManager.Singleton.InitializeMap();
    }
}
