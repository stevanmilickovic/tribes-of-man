using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{

    public int seed;

    public Tile[,] tiles;
    public Chunk[,] chunks;

    public Map(int mapWidth, int mapHeight, float[,] noise)
    {
        tiles = new Tile[mapWidth, mapHeight];
        chunks = new Chunk[(int)Mathf.Ceil((float)(mapWidth) / 50), (int)Mathf.Ceil((float)(mapHeight) / 50)];

        for (int y = 0; y < chunks.GetLength(1); y++)
        {
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                GenerateChunk(x, y, mapWidth, mapHeight);
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                GenerateTile(x, y, noise);
            }
        }
    }

    private void GenerateChunk(int x, int y, int mapWidth, int mapHeight)
    {
        chunks[x, y] = new Chunk(x, y, new Tile[50, 50]);

        if ((x + 1) * 50 > mapWidth && !((y + 1) * 50 > mapHeight))
        {
            chunks[x, y] = new Chunk(x, y, new Tile[mapWidth - x * 50, 50]);
        }
        if ((y + 1) * 50 > mapHeight && !((x + 1) * 50 > mapWidth))
        {
            chunks[x, y] = new Chunk(x, y, new Tile[50, mapHeight - y * 50]);
        }
        if ((x + 1) * 50 > mapWidth && (y + 1) * 50 > mapHeight)
        {
            chunks[x, y] = new Chunk(x, y, new Tile[mapWidth - x * 50, mapHeight - y * 50]);
        }
    }

    private void GenerateTile(int x, int y, float[,] noise)
    {
        tiles[x, y] = new Tile(x, y, SetTerrain(noise[x, y]));
        chunks[(int)Mathf.Floor((float)x / 50), (int)Mathf.Floor((float)y / 50)].tiles[x % 50, y % 50] = tiles[x, y];
    }

    public TerrainTypes SetTerrain(float noise)
    {
        if (noise < 0.4f)
        {
            return TerrainTypes.Water;
        }
        if (noise < 0.5f && noise >= 0.4f)
        {
            return TerrainTypes.Sand;
        }
        if (noise < 0.7f && noise >= 0.5f)
        {
            return TerrainTypes.Grass;
        }
        if (noise >= 0.7f)
        {
            return TerrainTypes.Stone;
        }

        return TerrainTypes.Water;
    }

}
