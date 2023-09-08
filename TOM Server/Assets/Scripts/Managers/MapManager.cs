using RiptideNetworking;
using UnityEngine;
using System;

public class MapManager : MonoBehaviour
{

    private static MapManager singleton;

    public static MapManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(MapManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public Map map;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
    }

    public void InitializeMap()
    {
        map = new Map(mapWidth, mapHeight, Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset));
        map.tiles[1, 1].SpawnStructure(StructureManager.Singleton.structures[0]); //TO REMOVE LATER
        map.tiles[2, 1].SpawnStructure(StructureManager.Singleton.structures[1]);
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
    }

    public Tile DropItem(int x, int y, ItemObject itemObject, int amount)
    {
        Tile tile = map.tiles[x, y];

        if(TryToTakeItem(tile, itemObject, amount))
        {
            return tile;
        }
        
        for (int _y = y + 1; _y >= y - 1; _y--)
        {
            for (int _x = x - 1; _x <= x + 1; _x++)
            {
                if (_x >= 0 && _y >= 0 && _x < map.tiles.GetLength(0) && _y < map.tiles.GetLength(1))
                {
                    if (TryToTakeItem(map.tiles[_x, _y], itemObject, amount))
                        return (map.tiles[_x, _y]);
                }
            }
        }

        return null;
    }

    private bool TryToTakeItem(Tile tile, ItemObject itemObject, int amount)
    {
        if (tile.itemObject == null)
        {
            tile.itemObject = new ItemObject(itemObject.item, amount);
            return true;
        }
        if (tile.itemObject.item == itemObject.item && itemObject.item.stackable)
        {
            tile.itemObject.amount += amount;
            return true;
        }
        return false;
    }

}