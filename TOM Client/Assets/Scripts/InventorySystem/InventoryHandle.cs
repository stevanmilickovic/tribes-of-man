using RiptideNetworking;
using UnityEngine;

public static class InventoryHandle
{

    [MessageHandler((ushort)ServerToClientId.playerInventory)]
    public static void UpdateInventory(Message message)
    {
        InventoryManager.UpdateInventoryIfNecessary(message.GetUShort(), MessageExtensions.GetInventory(message), MessageExtensions.GetClothes(message), MessageExtensions.GetTools(message));
        DisplayInventory.Singleton.UpdateInventory();
    }

    [MessageHandler((ushort)ServerToClientId.playerEquipment)]
    public static void UpdatePlayerEquipment(Message message)
    {
        int playerId = message.GetInt();
        Equipment clothes = MessageExtensions.GetClothes(message);
        Equipment tools = MessageExtensions.GetTools(message);
        PlayerManager.Singleton.UpdatePlayerEquipment(playerId, clothes, tools);
    }

}
