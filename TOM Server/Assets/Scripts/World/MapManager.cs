using RiptideNetworking;
using UnityEngine;

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
        map = new Map(mapWidth, mapHeight, Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset));
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

    private void SendChunkMessage(ushort clientId, Chunk chunk)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.chunk);
        MessageExtentions.Add(message, chunk);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

}