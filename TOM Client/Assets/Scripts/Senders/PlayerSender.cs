using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public static class PlayerSender
{
    
    public static void SendAttackMessage(Vector2 direction)
    {
        ushort tick = NetworkManager.Singleton.ServerTick;

        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.meleeAttack);
        message.Add(direction);
        message.Add(tick);
        NetworkManager.Singleton.Client.Send(message);
    }

    public static void SendInputs(bool[] inputs)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs);
        NetworkManager.Singleton.Client.Send(message);
    }

}
