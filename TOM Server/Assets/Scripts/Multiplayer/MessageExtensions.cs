using RiptideNetworking;
using UnityEngine;

public static class MessageExtentions
{

    public static Message AddItemObject(Message message, ItemObject itemObject)
    {
        if (itemObject == null)
        {
            message.Add(-1);
            return message;
        }

        message.Add(itemObject.item.id);
        message.Add(itemObject.amount);

        return message;
    }

    public static Message AddInventory(Message message, Inventory inventory) => Add(message, inventory);

    public static Message Add(Message message, Inventory inventory)
    {
        message.Add(inventory.slotNumber);

        for(int i = 0; i < inventory.slotNumber; i++)
        {
            AddItemObject(message, inventory.slots[i]);
        }
        return message;
    }

    public static Message AddEquipment(Message message, Equipment equipment)
    {
        message.Add(equipment.slotNumber);

        for (int i = 0; i < equipment.slotNumber; i++)
        {
            AddItemObject(message, equipment.slots[i]);
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
        AddStructureObject(message, tile.structureObject);

        return message;
    }

    public static Message AddStructureObject(Message message, StructureObject structureObject)
    {
        if(structureObject == null)
        {
            message.Add(-1);
            return message;
        }

        message.Add(structureObject.structure.id);
        message.Add(structureObject.collapsed);
        message.Add(structureObject.health);
        message.Add(structureObject.collapsedHealth);

        return message;
    }

    public static Message AddPlayer(Message message, Player player)
    {
        message.Add(player.id);
        message.Add(player.name);
        message.Add(new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y));
        AddEquipment(message, player.clothes);
        AddEquipment(message, player.tools);

        return message;
    }

}
