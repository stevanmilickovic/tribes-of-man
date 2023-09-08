using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public enum ClientToServerId : ushort
{
    join = 1,
    input,
    pickup,
    craft,
    moveItems,
    drop,
    meleeAttack,
    build,
    eat
}

public enum ServerToClientId : ushort
{
    sync = 1,
    spawnPlayer,
    yourPlayerId,
    playerPosition,
    chunk,
    tile,
    playerInventory,
    playerEquipment,
    chargingMeleeAttack,
    executingMeleeAttack
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager singleton;

    public static NetworkManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }
    public ushort CurrentTick { get; private set; } = 0;

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake()
    {
        Singleton = this;
        Application.runInBackground = true;
        Application.targetFrameRate = 32;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    private void FixedUpdate()
    {
        Server.Tick();

        if (CurrentTick % 160 == 0)
            SendSync();

        CurrentTick++;
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        PlayerManager.Singleton.PlayerLeft(e.Id);
    }

    public void SendSync()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.sync);
        message.Add(CurrentTick);
        Server.SendToAll(message);
    }
}
