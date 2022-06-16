using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    public int x;
    public int y;

    public Tile[,] tiles = new Tile[50, 50];
    public Dictionary<int, Player> players;
    public Dictionary<(int, int), GameObject> spawnedItems;
    public Dictionary<(int, int), GameObject> spawnedStructures;

    public Chunk(int _x, int _y, Tile[,] _tiles)
    {
        x = _x;
        y = _y;
        tiles = _tiles;
        players = new Dictionary<int, Player>();
        spawnedItems = new Dictionary<(int, int), GameObject>();
        spawnedStructures = new Dictionary<(int, int), GameObject>();
    }

}
