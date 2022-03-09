using RiptideNetworking;
using UnityEngine;

public static class InventoryHandle
{

    [MessageHandler((ushort)ServerToClientId.playerInventory)]
    public static void UpdateInventory(Message message)
    {
        PlayerManager.Singleton.UpdateInventory(MessageExtensions.GetInventory(message));
    }

}
