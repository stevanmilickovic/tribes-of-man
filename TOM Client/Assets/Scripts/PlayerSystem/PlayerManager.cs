using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using System;

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

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject myPlayerPrefab;
    public MyPlayer myPlayer;
    public Dictionary<int, Player> players;
    public int myId = -1;
    private bool[] inputs = new bool[4];
    private int awaitedInventoryUpdates = 0;

    public Camera mainCamera;

    private void Awake()
    {
        Singleton = this;
        players = new Dictionary<int, Player>();
    }

    private void Update()
    {
        GetInputs();
        if (Input.GetKey(KeyCode.Mouse1))
            Pickup();
    }

    private void FixedUpdate()
    {
        if (HasInput())
        {
            SendInputs();
        }
        RefreshInputs();
    }

    public void SpawnPlayer(int id, string username, Vector2 position)
    {
        if(id == myId)
        {
            InstantiatePlayer(myPlayerPrefab, id, username, position);
        }
        else
        {
            InstantiatePlayer(playerPrefab, id, username, position);
        }
    }

    private void InstantiatePlayer(GameObject playerObject ,int id, string username, Vector2 position)
    {
        GameObject player = Instantiate(playerObject);
        player.transform.position.Set(position.x, position.y, 0f);
        player.name = username;
        Player playerScript = SetPlayerScript(player, id, username);
        players.Add(id, playerScript);
    }

    private Player SetPlayerScript(GameObject newPlayer, int id, string username)
    {
        if (id == myId)
            return SetMyPlayerScript(newPlayer, id, username);
        Player newPlayerScript = newPlayer.AddComponent<Player>();
        newPlayerScript.username = username;
        newPlayerScript.id = id;
        return newPlayerScript;
    }

    private Player SetMyPlayerScript(GameObject newPlayer, int id, string username)
    {
        MyPlayer newPlayerScript = newPlayer.AddComponent<MyPlayer>();
        newPlayerScript.username = username;
        newPlayerScript.id = id;
        myPlayer = newPlayerScript;
        mainCamera.transform.SetParent(newPlayer.transform);
        return newPlayerScript;
    }

    public void SetMyId(int id)
    {
        myId = id;
        if(players.ContainsKey(id))
        {
            GameObject oldPlayer = players[id].gameObject;
            Vector2 position = new Vector2(oldPlayer.transform.position.x, oldPlayer.transform.position.y);
            string username = players[id].username;
            Destroy(oldPlayer);
            players.Remove(id);
            SpawnPlayer(id, username, position);
        }
    }

    public void UpdatePlayerPosition(int id, Vector2 position)
    {
        if(players.TryGetValue(id, out Player player))
        {
            player.gameObject.transform.position = new Vector3(position.x, position.y, 0f);
        }
    }

    #region Inventory

    public void UpdateInventory(Inventory inventory)
    {
        if(awaitedInventoryUpdates <= 1)
            myPlayer.inventory = inventory;
        awaitedInventoryUpdates--;
    }

    public void Pickup()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)mousePosition.x;
        int y = (int)mousePosition.y;
        if (MapManager.Singleton.tiles[(x, y)].itemObject != null)
        {
            SendPickupMessage(x, y);
        }
    }

    public void SendPickupMessage(int x, int y)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.pickup);
        message.Add(x);
        message.Add(y);
        NetworkManager.Singleton.Client.Send(message);
        awaitedInventoryUpdates++;
    }

    public void SwapItems(int slot1, int slot2)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.swap);
        message.Add(slot1);
        message.Add(slot2);
        NetworkManager.Singleton.Client.Send(message);
        awaitedInventoryUpdates++;
    }

    public void DropItem(int slot)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.drop);
        message.Add(slot);
        NetworkManager.Singleton.Client.Send(message);
        awaitedInventoryUpdates++;
    }

    #endregion

    #region Movement Inputs
    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs);
        NetworkManager.Singleton.Client.Send(message);
    }

    private void GetInputs()
    {
        if(Input.GetKey(KeyCode.W))
        {
            inputs[0] = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputs[1] = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputs[2] = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputs[3] = true;
        }
    }

    private void RefreshInputs()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    private bool HasInput() //Returns true if at least one input is pressed
    {
        foreach(bool input in inputs)
        {
            if(input)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}
