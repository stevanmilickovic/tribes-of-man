using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

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
    private MyPlayer myPlayer;
    private Dictionary<int, Player> players;
    private int myId = -1;
    private bool[] inputs = new bool[4];

    private void Awake()
    {
        Singleton = this;
        players = new Dictionary<int, Player>();
    }

    private void Update()
    {
        GetInputs();
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

    public void UpdateInventory(Inventory inventory)
    {
        myPlayer.inventory = inventory;

        foreach(ItemObject item in inventory.slots)
        {
            if(item != null)
                Debug.Log(item.item.name);
        }
    }

    public void UpdatePlayerPosition(int id, Vector2 position)
    {
        if(players.TryGetValue(id, out Player player))
        {
            player.gameObject.transform.position = new Vector3(position.x, position.y, 0f);
        }
    }

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
}
