using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Map
{

    public int seed;

    public Dictionary<(int, int), Tile> tiles = new Dictionary<(int, int), Tile>();
    public Dictionary<(int, int), Chunk> chunks = new Dictionary<(int, int), Chunk>();

    public static Random random = new Random();

    public Map(int mapWidth, int mapHeight, float[,] noise)
    {

        for (int y = 0; y < (int)Mathf.Ceil((float)(mapHeight) / MapConstants.CHUNK_SIZE); y++)
        {
            for (int x = 0; x < (int)Mathf.Ceil((float)(mapWidth) / MapConstants.CHUNK_SIZE); x++)
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
        chunks[(x, y)] = new Chunk(x, y, new Tile[Mathf.Min(mapWidth - x * MapConstants.CHUNK_SIZE, MapConstants.CHUNK_SIZE), Mathf.Min(mapHeight - y * MapConstants.CHUNK_SIZE, MapConstants.CHUNK_SIZE)]);
    }

    private void GenerateTile(int x, int y, float[,] noise)
    {
        Tile tile = new Tile(x, y, SetTerrain(noise[x, y]));
        Chunk chunk = chunks[((int)Mathf.Floor((float)x / MapConstants.CHUNK_SIZE), (int)Mathf.Floor((float)y / MapConstants.CHUNK_SIZE))];
        tiles[(x, y)] = tile;
        chunk.tiles[x % MapConstants.CHUNK_SIZE, y % MapConstants.CHUNK_SIZE] = tiles[(x, y)];
        tile.chunk = chunk;
    }

    public Tile.TerrainTypes SetTerrain(float noise)
    {
        if (noise < 0.4f)
        {
            return Tile.TerrainTypes.Water;
        }
        if (noise < 0.5f && noise >= 0.4f)
        {
            return Tile.TerrainTypes.Sand;
        }
        if (noise < 0.7f && noise >= 0.5f)
        {
            return Tile.TerrainTypes.Grass;
        }
        if (noise >= 0.7f)
        {
            return Tile.TerrainTypes.Stone;
        }

        return Tile.TerrainTypes.Water;
    }
}