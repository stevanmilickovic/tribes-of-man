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

    private void Awake()
    {
        structures = new Dictionary<int, Structure>();
        Singleton = this;
    }

    public void InitializeStructures()
    {
        AddStructure(0, Structure.Type.Natural, "Tree", "wood", 10, 0, "tree");
        AddStructure(1, Structure.Type.Manmade, "Wood Wall", "wood", 50, 100, "wood_wall");
    }

    private void AddStructure(int id, Structure.Type type, string name, string itemName, int maxHealth, int maxBrokenHealth, string spriteName)
    {
        structures.Add(id, new Structure(type, id, name, ItemManager.Singleton.itemsByName[itemName], maxHealth, maxBrokenHealth, GetSprite(spriteName)));
        Debug.Log($"Sprite is {GetSprite(spriteName)}");
    }

    public Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/{name}");
    }

}
