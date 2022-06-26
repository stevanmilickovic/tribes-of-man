using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using System;
using TMPro;
using UnityEngine.EventSystems;

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
    private GameObject hit;
    [SerializeField] private Interpolator interpolator;
    public MyPlayer myPlayer;
    public Dictionary<int, Player> players;
    public int myId = -1;
    private bool[] inputs = new bool[4];

    public Camera mainCamera;
    public bool cinematicMode;

    private void Awake()
    {
        Singleton = this;
        players = new Dictionary<int, Player>();
    }

    private void Update()
    {
        Aim();
        HandleInputs();
    }

    private void HandleInputs()
    {
        GetInputs();
        if (Input.GetKeyDown(KeyCode.Mouse1) && !EventSystem.current.IsPointerOverGameObject())
            Pickup();
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
            Attact();
    }

    private void Aim()
    {
        if (hit == null) return;
        Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2)hit.transform.position).normalized;
        hit.transform.right = direction;
    }

    private void Attact()
    {
        if (hit == null) return;
        Vector2 direction = hit.transform.right;
        ushort tick = NetworkManager.Singleton.ServerTick;

        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.attack);
        message.Add(direction);
        message.Add(tick);
        NetworkManager.Singleton.Client.Send(message);
    }

    private void FixedUpdate()
    {
        if (HasInput())
        {
            SendInputs();
        }
        RefreshInputs();
    }

    public void SpawnPlayer(int id, string username, Vector2 position, int health, Equipment clothes, Equipment tools)
    {
        if(id == myId)
        {
            InstantiatePlayer(myPlayerPrefab, id, username, position, health, clothes, tools);
        }
        else
        {
            InstantiatePlayer(playerPrefab, id, username, position, health, clothes, tools);
        }
    }

    private void InstantiatePlayer(GameObject playerObject ,int id, string username, Vector2 position, int health, Equipment clothes, Equipment tools)
    {

        if (players.ContainsKey(id)) return;

        GameObject player = Instantiate(playerObject);
        player.transform.position = new Vector3(position.x, position.y, 0f);
        player.name = username;
        Player playerScript = SetPlayerScript(player, id, username, health, clothes, tools);
        playerScript.nameText.GetComponent<TextMeshPro>().text = username;
        playerScript.CheckEquipment();
        players.Add(id, playerScript);

        if (id != myId)
            MapUtil.GetChunk(position).players.Add(id, playerScript);
    }

    private Player SetPlayerScript(GameObject newPlayer, int id, string username, int health, Equipment clothes, Equipment tools)
    {
        if (id == myId)
            return SetMyPlayerScript(newPlayer, id, username, clothes, tools);

        Player newPlayerScript = newPlayer.AddComponent<Player>();
        newPlayerScript.username = username;
        newPlayerScript.id = id;
        newPlayerScript.health = health;
        newPlayerScript.clothes = clothes;
        newPlayerScript.tools = tools;

        newPlayerScript.nameText = newPlayer.transform.GetChild(1).gameObject;
        newPlayerScript.healthText = newPlayer.transform.GetChild(2).gameObject;
        newPlayerScript.leftArm = newPlayer.transform.GetChild(3).gameObject;
        newPlayerScript.rightArm = newPlayer.transform.GetChild(4).gameObject;


        return newPlayerScript;
    }

    private Player SetMyPlayerScript(GameObject newPlayer, int id, string username, Equipment clothes, Equipment tools)
    {
        MyPlayer newPlayerScript = newPlayer.AddComponent<MyPlayer>();
        newPlayerScript.username = username;
        newPlayerScript.id = id;
        newPlayerScript.clothes = clothes;
        newPlayerScript.tools = tools;

        newPlayerScript.nameText = newPlayer.transform.GetChild(2).gameObject;
        newPlayerScript.healthText = newPlayer.transform.GetChild(3).gameObject;
        newPlayerScript.leftArm = newPlayer.transform.GetChild(4).gameObject;
        newPlayerScript.rightArm = newPlayer.transform.GetChild(5).gameObject;

        myPlayer = newPlayerScript;
        if (!cinematicMode)
        {
            mainCamera.transform.SetParent(newPlayer.transform);
            mainCamera.transform.position = newPlayer.transform.position;
        }
        newPlayer.GetComponentInChildren<Transform>().position = newPlayer.transform.position; //Possible difficulties with children position

        hit = newPlayer.transform.GetChild(1).gameObject;

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
            int health = players[id].health;
            Equipment clothes = players[id].clothes;
            Equipment tools = players[id].tools;

            Destroy(oldPlayer);
            players.Remove(id);

            SpawnPlayer(id, username, position, health, clothes, tools);
        }
    }

    public void UpdatePlayerPosition(int id, ushort tick, Vector2 position)
    {
        if(players.TryGetValue(id, out Player player) && id != myId)
            player.Move(tick, position);
        else if (id == myId)
        {
            myPlayer.Reconciliate(tick, position);
        }
    }

    public void UpdatePlayerHealth(int id, int health)
    {
        if (players.TryGetValue(id, out Player player) && id != myId)
        {
            player.health = health;
            player.healthText.GetComponent<TextMeshPro>().text = health.ToString();
        }
        else if (id == myId)
        {
            myPlayer.health = health;
            myPlayer.healthText.GetComponent<TextMeshPro>().text = health.ToString();
        }
    }

    public void UpdatePlayerEquipment(int id, Equipment clothes, Equipment tools)
    {
        if (players.TryGetValue(id, out Player player) && id != myId)
        {
            player.clothes = clothes;
            player.tools = tools;
            player.CheckEquipment();
        }
    }

    #region Inventory

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
    }

    public void MoveItems(ushort tick, int slot1, int amount, int slot2)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.moveItems);
        message.Add(tick);
        message.Add(slot1);
        message.Add(amount);
        message.Add(slot2);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void DropItem(int slot, int amount)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.drop);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(amount);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void CraftItems(int slot, int tileX, int tileY)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.craft);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(tileX);
        message.Add(tileY);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void Build(int slot, int tileX, int tileY)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.build);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(tileX);
        message.Add(tileY);
        NetworkManager.Singleton.Client.Send(message);
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
