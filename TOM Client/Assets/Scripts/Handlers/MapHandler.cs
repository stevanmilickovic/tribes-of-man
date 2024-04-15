using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class MapHandler
{
    [MessageHandler((ushort)ServerToClientId.seed)]
    public static void GenerateMap(Message message)
    {
        /*
        message.AddInt(MapManager.Singleton.mapWidth);
        message.AddInt(MapManager.Singleton.mapHeight);
        message.AddInt(MapManager.Singleton.seed);
        message.AddFloat(MapManager.Singleton.noiseScale);
        message.AddInt(MapManager.Singleton.octaves);
        message.AddFloat(MapManager.Singleton.persistence);
        message.AddFloat(MapManager.Singleton.lacunarity);
        message.AddVector2(MapManager.Singleton.offset);
        */
        int mapWidth = message.GetInt();
        int mapHeight = message.GetInt();
        int seed = message.GetInt();
        float noiseScale = message.GetFloat();
        int octaves = message.GetInt();
        float persistence = message.GetFloat();
        float lacunarity = message.GetFloat();
        Vector2 offset = message.GetVector2();
        MapManager.Singleton.GenerateMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);
    }

    [MessageHandler((ushort)ServerToClientId.tile)]
    public static void UpdateTile(Message message)
    {
        Tile tile = MessageExtensions.GetTile(message);
        MapManager.Singleton.UpdateTileItemAndStructure(tile);
    }

}
