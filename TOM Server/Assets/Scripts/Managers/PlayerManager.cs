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

    private void FixedUpdate()
    {

    }

    public void PlayerJoined(ushort clientId, string name)
    {
        NetworkManager.Singleton.SendSync();
        if (playersByName.ContainsKey(name)) 
        {
            UpdatePlayerClientId(playersByName[name], clientId);
        }
        else 
        { 
            SpawnNewPlayer(clientId, name);
        }

        PlayerSend.SendYourIdMessage(playersByName[name], clientId);
        PlayerSend.SendSpawnPlayerMessage(playersByName[name], clientId); // This player
        PlayerSend.SendInventoryMessage(playersByName[name], clientId, 1);
        PlayerSend.SendBasicRelevantInformation(playersByName[name], clientId);

        if (name == "admin")
            MapSend.SendAllChunks(clientId, MapManager.Singleton.map);
    }

    public void PlayerLeft(ushort clientId)
    {
        playersByClientId.Remove(clientId);
    }

    public void SpawnNewPlayer(ushort clientId, string name)
    {
        GameObject newPlayer = Instantiate(playerPrefab);
        newPlayer.name = name;
        newPlayer.transform.position = new Vector3(2f, 2f);

        Player newPlayerScript = SetNewPlayerScript(newPlayer, clientId, name);

        playersById.Add(playersById.Count, newPlayerScript);
        playersByName.Add(name, newPlayerScript);
        playersByClientId.Add(clientId, newPlayerScript);
    }

    private Player SetNewPlayerScript(GameObject newPlayer, ushort clientId, string username)
    {
        Player newPlayerScript = newPlayer.AddComponent<Player>();
        newPlayerScript.currentClientId = clientId;
        newPlayerScript.name = username;
        newPlayerScript.id = playersById.Count;
        newPlayerScript.pivot = newPlayer.transform.GetChild(1).gameObject;
        newPlayerScript.hit = newPlayerScript.pivot.transform.GetChild(0).GetComponent<Hit>();
        newPlayerScript.inventory = new Inventory(newPlayerScript.id, 9);
        newPlayerScript.clothes = new Equipment(newPlayerScript.id, 3, new[] { Item.Type.Armor, Item.Type.Clothing });
        newPlayerScript.tools = new Equipment(newPlayerScript.id, 2, new[] { Item.Type.Weapon, Item.Type.Tool, Item.Type.Shield });
        newPlayerScript.chunksInRange = new List<Chunk>();

        newPlayerScript.inventory.Test();
        newPlayerScript.tools.Test();
        newPlayerScript.CheckEquipment();
        newPlayerScript.UpdateChunk();

        return newPlayerScript;
    }

    private void UpdatePlayerClientId(Player player, ushort clientId)
    {
        player.currentClientId = clientId;
        playersByClientId.Add(clientId, player);
    }

    public void HandlePlayerInput(ushort fromClient, bool[] inputs)
    {
        Player player = playersByClientId[fromClient];
        player.gameObject.transform.Translate(GetInputDirection(inputs) * Time.deltaTime * 3);
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

    public void HandlePlayerMeleeAttack(ushort clientId, Vector2 direction, MeleeAttackTypes type, ushort tick)
    {
        Player player = playersByClientId[clientId];
        player.ChargeMeleeAttack(type, direction, tick);
    }
}
