using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    public int x;
    public int y;

    public Tile[,] tiles = new Tile[50, 50];

    public Chunk(int _x, int _y, Tile[,] _tiles)
    {
        x = _x;
        y = _y;
        tiles = _tiles;
    }

}
