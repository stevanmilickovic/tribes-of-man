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

        InvokeRepeating("CallDropPlayersHealth", 2f, 12f);
        InvokeRepeating("CallDropMalnourishedPlayersHealth", 2f, 2.4f);
    }

    private void CallDropPlayersHealth()
    {
        PlayerManager.Singleton.DropAllPlayersHunger();
    }

    private void CallDropMalnourishedPlayersHealth()
    {
        PlayerManager.Singleton.DropMalnourishedPlayersHealth();
    }
}
