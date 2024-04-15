using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum TerrainTypes
    {
        Water = 1,
        Sand,
        Grass,
        Stone
    }

    public TerrainTypes type;

    public int x;
    public int y;
    public ItemObject itemObject;
    public StructureObject structureObject;

    public Chunk chunk;

    public int firstVert;

    public Tile(int _x, int _y, TerrainTypes _type)
    {
        x = _x;
        y = _y;
        type = _type;
    }

    public Tile(int _x, int _y, ItemObject _itemObject, StructureObject _structureObject)
    {
        x = _x;
        y = _y;
        itemObject = _itemObject;
        structureObject = _structureObject;
    }
}
