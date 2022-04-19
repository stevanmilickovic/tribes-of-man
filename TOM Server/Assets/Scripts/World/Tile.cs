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

}
