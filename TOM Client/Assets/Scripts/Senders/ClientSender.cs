using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiptideNetworking;

public static class ClientSender
{

    public static void SendNameMessage(string text)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.name);
        message.AddString(text);
        NetworkManager.Singleton.Client.Send(message);
    }

}
