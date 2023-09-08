using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainTypes
{
    Water = 1,
    Sand,
    Grass,
    Stone
}
public class Tile
{

    public TerrainTypes type;

    public int x;
    public int y;
    public ItemObject itemObject;
    public StructureObject structureObject;

    public Chunk chunk;

    public Tile(int _x, int _y, TerrainTypes _type)
    {
        x = _x;
        y = _y;
        type = _type;
    }

    public void SpawnStructure(Structure structure)
    {
        if (!structure.collapsable)
        {
            structureObject = StructureManager.Singleton.SpawnStructure(this, structure);
            return;
        }
        else
        {
            structureObject = StructureManager.Singleton.SpawnCollapsedStructure(this, structure);
            return;
        }
    }

    public void CollapseStructure()
    {
        structureObject.collapsed = true;
    }

    public void DestroyStructure()
    {
        if (structureObject == null) return;

        structureObject = null;
        StructureManager.Singleton.DestroyStructure(this);
    }

}
