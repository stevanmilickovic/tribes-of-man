using RiptideNetworking;
using UnityEngine;

public static class MessageExtentions
{

    public static Message AddInventory(Message message, Inventory inventory) => Add(message, inventory);

    public static Message Add(Message message, Inventory inventory)
    {
        message.Add(inventory.slotNumber);

        for(int i = 0; i < inventory.slotNumber; i++)
        {
            if (inventory.slots[i] == null)
                message.Add(-1);
            else
            {
                message.Add(inventory.slots[i].item.id);
                message.Add(inventory.slots[i].amount);
            }
        }
        return message;
    }

    public static Message AddChunk(Message message, Chunk chunk) => Add(message, chunk);
    
    public static Message Add(Message message, Chunk chunk)
    {

        message.Add(chunk.x);
        message.Add(chunk.y);

        message.Add(chunk.tiles.GetLength(0));
        message.Add(chunk.tiles.GetLength(1));

        for (int y = 0; y < chunk.tiles.GetLength(1); y++)
        {
            for (int x = 0; x < chunk.tiles.GetLength(0); x++)
            {
                Add(message, chunk.tiles[x, y]);
            }
        }

        return message;
    }

    public static Message AddTile(Message message, Tile tile) => Add(message, tile);

    public static Message Add(Message message, Tile tile)
    {

        message.Add(tile.x);
        message.Add(tile.y);

        message.Add((int)tile.type);
        AddItemObject(message, tile.itemObject);

        return message;
    }

    public static Message AddItemObject(Message message, ItemObject itemObject)
    {
        if (itemObject == null)
        {
            message.Add(-1);
            return message;
        }

        Debug.Log($"ID is {itemObject.item.id}");
        message.Add(itemObject.item.id);
        message.Add(itemObject.amount);

        return message;
    }

}
