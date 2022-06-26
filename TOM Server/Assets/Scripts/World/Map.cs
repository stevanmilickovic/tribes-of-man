using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Map
{

    public int seed;

    public Tile[,] tiles;
    public Chunk[,] chunks;

    public static Random random = new Random();
    public List<StructureMap> structureMaps = new List<StructureMap>();

    public Map(int mapWidth, int mapHeight, float[,] noise)
    {
        tiles = new Tile[mapWidth, mapHeight];
        chunks = new Chunk[(int)Mathf.Ceil((float)(mapWidth) / 10), (int)Mathf.Ceil((float)(mapHeight) / 10)];
        AddStructureMaps(seed, mapWidth, mapHeight);

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

        GenerateFlora(mapWidth, mapHeight);

        tiles[0, 0].itemObject = new ItemObject(ItemManager.Singleton.items[0], 1); //TO REMOVE LATER BUT MAYBE NOT
    }

    private void GenerateChunk(int x, int y, int mapWidth, int mapHeight)
    {
        chunks[x, y] = new Chunk(x, y, new Tile[10, 10]);

        if ((x + 1) * 10 > mapWidth && !((y + 1) * 10 > mapHeight))
        {
            chunks[x, y] = new Chunk(x, y, new Tile[mapWidth - x * 10, 10]);
        }
        if ((y + 1) * 10 > mapHeight && !((x + 1) * 10 > mapWidth))
        {
            chunks[x, y] = new Chunk(x, y, new Tile[10, mapHeight - y * 10]);
        }
        if ((x + 1) * 10 > mapWidth && (y + 1) * 10 > mapHeight)
        {
            chunks[x, y] = new Chunk(x, y, new Tile[mapWidth - x * 10, mapHeight - y * 10]);
        }
    }

    private void GenerateTile(int x, int y, float[,] noise)
    {
        Tile tile = new Tile(x, y, SetTerrain(noise[x, y]));
        Chunk chunk = chunks[(int)Mathf.Floor((float)x / 10), (int)Mathf.Floor((float)y / 10)];
        tiles[x, y] = tile;
        chunk.tiles[x % 10, y % 10] = tiles[x, y];
        tile.chunk = chunk;
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

    private void AddStructureMaps(int seed, int mapWidth, int mapHeight)
    {
        AddStructureMap(seed, mapWidth, mapHeight, 30, "Tree", TerrainTypes.Grass);
        AddStructureMap(seed, mapWidth, mapHeight, 40, "Bush", TerrainTypes.Grass);
        AddStructureMap(seed, mapWidth, mapHeight, 45, "Berries", TerrainTypes.Grass);
    }

    private void AddStructureMap(int seed, int mapWidth, int mapHeight, int perThousand, string structureName, TerrainTypes terrainType)
    {
        structureMaps.Add(new StructureMap(seed, mapWidth, mapHeight, perThousand, StructureManager.Singleton.structuresByName[structureName], terrainType));
    }

    private void GenerateFlora(int mapWidth, int mapHeight)
    {
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                foreach (StructureMap map in structureMaps)
                {
                    if (map.map[x, y] == 1 && tiles[x, y].structureObject == null && tiles[x, y].type == map.terrainType)
                    {
                        tiles[x, y].SpawnStructure(map.structure);
                    }
                }
            }
        }
    }
}

public class StructureMap
{
    public int[,] map;
    public Structure structure;
    public TerrainTypes terrainType;

    public StructureMap(int seed, int mapWidth, int mapHeight, int perThousand, Structure _structure, TerrainTypes _terrainType)
    {
        map = new int[mapWidth, mapHeight];
        Random random = new Random(seed);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y] = random.Next(1000 / perThousand) == 1 ? 1 : 0;
            }
        }
        structure = _structure;
        terrainType = _terrainType;
    }
}