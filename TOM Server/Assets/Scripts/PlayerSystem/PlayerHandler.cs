using RiptideNetworking;

public static class PlayerHandler
{

    [MessageHandler((ushort)ClientToServerId.join)]
    public static void PlayerJoined(ushort fromClientId, Message message)
    {
        PlayerManager.Singleton.PlayerJoined(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    public static void PlayerInput(ushort fromClientId, Message message)
    {
        bool[] inputs = message.GetBools();
        PlayerManager.Singleton.PlayerInput(fromClientId, inputs);
    }

}
