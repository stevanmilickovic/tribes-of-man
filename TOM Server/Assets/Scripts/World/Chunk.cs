using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk
{

    public int x;
    public int y;

    public Tile[,] tiles = new Tile[50, 50];
    public List<Player> players;
    public List<Player> playersInRange;

    public Chunk(int _x, int _y, Tile[,] _tiles)
    {
        x = _x;
        y = _y;
        tiles = _tiles;
        players = new List<Player>();
        playersInRange = new List<Player>();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }

    public bool HasPlayer(Player player)
    {
        return players.Contains(player);
    }

}
