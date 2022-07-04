using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    private static StructureManager singleton;

    public static StructureManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(StructureManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Dictionary<int, Structure> structures;
    public Dictionary<string, Structure> structuresByName;
    public GameObject largeStructurePrefab;
    public GameObject mediumStructurePrafab;
    public GameObject smallStructurePrefab;
    public Dictionary<(int, int), GameObject> spawnedStructures;

    private void Awake()
    {
        structures = new Dictionary<int, Structure>();
        structuresByName = new Dictionary<string, Structure>();
        spawnedStructures = new Dictionary<(int, int), GameObject>();
        Singleton = this;
    }

    public StructureObject SpawnStructure(Tile tile, Structure structure)
    {
        int x = tile.x;
        int y = tile.y;

        GameObject newStructure = InstantiateStructure(structure);
        newStructure.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
        spawnedStructures.Add((x, y), newStructure);

        StructureObject structureObject = newStructure.AddComponent<StructureObject>();
        structureObject.structure = structure;
        structureObject.collapsed = false;
        structureObject.health = structure.maxHealth;
        structureObject.collapsedHealth = structure.maxCollapsedHealth;
        structureObject.tile = tile;
        return structureObject;
    }

    public StructureObject SpawnCollapsedStructure(Tile tile, Structure structure)
    {
        int x = tile.x;
        int y = tile.y;

        GameObject newStructure = InstantiateStructure(structure);
        newStructure.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
        spawnedStructures.Add((x, y), newStructure);

        StructureObject structureObject = newStructure.AddComponent<StructureObject>();
        structureObject.structure = structure;
        structureObject.collapsed = true;
        structureObject.health = 0;
        structureObject.collapsedHealth = 1;
        structureObject.tile = tile;
        return structureObject;
    }

    private GameObject InstantiateStructure(Structure structure)
    {
        if (structure.sizeType == Structure.SizeType.Large)
            return Instantiate(largeStructurePrefab);
        if (structure.sizeType == Structure.SizeType.Medium)
            return Instantiate(mediumStructurePrafab);
        if (structure.sizeType == Structure.SizeType.Small)
            return Instantiate(smallStructurePrefab);
        return null;
    }

    public void DestroyStructure(Tile tile)
    {
        int x = tile.x;
        int y = tile.y;
        GameObject structureToDestroy = spawnedStructures[(x, y)];
        spawnedStructures.Remove((x, y));
        Destroy(structureToDestroy);
    }

    public void InitializeStructures()
    {
        AddStructure(0, Structure.Type.Natural, Structure.SizeType.Medium, false, "Tree", "wood", 7, 10, 30);
        AddStructure(1, Structure.Type.Manmade, Structure.SizeType.Large, true, "Wood Wall", "wood", 5, 50, 100);
        AddStructure(2, Structure.Type.Manmade, Structure.SizeType.Large, false, "Frame", "stick", 1, 1, 1);
        AddStructure(3, Structure.Type.Natural, Structure.SizeType.Small, false, "Bush", "stick", 1, 5, 0);
        AddStructure(4, Structure.Type.Natural, Structure.SizeType.Small, false, "Berries", "stick", 1, 5, 0);
    }

    private void AddStructure(int id, Structure.Type type, Structure.SizeType sizeType, bool collapsable, string name, string itemName, int itemAmount, int maxHealth, int maxBrokenHealth)
    {
        structures.Add(id, new Structure(type, sizeType, collapsable, id, name, ItemManager.Singleton.itemsByName[itemName], itemAmount, maxHealth, maxBrokenHealth));
        structuresByName.Add(name, structures[id]);
    }

}
