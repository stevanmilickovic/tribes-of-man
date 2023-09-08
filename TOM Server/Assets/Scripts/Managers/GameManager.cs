using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        ItemService.InitializeItems();
        CraftingService.InitializeRecipes();
        StructureManager.Singleton.InitializeStructures();
        BuildingService.InitializeRecipes();
        MapManager.Singleton.InitializeMap();

        //InvokeRepeating("CallDropPlayersHunger", 2f, 12f);
        //InvokeRepeating("CallDropMalnourishedPlayersHealth", 2f, 2.4f);
    }

    /**
    private void CallDropPlayersHunger()
    {
        PlayerManager.Singleton.DropAllPlayersHunger();
    }

    private void CallDropMalnourishedPlayersHealth()
    {
        PlayerManager.Singleton.DropMalnourishedPlayersHealth();
    }*/
}
