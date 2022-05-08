using RiptideNetworking;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager singleton;

    public static PlayerManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(PlayerManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Dictionary<int, Player> playersById;
    public Dictionary<string, Player> playersByName;
    public Dictionary<ushort, Player> playersByClientId;
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Singleton = this;
        playersById = new Dictionary<int, Player>();
        playersByName = new Dictionary<string, Player>();
        playersByClientId = new Dictionary<ushort, Player>();
    }

    private void Start()
    {

    }


    private void FixedUpdate()
    {
        SendAllPlayerPositions();
    }

    public void PlayerJoined(ushort clientId, string name)
    {
        NetworkManager.Singleton.SendSync();
        if (playersByName.ContainsKey(name)) 
        {
            UpdatePlayer(playersByName[name], clientId);
            SendYourIdMessage(playersByName[name], clientId);
            SpawnAllPlayers(clientId);
            SendInventoryMessage(playersByName[name], clientId);
        }
        else { SpawnNewPlayer(clientId, name); }
        MapManager.Singleton.SendAllChunks(clientId);
    }

    public void PlayerLeft(ushort clientId)
    {
        playersByClientId.Remove(clientId);
    }

    public void SpawnNewPlayer(ushort clientId, string name)
    {
        SpawnAllPlayers(clientId);

        GameObject newPlayer = Instantiate(playerPrefab);
        newPlayer.name = name;

        Player newPlayerScript = SetNewPlayerScript(newPlayer, clientId, name);

        playersById.Add(playersById.Count, newPlayerScript);
        playersByName.Add(name, newPlayerScript);
        playersByClientId.Add(clientId, newPlayerScript);

        SendYourIdMessage(newPlayerScript, clientId);
        SendSpawnPlayerMessageToAll(newPlayerScript);
        SendInventoryMessage(newPlayerScript, clientId);
    }

    private Player SetNewPlayerScript(GameObject newPlayer, ushort clientId, string username)
    {
        Player newPlayerScript = newPlayer.AddComponent<Player>();
        newPlayerScript.currectClientId = clientId;
        newPlayerScript.name = username;
        newPlayerScript.id = playersById.Count;
        newPlayerScript.inventory = new Inventory(newPlayerScript.id, 9);
        newPlayerScript.clothes = new Equipment(newPlayerScript.id, 3, new[] { Item.Type.Armor, Item.Type.Clothing });
        newPlayerScript.tools = new Equipment(newPlayerScript.id, 2, new[] { Item.Type.Weapon, Item.Type.Tool, Item.Type.Shield });

        newPlayerScript.inventory.Test();
        newPlayerScript.tools.Test();

        return newPlayerScript;
    }

    public void SendInventoryMessage(Player player, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.playerInventory);
        MessageExtentions.Add(message, player.inventory);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    private void SendYourIdMessage(Player player, ushort clientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.yourPlayerId);
        message.Add(player.id);
        NetworkManager.Singleton.Server.Send(message, clientId);
    }

    private void SendSpawnPlayerMessageToAll(Player player)
    {
        NetworkManager.Singleton.Server.SendToAll(PackPlayerInMessage(player));
    }

    private void SendSpawnPlayerMessage(Player player, ushort clientId)
    {
        NetworkManager.Singleton.Server.Send(PackPlayerInMessage(player), clientId);
    }

    private Message PackPlayerInMessage(Player player)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.spawnPlayer);
        MessageExtentions.AddPlayer(message, player);
        
        return message;
    }

    private void SpawnAllPlayers(ushort clientId)
    {
        foreach (Player player in playersById.Values)
        {
            SendSpawnPlayerMessage(player, clientId);
        }
    }

    private void UpdatePlayer(Player player, ushort clientId)
    {
        player.currectClientId = clientId;
        playersByClientId.Add(clientId, player);
    }

    public void PlayerInput(ushort fromClient, bool[] inputs)
    {
        Player player = playersByClientId[fromClient];
        player.gameObject.transform.Translate(GetInputDirection(inputs) * Time.deltaTime * 3);
    }

    public void SendAllPlayerPositions()
    {
        foreach(Player player in playersById.Values)
        {
            SendPlayerPosition(player);
        }
    }

    private Vector2 GetInputDirection(bool[] inputs)
    {
        Vector2 inputDirection = Vector2.zero;
        if (inputs[0])
            inputDirection.y += 1;
        if (inputs[1])
            inputDirection.x -= 1;
        if (inputs[2])
            inputDirection.y -= 1;
        if (inputs[3])
            inputDirection.x += 1;
        return inputDirection;
    }

    private void SendPlayerPosition(Player player)
    {

        if (NetworkManager.Singleton.CurrentTick % 2 != 0)
            return;

        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerPosition);
        message.AddInt(player.id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector2(new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y));
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void Pickup(ushort clientId, int x, int y)
    {
        Tile tile = MapManager.Singleton.map.tiles[x, y];
        if(tile.itemObject != null)
        {
            if(playersByClientId[clientId].inventory.Add(tile.itemObject))
                tile.itemObject = null;
        }
        SendInventoryMessage(playersByClientId[clientId], clientId);
        MapManager.Singleton.SendTileMessage(tile);
    }

    public void SwapItems(ushort fromClientId, int slot1, int slot2)
    {
        Inventory inventory1 = GetInventory(slot1, fromClientId);
        Inventory inventory2 = GetInventory(slot2, fromClientId);

        ItemObject item1 = inventory1.slots[GetSlotNumber(slot1)];
        ItemObject item2 = inventory2.slots[GetSlotNumber(slot2)];

        if (!inventory1.CanAddItem(item2) || !inventory2.CanAddItem(item1))
        {
            SendInventoryMessage(playersByClientId[fromClientId], fromClientId);
            return;
        }

        inventory1.slots[GetSlotNumber(slot1)] = item2;
        inventory2.slots[GetSlotNumber(slot2)] = item1;

        SendInventoryMessage(playersByClientId[fromClientId], fromClientId);
    }

    private Inventory GetInventory(int i, ushort fromClientId)
    {
        if (i < 9)
            return playersByClientId[fromClientId].inventory;
        else if (i < 12)
            return playersByClientId[fromClientId].clothes;
        else
            return playersByClientId[fromClientId].tools;
    }

    private int GetSlotNumber(int i)
    {
        if (i < 9)
            return i;
        else if (i < 12)
            return i - 9;
        else
            return i - 12;
    }

    public void DropItem(ushort clientId, int slot)
    {
        Player player = playersByClientId[clientId];
        ItemObject itemObject = player.inventory.slots[slot];
        if (itemObject == null)
            return;

        Tile tile = MapManager.Singleton.DropItem((int)player.gameObject.transform.position.x, (int)player.gameObject.transform.position.y, itemObject);

        if (tile != null)
        {
            player.inventory.slots[slot] = null;
            MapManager.Singleton.SendTileMessage(tile);
        }
        SendInventoryMessage(player, clientId);
    }

    public void CraftItems(ushort clientId, int slot, int tileX, int tileY)
    {

        Crafting.Singleton.Craft(clientId, slot, tileX, tileY);
    }
}
