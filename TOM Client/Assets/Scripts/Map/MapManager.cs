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
    public Dictionary<(int, int), Chunk> chunks;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        chunks = new Dictionary<(int, int), Chunk>();
    }

    public void CreateChunk(Chunk chunk)
    {
        chunks.Add((chunk.x, chunk.y), chunk);

        GameObject newChunk = Instantiate(chunkPrefab);
        newChunk.transform.Translate(chunk.x * 50, chunk.y * 50, 0f);

        DrawTiles(chunk, newChunk.GetComponent<MeshFilter>().mesh);
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

}
