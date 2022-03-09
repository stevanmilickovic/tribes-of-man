using RiptideNetworking;
using UnityEngine;

public static class MessageExtensions
{

    public static Inventory GetInventory(Message message)
    {
        int slotSize = message.GetInt();

        ItemObject[] slots = new ItemObject[slotSize];

        for(int i = 0; i < slotSize; i++)
        {
            int itemId = message.GetInt();
            if(itemId != -1)
            {
                slots[i] = new ItemObject(ItemManager.Singleton.items[itemId], message.GetInt());
            }
        }

        return new Inventory(slots);
    }

    public static Chunk GetChunk(Message message)
    {
        int x = message.GetInt();
        int y = message.GetInt();

        int tileX = message.GetInt();
        int tileY = message.GetInt();

        Tile[,] tiles = new Tile[tileX, tileY];

        for(int _y = 0; _y < tileY; _y++)
        {
            for(int _x = 0; _x < tileX; _x++)
            {
                tiles[_x, _y] = GetTile(message);
            }
        }

        return new Chunk(x, y, tiles);
    }

    public static Tile GetTile(Message message)
    {
        return new Tile(message.GetInt(), message.GetInt(), (Tile.TerrainTypes)message.GetInt());
    }

}
