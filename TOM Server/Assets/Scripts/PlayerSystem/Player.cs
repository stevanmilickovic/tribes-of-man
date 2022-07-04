using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Player : MonoBehaviour
{
    public enum EquipmentType
    {
        Unarmed,
        Armed,
        HoldingTool,
        Other
    }

    public int id;
    public ushort currentClientId;
    public string playerName;
    public int health;
    public int hunger;
    public bool malnourished = false;
    public GameObject pivot;
    public Hit hit;
    public Inventory inventory;
    public Equipment clothes;
    public Equipment tools;
    public EquipmentType equipmentType;
    public Chunk currentChunk;
    public List<Chunk> chunksInRange;

    public static Random random = new Random();

    private void Update()
    {
        UpdateChunk();
    }

    public Player(int _id, ushort _currectClientId, string _playerName, GameObject playerObject)
    {
        id = _id;
        currentClientId = _currectClientId;
        playerName = _playerName;
        health = 100;
        pivot = playerObject.transform.GetChild(1).gameObject;
        hit = pivot.transform.GetChild(0).GetComponent<Hit>();
        inventory = new Inventory(id, 9);
        clothes = new Equipment(id, 3, new[] { Item.Type.Armor, Item.Type.Clothing });
        tools = new Equipment(id, 2, new[] { Item.Type.Weapon, Item.Type.Tool, Item.Type.Shield });
        chunksInRange = new List<Chunk>();
        currentChunk = MapUtil.GetChunk(playerObject.transform.position);

        inventory.Test();
        tools.Test();
        CheckEquipment();
        UpdateChunk();
    }

    public void Attack(Vector2 direction)
    {
        hit.gameObject.SetActive(true);
        hit.ExecuteHit(direction, this);
    }

    public void CheckEquipment()
    {
        equipmentType = EquipmentType.Unarmed;
        foreach (ItemObject slot in tools.slots)
        {
            if (slot != null && slot.item.type == Item.Type.Weapon)
            {
                equipmentType = EquipmentType.Armed;
                break;
            }
            if (slot != null && slot.item.type == Item.Type.Tool)
            {
                equipmentType = EquipmentType.HoldingTool;
            }
        }
    }

    public void UpdateChunk()
    {
        if (currentChunk != MapUtil.GetChunk(transform.position))
        {

            if (currentChunk != null)
            {
                currentChunk.RemovePlayer(this);
            }
            foreach (Chunk chunk in chunksInRange)
            {
                chunk.playersInRange.Remove(this);
            }
            chunksInRange = new List<Chunk>();

            currentChunk = MapUtil.GetChunk(transform.position);
            currentChunk.AddPlayer(this);
            UpdateChunksInRange();

            PlayerSend.SendBasicRelevantInformation(this, currentClientId);
        }
        PlayerSend.SendRelevantPlayerPosition(this, currentClientId);
    }

    private void UpdateChunksInRange()
    {
        int currentX = (int)(transform.position.x / 10);
        int currectY = (int)(transform.position.y / 10);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Chunk chunk = MapUtil.GetChunk(currentX + x, currectY + y);
                if (chunk != null)
                {
                    chunk.playersInRange.Add(this);
                    chunksInRange.Add(chunk);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            int randomSlot = random.Next(9);
            inventory.DropItem((int)transform.position.x, (int)transform.position.y, randomSlot, inventory.slots[randomSlot].amount);
            transform.position = new Vector3(1, 1, 0);
            health = 100;
        }
        PlayerSend.SendInventoryMessage(this, currentClientId, 1);
    }
}
