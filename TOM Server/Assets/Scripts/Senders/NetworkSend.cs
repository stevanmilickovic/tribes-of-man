using RiptideNetworking;
using RiptideNetworking.Utils;

public static class NetworkSend
{
    public static void SendSync()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.sync);
        message.Add(NetworkManager.Singleton.CurrentTick);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

}