using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiptideNetworking;

public static class InventorySender
{

    public static void SendPickupMessage(int x, int y)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.pickup);
        message.Add(x);
        message.Add(y);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendMoveItemsMessage(ushort tick, int slot1, int amount, int slot2)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.moveItems);
        message.Add(tick);
        message.Add(slot1);
        message.Add(amount);
        message.Add(slot2);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendDropItemMessage(int slot, int amount)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.drop);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(amount);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendEatItemMessage(int slot)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.eat);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendCraftItemsMessage(int slot, int tileX, int tileY)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.craft);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(tileX);
        message.Add(tileY);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendBuildMessage(int slot, int tileX, int tileY)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.build);
        message.Add(NetworkManager.Singleton.ServerTick);
        message.Add(slot);
        message.Add(tileX);
        message.Add(tileY);
        NetworkManager.Singleton.Client.Send(message);
    }

}
