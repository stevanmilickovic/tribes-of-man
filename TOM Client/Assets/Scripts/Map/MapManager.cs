using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (chunks.ContainsKey((chunk.x, chunk.y)))
            return;
        chunks.Add((chunk.x, chunk.y), chunk);


        GameObject newChunk = Instantiate(chunkPrefab);
        newChunk.transform.Translate(chunk.x * 10, chunk.y * 10, 0f);
        chunkObjects.Add((chunk.x, chunk.y), newChunk);

        DrawTiles(chunk, newChunk.GetComponent<MeshFilter>().mesh);
    }

    public void UpdateRelevantChunks(Chunk oldChunk, Chunk newChunk)
    {
        int differenceX = oldChunk.x - newChunk.x;
        int differenceY = oldChunk.y - newChunk.y;

        if (differenceX != 0)
        {
            for (int y = oldChunk.y - 1; y <= oldChunk.y + 1; y++)
            {
                if (chunks.ContainsKey((oldChunk.x + differenceX, y)))
                {
                    DestroyEntitiesInChunk(chunks[(oldChunk.x + differenceX, y)]);
                    chunks.Remove((oldChunk.x + differenceX, y));
                    Destroy(chunkObjects[(oldChunk.x + differenceX, y)]);
                    chunkObjects.Remove((oldChunk.x + differenceX, y));
                }
            }
        }
        if (differenceY != 0)
        {
            for (int x = oldChunk.x - 1; x <= oldChunk.x + 1; x++)
            {
                if (chunks.ContainsKey((x, oldChunk.y + differenceY)))
                {
                    DestroyEntitiesInChunk(chunks[(x, oldChunk.y + differenceY)]);
                    chunks.Remove((x, oldChunk.y + differenceY));
                    Destroy(chunkObjects[(x, oldChunk.y + differenceY)]);
                    chunkObjects.Remove((x, oldChunk.y + differenceY));
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
        Vector3[] vertices = new Vector3[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 4];
        int[] triangles = new int[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 6];
        Vector2[] uvs = new Vector2[chunk.tiles.GetLength(0) * chunk.tiles.GetLength(1) * 4];
        int vert = 0;
        int tris = 0;
        int cell = 0;
        Vector2[] currectUvs = new Vector2[4];

        for (int i = 0, y = 0; y < chunk.tiles.GetLength(1); y++)
        {
            for (int x = 0; x < chunk.tiles.GetLength(0); x++)
            {
                SetVertices(vertices, i, x, y);
                SetTriangles(triangles, tris, vert);
                SetUVs(currectUvs, uvs, chunk, cell, x, y);
                AddTile(x, y, chunk);
                UpdateTile(chunk.tiles[x, y]);

                i += 4;
                tris += 6;
                vert += 4;
                cell += 4;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    private void AddTile(int x, int y, Chunk chunk)
    {
        if (tiles.ContainsKey((chunk.tiles[x, y].x, chunk.tiles[x, y].y))) return;
        tiles.Add((chunk.tiles[x, y].x, chunk.tiles[x, y].y), chunk.tiles[x, y]);
    }

    private void SetVertices(Vector3[] vertices, int i, int x, int y)
    {
        vertices[i] = new Vector3(x, y, 0);
        vertices[i + 1] = new Vector3(x + 1, y, 0);
        vertices[i + 2] = new Vector3(x, y + 1, 0);
        vertices[i + 3] = new Vector3(x + 1, y + 1, 0);
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

    private void SetUVs(Vector2[] currentUvs, Vector2[] uvs, Chunk chunk, int cell, int x, int y)
    {
        currentUvs = GetTexture(chunk.tiles[x, y].type);

        uvs[cell] = currentUvs[0];
        uvs[cell + 1] = currentUvs[1];
        uvs[cell + 2] = currentUvs[2];
        uvs[cell + 3] = currentUvs[3];
    }

    private Vector2[] GetTexture(Tile.TerrainTypes type)
    {
        Vector2[] uvs = new Vector2[4];

        if (type == Tile.TerrainTypes.Water)
        {
            uvs[0] = new Vector2(0.5f, 0.5f);
            uvs[1] = new Vector2(1f, 0.5f);
            uvs[2] = new Vector2(0.5f, 1f);
            uvs[3] = new Vector2(1f, 1f);
        }
        if (type == Tile.TerrainTypes.Sand)
        {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0.5f, 0f);
            uvs[2] = new Vector2(0f, 0.5f);
            uvs[3] = new Vector2(0.5f, 0.5f);
        }
        if (type == Tile.TerrainTypes.Grass)
        {
            uvs[0] = new Vector2(0f, 0.5f);
            uvs[1] = new Vector2(0.5f, 0.5f);
            uvs[2] = new Vector2(0f, 1f);
            uvs[3] = new Vector2(0.5f, 1f);
        }
        if (type == Tile.TerrainTypes.Stone)
        {
            uvs[0] = new Vector2(0.5f, 0f);
            uvs[1] = new Vector2(1f, 0f);
            uvs[2] = new Vector2(0.5f, 0.5f);
            uvs[3] = new Vector2(1f, 0.5f);
        }

        return uvs;
    }

    public void UpdateTile(Tile tile)
    {
        UpdateTileItemObject(tile);
        UpdateTileStructureObject(tile);
    }

    public void UpdateTileItemObject(Tile tile)
    {
        tiles[(tile.x, tile.y)].itemObject = tile.itemObject;
        chunks[(tile.x / 10, tile.y / 10)].spawnedItems.Remove((tile.x, tile.y));
        if (spawnedItems.ContainsKey((tile.x, tile.y)))
        {
            Destroy(spawnedItems[(tile.x, tile.y)]);
        }

        if(tile.itemObject != null)
        {
            chunks[(tile.x/10, tile.y/10)].spawnedItems.Add((tile.x, tile.y), SpawnItem(tile.x, tile.y, tile.itemObject));
        }
    }

    public GameObject SpawnItem(int x, int y, ItemObject itemObject)
    {
        spawnedItems[(x, y)] = Instantiate(itemObjectPrefab);
        spawnedItems[(x, y)].transform.position = new Vector3(x + 0.5f, y + 0.5f, 0f);
        spawnedItems[(x, y)].GetComponentInChildren<SpriteRenderer>().sprite = itemObject.item.sprite;
        return spawnedItems[(x, y)];
    }

    public void UpdateTileStructureObject(Tile tile)
    {
        tiles[(tile.x, tile.y)].structureObject = tile.structureObject;
        chunks[(tile.x / 10, tile.y / 10)].spawnedStructures.Remove((tile.x, tile.y));

        if (spawnedStructures.ContainsKey((tile.x, tile.y)))
        {
            Destroy(spawnedStructures[(tile.x, tile.y)]);
        }

        if (tile.structureObject != null)
        {
            chunks[(tile.x / 10, tile.y / 10)].spawnedStructures.Add((tile.x, tile.y), SpawnStructure(tile.x, tile.y, tile.structureObject));
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
