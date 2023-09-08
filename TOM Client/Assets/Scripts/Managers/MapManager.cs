using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Linq;

struct Key
{
    public readonly int x;
    public readonly int y;
    public Key(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

struct TextureVersion
{
    public bool R; //regular
    public bool LE; //left edge
    public bool RE; //right edge
    public bool BE; //bottom edge
    public bool TE; //top edge
    public bool BLC; //bottom left corner
    public bool BRC; //bottom right corner
    public bool TLC; //top left corner
    public bool TRC; //top right corner
    public bool BLE; //bottom left edge
    public bool BRE; //bottom right edge
    public bool TLE; //top left edge
    public bool TRE; //top right edge
}

public class MapManager : MonoBehaviour
{

    private static MapManager singleton;

    public static MapManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(MapManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private GameObject itemObjectPrefab;
    [SerializeField] private GameObject largeStructureObjectPrefab;
    [SerializeField] private GameObject mediumStructureObjectPrefab;
    [SerializeField] private GameObject smallStructureObjectPrefab;
    private Dictionary<(int, int), GameObject> spawnedItems;
    private Dictionary<(int, int), GameObject> spawnedStructures; 
    public Dictionary<(int, int), Chunk> chunks;
    public Dictionary<(int, int), GameObject> chunkObjects;
    public Dictionary<(int, int), Tile> tiles;

    public static int CHUNK_SIZE = 5;
    public static int VIEW_RANGE = 3;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        chunks = new Dictionary<(int, int), Chunk>();
        chunkObjects = new Dictionary<(int, int), GameObject>();
        tiles = new Dictionary<(int, int), Tile>();
        spawnedItems = new Dictionary<(int, int), GameObject>();
        spawnedStructures = new Dictionary<(int, int), GameObject>();
    }

    public void CreateChunk(Chunk chunk)
    {
        if (chunks.ContainsKey((chunk.x, chunk.y))) return;

        chunks.Add((chunk.x, chunk.y), chunk);

        GameObject newChunk = Instantiate(chunkPrefab);
        newChunk.transform.Translate(chunk.x * CHUNK_SIZE, chunk.y * CHUNK_SIZE, 0f);
        chunkObjects.Add((chunk.x, chunk.y), newChunk);

        DrawTiles(chunk, newChunk.GetComponent<MeshFilter>().mesh);
    }

    public void UpdateRelevantChunks(Chunk oldChunk, Chunk newChunk)
    {
        int differenceX = oldChunk.x - newChunk.x;
        int differenceY = oldChunk.y - newChunk.y;

        if (differenceX != 0)
        {
            for (int y = oldChunk.y - VIEW_RANGE; y <= oldChunk.y + VIEW_RANGE; y++)
            {
                int rowToDelete = oldChunk.x + (differenceX < 0 ? -VIEW_RANGE : VIEW_RANGE);
                if (chunks.ContainsKey((rowToDelete, y)))
                {
                    DestroyEntitiesInChunk(chunks[(rowToDelete, y)]);
                    chunks.Remove((rowToDelete, y));
                    Destroy(chunkObjects[(rowToDelete, y)]);
                    chunkObjects.Remove((rowToDelete, y));
                }
            }
        }
        if (differenceY != 0)
        {
            int columnToDelete = oldChunk.y + (differenceY < 0 ? -VIEW_RANGE : VIEW_RANGE);
            for (int x = oldChunk.x - VIEW_RANGE; x <= oldChunk.x + VIEW_RANGE; x++)
            {
                if (chunks.ContainsKey((x, columnToDelete)))
                {
                    DestroyEntitiesInChunk(chunks[(x, columnToDelete)]);
                    chunks.Remove((x, columnToDelete));
                    Destroy(chunkObjects[(x, columnToDelete)]);
                    chunkObjects.Remove((x, columnToDelete));
                }
            }
        }
    }

    private void DestroyEntitiesInChunk(Chunk chunk)
    {
        foreach (KeyValuePair<int, Player> player in chunk.players)
        {
            Destroy(player.Value.gameObject);
            PlayerManager.Singleton.players.Remove(player.Value.id);
        }
        foreach (KeyValuePair<(int, int), GameObject> spawnedItem in chunk.spawnedItems)
        {
            Destroy(spawnedItem.Value);
            spawnedItems.Remove(spawnedItem.Key);
        }
        foreach (KeyValuePair<(int, int), GameObject> spawnedStructure in chunk.spawnedStructures)
        {
            Destroy(spawnedStructure.Value);
            spawnedStructures.Remove(spawnedStructure.Key);
        }
    }

    private void DrawTiles(Chunk chunk, Mesh mesh)
    {
        Vector3[] vertices = new Vector3[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 4 * 4];
        int[] triangles = new int[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 6 * 4];
        Vector2[] uvs = new Vector2[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 4 * 4];
        int vert = 0;
        int tris = 0;
        int cell = 0;


        for (int y = 0; y < chunk.tiles.GetLength(1); y++) for (int x = 0; x < chunk.tiles.GetLength(0); x++) AddTileToDictionary(x, y, chunk);

        int i = 0;
        for (int y = 0; y < chunk.tiles.GetLength(1); y++)
        {
            for (int x = 0; x < chunk.tiles.GetLength(0); x++)
            {
                int quadrant = 0;
                int firstNeighbour = 0;
                chunk.tiles[x, y].firstVert = cell;

                for (int quadY = 0; quadY < 2; quadY++)
                {
                    for (int quadX = 0; quadX < 2; quadX++)
                    {
                        SetVertices(vertices, i, x + (quadX * 0.5f), y + (quadY * 0.5f));
                        SetTriangles(triangles, tris, vert);

                        Tile[] neighbourTiles = getNeighbourTiles(chunk.tiles[x, y]);
                        SetTextureUVs(uvs, chunk, cell, x, y, quadrant, neighbourTiles.SubArray(firstNeighbour, 3));

                        foreach (Tile tile in neighbourTiles)
                        {
                            if (tile == null) continue;
                            Chunk neighbourChunk = MapUtil.GetChunk(tile);
                            if (neighbourChunk != null && neighbourChunk != chunk) UpdateTileUvs(tile);
                        }

                        firstNeighbour += 3;
                        quadrant++;

                        i += 4;
                        tris += 6;
                        vert += 4;
                        cell += 4;
                    }
                }
                
                UpdateTileItemAndStructure(chunk.tiles[x, y]);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    private void UpdateTileUvs(Tile tile)
    {
        int quadrant = 0;
        int firstNeighbour = 0;
        int cell = tile.firstVert;
        Chunk chunk = MapUtil.GetChunk(tile);
        if (chunk == null) return;
        Mesh mesh = chunkObjects[(chunk.x, chunk.y)].GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = mesh.uv;

        for (int quadY = 0; quadY < 2; quadY++)
        {
            for (int quadX = 0; quadX < 2; quadX++)
            {
                Tile[] neighbourTiles = getNeighbourTiles(tile);
                SetTextureUVs(uvs, chunk, cell, tile.x % chunk.tiles.GetLength(0), tile.y % chunk.tiles.GetLength(1), quadrant, neighbourTiles.SubArray(firstNeighbour, 3));

                firstNeighbour += 3;
                quadrant++;
                cell += 4;
            }
        }
        mesh.uv = uvs;
    }

    private void AddTileToDictionary(int x, int y, Chunk chunk)
    {
        if (tiles.ContainsKey((chunk.tiles[x, y].x, chunk.tiles[x, y].y))) return;
        tiles.Add((chunk.tiles[x, y].x, chunk.tiles[x, y].y), chunk.tiles[x, y]);
    }

    private void SetVertices(Vector3[] vertices, int i, float x, float y)
    {
        vertices[i] = new Vector3(x, y, 0);
        vertices[i + 1] = new Vector3(x + 0.5f, y, 0);
        vertices[i + 2] = new Vector3(x, y + 0.5f, 0);
        vertices[i + 3] = new Vector3(x + 0.5f, y + 0.5f, 0);
    }

    private void SetTriangles(int[] triangles, int tris, int vert)
    {
        triangles[tris + 0] = vert;
        triangles[tris + 1] = vert + 2;
        triangles[tris + 2] = vert + 1;
        triangles[tris + 3] = vert + 1;
        triangles[tris + 4] = vert + 2;
        triangles[tris + 5] = vert + 3;
    }

    private void SetTextureUVs(Vector2[] uvs, Chunk chunk, int cell, int x, int y, int quadrant, Tile[] neighbours)
    {
        if (uvs.Length == 0) return;
        Vector2[] currentUvs = GetTextureUVArray(chunk.tiles[x, y], quadrant, neighbours);

        uvs[cell] = currentUvs[0];
        uvs[cell + 1] = currentUvs[1];
        uvs[cell + 2] = currentUvs[2];
        uvs[cell + 3] = currentUvs[3];
    }

    static int TEXTURE_WIDTH = 45;
    static int TEXTURE_HEIGHT = 78;
    static int TYPE_HEIGHT = 22;
    static int CELL_CONTAINER_SIZE = 11;
    static int CELL_SIZE = 8;

    private float getWidthPercentage(float numberOfPixels)
    {
        return numberOfPixels / TEXTURE_WIDTH;
    }

    private float getHeightPercentage(float numberOfPixels)
    {
        return numberOfPixels / TEXTURE_HEIGHT;
    }

    private Vector2[] getTextureCoordinates(Vector2 sectorStart, int textureX, int textureY)
    {
        float textureStartX = sectorStart.x + (textureX - 1) * getWidthPercentage(CELL_CONTAINER_SIZE);
        float textureStartY = sectorStart.y + (textureY - 1) * getHeightPercentage(CELL_CONTAINER_SIZE);

        Vector2[] result = {
            new Vector2(textureStartX, textureStartY),
            new Vector2(textureStartX + getWidthPercentage(CELL_SIZE), textureStartY),
            new Vector2(textureStartX, textureStartY + getHeightPercentage(CELL_SIZE)),
            new Vector2(textureStartX + getWidthPercentage(CELL_SIZE), textureStartY + getHeightPercentage(CELL_SIZE))
        };

        return result;
    }

    private Vector2[] invertTextureCoordinates(Vector2[] uvs)
    {
        Vector2[] result = { uvs[1], uvs[0], uvs[3], uvs[2] };
        return result;
    }

    private Vector2[] GetTextureUVArray(Tile tile, int quadrant, Tile[] neighbours)
    {
        float startingHeigthPercentage = 0f;
        Tile.TerrainTypes type = tile.type;
        TextureVersion version = getTextureVersion(tile, quadrant, neighbours);

        switch(type)
        {
            case Tile.TerrainTypes.Water: startingHeigthPercentage = getHeightPercentage(CELL_CONTAINER_SIZE) + 2f * getHeightPercentage(TYPE_HEIGHT); break;
            case Tile.TerrainTypes.Sand: startingHeigthPercentage = getHeightPercentage(CELL_CONTAINER_SIZE) + getHeightPercentage(TYPE_HEIGHT); break;
            case Tile.TerrainTypes.Grass: startingHeigthPercentage = getHeightPercentage(CELL_CONTAINER_SIZE); break;
            case Tile.TerrainTypes.Stone: startingHeigthPercentage = 0; break;
            default: break;
        }

        Vector2 sectorStart = new Vector2(getWidthPercentage(2), startingHeigthPercentage + getHeightPercentage(2));

        if (!isSingleTypeTexture(type))
        {
            if (version.R) return getTextureCoordinates(sectorStart, 1, 2);
            if (version.BE) return getTextureCoordinates(sectorStart, 2, 2);
            if (version.LE) return getTextureCoordinates(sectorStart, 3, 2);
            if (version.RE) return invertTextureCoordinates(getTextureCoordinates(sectorStart, 3, 2));
            if (version.TE) return getTextureCoordinates(sectorStart, 4, 2);
            if (version.TLC) return getTextureCoordinates(sectorStart, 1, 1);
            if (version.TRC) return invertTextureCoordinates(getTextureCoordinates(sectorStart, 1, 1));
            if (version.BLC) return getTextureCoordinates(sectorStart, 2, 1);
            if (version.BRC) return invertTextureCoordinates(getTextureCoordinates(sectorStart, 2, 1));
            if (version.TLE) return getTextureCoordinates(sectorStart, 3, 1);
            if (version.TRE) return invertTextureCoordinates(getTextureCoordinates(sectorStart, 3, 1));
            if (version.BLE) return getTextureCoordinates(sectorStart, 4, 1);
            if (version.BRE) return invertTextureCoordinates(getTextureCoordinates(sectorStart, 4, 1));
        }

        return getTextureCoordinates(sectorStart, 1, 1);
    }

    private bool isSingleTypeTexture(Tile.TerrainTypes type)
    {
        Tile.TerrainTypes[] singleTypes = { Tile.TerrainTypes.Stone };
        if (singleTypes.Contains(type)) return true;
        else return false;
    }

    private TextureVersion getTextureVersion(Tile tile, int quadrant, Tile[] neighbours)
    {
        Tile.TerrainTypes type = getTypeForTransition(tile.type);
        bool first = neighbours[0] != null && neighbours[0].type == type;
        bool second = neighbours[1] != null && neighbours[1].type == type;
        bool third = neighbours[2] != null && neighbours[2].type == type;

        TextureVersion result = new TextureVersion();

        if (first && !third)
        {
            switch (quadrant)
            {
                case 0: result.LE = true; break;
                case 1: result.BE = true; break;
                case 2: result.TE = true; break;
                case 3: result.RE = true; break;
            }
        } else if (third && !first)
        {
            switch (quadrant)
            {
                case 0: result.BE = true; break;
                case 1: result.RE = true; break;
                case 2: result.LE = true; break;
                case 3: result.TE = true; break;
            }
        } else if (first && third)
        {
            switch (quadrant)
            {
                case 0: result.BLE = true; break;
                case 1: result.BRE = true; break;
                case 2: result.TLE = true; break;
                case 3: result.TRE = true; break;
            }
        } else if (second && !first && !third)
        {
            switch (quadrant)
            {
                case 0: result.BLC = true; break;
                case 1: result.BRC = true; break;
                case 2: result.TLC = true; break;
                case 3: result.TRC = true; break;
            }
        } else
        {
            result.R = true;
        }

        return result;
    }

    private Tile.TerrainTypes getTypeForTransition(Tile.TerrainTypes type)
    {
        switch (type)
        {
            case Tile.TerrainTypes.Water: return Tile.TerrainTypes.Sand;
            case Tile.TerrainTypes.Sand: return Tile.TerrainTypes.Grass;
            case Tile.TerrainTypes.Grass: return Tile.TerrainTypes.Stone;
            default: return Tile.TerrainTypes.Water;
        }
    }

    private Tile[] getNeighbourTiles(Tile tile)
    {
        Tile[] neighbourTiles = new Tile[12];

        neighbourTiles[0] = tiles.ContainsKey((tile.x - 1, tile.y)) ? tiles[(tile.x - 1, tile.y)] : null;
        neighbourTiles[1] = tiles.ContainsKey((tile.x - 1, tile.y - 1)) ? tiles[(tile.x - 1, tile.y - 1)] : null;
        neighbourTiles[2] = tiles.ContainsKey((tile.x, tile.y - 1)) ? tiles[(tile.x, tile.y - 1)] : null;

        neighbourTiles[3] = tiles.ContainsKey((tile.x, tile.y - 1)) ? tiles[(tile.x, tile.y - 1)] : null;
        neighbourTiles[4] = tiles.ContainsKey((tile.x + 1, tile.y - 1)) ? tiles[(tile.x + 1, tile.y - 1)] : null;
        neighbourTiles[5] = tiles.ContainsKey((tile.x + 1, tile.y)) ? tiles[(tile.x + 1, tile.y)] : null;

        neighbourTiles[6] = tiles.ContainsKey((tile.x, tile.y + 1)) ? tiles[(tile.x, tile.y + 1)] : null;
        neighbourTiles[7] = tiles.ContainsKey((tile.x - 1, tile.y + 1)) ? tiles[(tile.x - 1, tile.y + 1)] : null;
        neighbourTiles[8] = tiles.ContainsKey((tile.x - 1, tile.y)) ? tiles[(tile.x - 1, tile.y)] : null;

        neighbourTiles[9] = tiles.ContainsKey((tile.x + 1, tile.y)) ? tiles[(tile.x + 1, tile.y)] : null;
        neighbourTiles[10] = tiles.ContainsKey((tile.x + 1, tile.y + 1)) ? tiles[(tile.x + 1, tile.y + 1)] : null;
        neighbourTiles[11] = tiles.ContainsKey((tile.x, tile.y + 1)) ? tiles[(tile.x, tile.y + 1)] : null;

        return neighbourTiles;
    }

    public void UpdateTileItemAndStructure(Tile tile)
    {
        UpdateTileItemObject(tile);
        UpdateTileStructureObject(tile);
    }

    public void UpdateTileItemObject(Tile tile)
    {
        tiles[(tile.x, tile.y)].itemObject = tile.itemObject;
        chunks[(tile.x / CHUNK_SIZE, tile.y / CHUNK_SIZE)].spawnedItems.Remove((tile.x, tile.y));
        if (spawnedItems.ContainsKey((tile.x, tile.y)))
        {
            Destroy(spawnedItems[(tile.x, tile.y)]);
        }

        if(tile.itemObject != null)
        {
            chunks[(tile.x / CHUNK_SIZE, tile.y / CHUNK_SIZE)].spawnedItems.Add((tile.x, tile.y), SpawnItem(tile.x, tile.y, tile.itemObject));
        }
    }

    public GameObject SpawnItem(int x, int y, ItemObject itemObject)
    {
        spawnedItems[(x, y)] = Instantiate(itemObjectPrefab);
        spawnedItems[(x, y)].transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
        spawnedItems[(x, y)].GetComponentInChildren<SpriteRenderer>().sprite = itemObject.item.sprite;
        spawnedItems[(x, y)].transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = itemObject.amount == 1 ? "" : itemObject.amount.ToString();
        return spawnedItems[(x, y)];
    }

    public void UpdateTileStructureObject(Tile tile)
    {
        tiles[(tile.x, tile.y)].structureObject = tile.structureObject;
        chunks[(tile.x / CHUNK_SIZE, tile.y / CHUNK_SIZE)].spawnedStructures.Remove((tile.x, tile.y));

        if (spawnedStructures.ContainsKey((tile.x, tile.y)))
        {
            Destroy(spawnedStructures[(tile.x, tile.y)]);
        }

        if (tile.structureObject != null)
        {
            chunks[(tile.x / CHUNK_SIZE, tile.y / CHUNK_SIZE)].spawnedStructures.Add((tile.x, tile.y), SpawnStructure(tile.x, tile.y, tile.structureObject));
        }
    }

    private GameObject SpawnStructure(int x, int y, StructureObject structureObject)
    {
        spawnedStructures[(x, y)] = InstantiateStructure(structureObject.structure);
        spawnedStructures[(x, y)].transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
        spawnedStructures[(x, y)].GetComponentInChildren<SpriteRenderer>().sprite = 
            ! structureObject.destroyed 
            ? structureObject.structure.sprite
            : structureObject.structure.collapsedSprite;
        if (structureObject.destroyed)
            spawnedStructures[(x, y)].GetComponent<Collider2D>().isTrigger = true;
        return spawnedStructures[(x, y)];
    }

    private GameObject InstantiateStructure(Structure structure)
    {
        if (structure.sizeType == Structure.SizeType.Large)
            return Instantiate(largeStructureObjectPrefab);
        if (structure.sizeType == Structure.SizeType.Medium)
            return Instantiate(mediumStructureObjectPrefab);
        if (structure.sizeType == Structure.SizeType.Small)
            return Instantiate(smallStructureObjectPrefab);
        return null;
    }

    public Tile DropItem(int x, int y, ItemObject itemObject)
    {
        Tile tile = tiles[(x, y)];

        if (TryToDrop(tile, itemObject))
        {
            return tile;
        }

        for (int _y = y + 1; _y >= y - 1; _y--)
        {
            for (int _x = x - 1; _x <= x + 1; _x++)
            {
                if (TryToDrop(tiles[(x, y)], itemObject))
                    return (tiles[(x, y)]);
            }
        }

        return null;
    }

    private bool TryToDrop(Tile tile, ItemObject itemObject)
    {
        if (tile.itemObject == null)
        {
            tile.itemObject = itemObject;
            return true;
        }
        return false;
    }

}
