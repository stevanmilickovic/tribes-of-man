using RiptideNetworking;
using UnityEngine;
using System;

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

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public Map map;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
    }

    public void InitializeMap()
    {
        map = new Map(mapWidth, mapHeight, Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset));
        map.tiles[1, 1].structureObject = new StructureObject(StructureManager.Singleton.structures[0]); //TO REMOVE LATER
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
    }

    public void SendAllChunks(ushort clientId)
    {
        foreach (Chunk chunk in map.chunks)
        {
            SendChunkMessage(clientId, chunk);
        }
    }

    public void SendChunkMessage(ushort clientId, Chunk chunk)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.chunk);
        MessageExtentions.Add(message, chunk);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    public void SendTileMessage(Tile tile)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.tile);
        MessageExtentions.Add(message, tile);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public Tile DropItem(int x, int y, ItemObject itemObject)
    {
        Tile tile = map.tiles[x, y];

        if(TryToTakeItem(tile, itemObject))
        {
            return tile;
        }
        
        for (int _y = y + 1; _y >= y - 1; _y--)
        {
            for (int _x = x - 1; _x <= x + 1; _x++)
            {
                if (_x >= 0 && _y >= 0 && _x < map.tiles.GetLength(0) && _y < map.tiles.GetLength(1))
                {
                    if (TryToTakeItem(map.tiles[_x, _y], itemObject))
                        return (map.tiles[_x, _y]);
                }
            }
        }

        return null;
    }

    private bool TryToTakeItem(Tile tile, ItemObject itemObject)
    {
        if (tile.itemObject == null)
        {
            tile.itemObject = itemObject;
            return true;
        }
        return false;
    }

}