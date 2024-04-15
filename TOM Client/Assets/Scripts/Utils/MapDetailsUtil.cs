using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MapDetailsUtil
{
    public class DetailSprite
    {
        public Sprite sprite;
        public int weight;

        public DetailSprite(Sprite _sprite, int _weight)
        {
            sprite = _sprite;
            weight = _weight;
        }
    }

    public static Dictionary<Tile.TerrainTypes, DetailSprite[]> terrainDetails = new Dictionary<Tile.TerrainTypes, DetailSprite[]> {
        { Tile.TerrainTypes.Grass, new DetailSprite[] {
            new DetailSprite(GetSprite("grass"), 40),
            new DetailSprite(GetSprite("flower"), 4),
            new DetailSprite(GetSprite("pebbles"), 2),
        }
        },
        {
          Tile.TerrainTypes.Sand, new DetailSprite[]
          {
            new DetailSprite(GetSprite("sandstone"), 10),
            new DetailSprite(GetSprite("pebbles"), 10),
          }
        },
        {
          Tile.TerrainTypes.Water, new DetailSprite[]
          {
            new DetailSprite(GetSprite("wave"), 20),
          }
        },
        {
          Tile.TerrainTypes.Stone, new DetailSprite[]
          {
            new DetailSprite(GetSprite("rock"), 3),
            new DetailSprite(GetSprite("crack"), 3),
          }
        }
    };

    public static DetailSprite GetRandomDetailSprite(Tile.TerrainTypes type, System.Random random)
    {
        if (!terrainDetails.ContainsKey(type)) return null;

        DetailSprite[] details = terrainDetails[type];
        int choice = random.Next(100);
        int sum = 0;

        foreach (var detail in details)
        {
            sum += detail.weight;
            if (choice < sum)
            {
                return detail;
            }
        }

        return null;
    }

    public static Sprite GetSprite(string name)
    {
        return Resources.Load<Sprite>($"Textures/Tile Details/{name}");
    }

}
