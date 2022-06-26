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

    private void Awake()
    {
        structures = new Dictionary<int, Structure>();
        structuresByName = new Dictionary<string, Structure>();
        Singleton = this;
    }

    public void InitializeStructures()
    {
        AddStructure(0, Structure.Type.Natural, Structure.SizeType.Medium, "Tree", "wood", 10, 30, "tree", "rubble");
        AddStructure(1, Structure.Type.Manmade, Structure.SizeType.Large, "Wood Wall", "wood", 50, 100, "wood_wall", "rubble");
        AddStructure(2, Structure.Type.Manmade, Structure.SizeType.Large, "Frame", "stick", 1, 1, "frame", "rubble");
        AddStructure(3, Structure.Type.Natural, Structure.SizeType.Small, "Bush", "stick", 5, 0, "bush", "rubble");
        AddStructure(4, Structure.Type.Natural, Structure.SizeType.Small, "Berries", "stick", 5, 0, "bush", "rubble");
    }

    private void AddStructure(int id, Structure.Type type, Structure.SizeType sizeType, string name, string itemName, int maxHealth, int maxBrokenHealth, string spriteName, string collapsedSpriteName)
    {
        structures.Add(id, new Structure(type, sizeType, id, name, ItemManager.Singleton.itemsByName[itemName], maxHealth, maxBrokenHealth, GetSprite(spriteName), GetSprite(collapsedSpriteName)));
        structuresByName.Add(name, structures[id]);
    }

    public Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }

}
