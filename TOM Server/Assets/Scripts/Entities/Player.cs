using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System.Linq;

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
    public GameObject pivot;
    public Hit hit;
    public Inventory inventory;
    public Equipment clothes;
    public Equipment tools;
    public EquipmentType equipmentType;
    public Chunk currentChunk;
    public List<Chunk> chunksInRange;
    public static int BUFFER_SIZE = 1024;
    public Dictionary<ushort, Vector2> positionsInTime;

    public bool isChargingAttack = false;
    public static int CHARGE_TIME_IN_TICKS = 16;
    public Attack attack;

    public static Random random = new Random();

    private void Awake()
    {
        positionsInTime = new Dictionary<ushort, Vector2>();
    }

    private void Update()
    {
        UpdateChunk();
    }

    private void FixedUpdate()
    {
        if (positionsInTime.ContainsKey((ushort)(NetworkManager.Singleton.CurrentTick % BUFFER_SIZE)))
        {
            positionsInTime.Remove((ushort)(NetworkManager.Singleton.CurrentTick % BUFFER_SIZE));
        }
        positionsInTime.Add((ushort)(NetworkManager.Singleton.CurrentTick % BUFFER_SIZE), transform.position);

        if (isChargingAttack)
        {
            attack.remainingTicks--;
            if (attack.remainingTicks == 0) ExecuteAttack(NetworkManager.Singleton.CurrentTick);
        }
    }

    public Player(int _id, ushort _currectClientId, string _playerName, GameObject playerObject)
    {
        id = _id;
        currentClientId = _currectClientId;
        playerName = _playerName;
        pivot = playerObject.transform.GetChild(1).gameObject;
        hit = pivot.transform.GetChild(0).GetComponent<Hit>();
        inventory = new Inventory(id, 9);
        clothes = new Equipment(id, 3, new[] { Item.Type.Armor, Item.Type.Clothing });
        tools = new Equipment(id, 2, new[] { Item.Type.Weapon, Item.Type.Tool, Item.Type.Shield });
        chunksInRange = new List<Chunk>();
        currentChunk = MapUtil.GetChunk(playerObject.transform.position);

        inventory.Test();
        tools.Test();
        UpdateEquipmentStatus();
        UpdateChunk();
    }

    public void ChargeAttack(Vector2 direction, ushort tick)
    {
        Item weapon = tools.GetMainWeapon().item;

        if (weapon is MeleeWeaponItem) ChargeMeleeAttack(direction, tick, weapon as MeleeWeaponItem);
        else if (weapon is RangedWeaponItem) ChargeRangedAttack(direction, tick, weapon as RangedWeaponItem);
    }

    public void ChargeMeleeAttack(Vector2 direction, ushort tick, MeleeWeaponItem weapon)
    {
        if (isChargingAttack || equipmentType != EquipmentType.Armed) return;

        MeleeAttackTypes type = weapon.meleeAttackType;

        pivot.transform.right = direction;
        isChargingAttack = true;
        attack = new Attack(weapon, direction, CHARGE_TIME_IN_TICKS);

        hit.gameObject.SetActive(true);
        hit.EnableAttackCollider(type); //Here we choose the attack collider type based on the weapon used

        foreach (Player player in currentChunk.playersInRange)
        {
            PlayerSend.SendChargingAttackMessage(direction, id, player.currentClientId, tick);
        }
    }

    public void ChargeRangedAttack(Vector2 direction, ushort tick, RangedWeaponItem weapon)
    {
        if (isChargingAttack || equipmentType != EquipmentType.Armed) return;

        pivot.transform.right = direction;
        isChargingAttack = true;
        attack = new Attack(weapon, direction, CHARGE_TIME_IN_TICKS);

        foreach (Player player in currentChunk.playersInRange)
        {
            PlayerSend.SendChargingAttackMessage(direction, id, player.currentClientId, tick);
        }
    }

    public void ExecuteAttack(ushort tick)
    {
        isChargingAttack = false;

        if (attack.item is MeleeWeaponItem)
        {
            ExecuteMeleeAttack(tick);
        }
        else if (attack.item is RangedWeaponItem)
        {
            ExecuteRangedAttack(tick);
        }
    }

    public void ExecuteMeleeAttack(ushort tick)
    {
        Dash();
        AttackObjects();

        hit.DisableAttackCollider(((MeleeWeaponItem)attack.item).meleeAttackType);

        PlayerSend.SendExecutingAttackMessage(attack.direction, id, currentClientId, tick);
    }

    public void ExecuteRangedAttack(ushort tick)
    {
        GameObject projectile = Instantiate(PlayerManager.Singleton.projectilePrefab);
        projectile.transform.right = attack.direction;
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        RangedWeaponItem item = (RangedWeaponItem)tools.GetMainWeapon().item;
        projectileScript.ConfigureProjectile(10, item.damage, gameObject);

        PlayerSend.SendExecutingAttackMessage(attack.direction, id, currentClientId, tick);
    }

    private void Dash()
    {
        transform.position = transform.position + new Vector3(attack.direction.x, attack.direction.y, 0) * ((MeleeWeaponItem)attack.item).dash;
    }

    private void AttackObjects()
    {
        List<Collider2D> hitPlayerColliders = hit.hitPlayerColliders;
        List<Collider2D> hitStructureColliders = hit.hitStructureColliders;

        foreach (Collider2D playerCollider in hitPlayerColliders.ToArray())
        {
            playerCollider.gameObject.GetComponent<Player>().ReceiveMeleeAttack();
        }
        foreach (Collider2D structureCollider in hitStructureColliders.ToArray())
        {
            structureCollider.gameObject.GetComponent<StructureObject>().TakeDamage(PlayerUtil.CalculateDamage(this));
        }
    }

    private void BuildStructures()
    {
        List<Collider2D> hitStructureColliders = hit.hitStructureColliders;

        foreach (Collider2D structureCollider in hitStructureColliders.ToArray())
        {
            StructureObject hitStructure = structureCollider.gameObject.GetComponent<StructureObject>();
            hitStructure.MakeBuildingProgress(StructureUtil.CalculateBuildingProgress(this, hitStructure));
            PlayerSend.SendInventoryMessage(this, currentClientId, 0);
        }
    }

    public void ReceiveMeleeAttack()
    {
        Die();
    }

    public void UpdateEquipmentStatus()
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
                currentChunk.RemovePlayer(this);

            List<Chunk> previousChunks = new List<Chunk>(chunksInRange);

            chunksInRange.Clear();
            currentChunk = MapUtil.GetChunk(transform.position);
            currentChunk.AddPlayer(this);
            UpdateChunksInRange();

            foreach (Chunk newChunk in chunksInRange.Except(previousChunks).ToList())
            {
                MapSend.SendObjectsInChunk(newChunk, currentClientId);
            }
        }
        PlayerSend.SendPositionOfPlayersInRange(this, currentClientId);
    }

    private void UpdateChunksInRange()
    {
        int currentX = (int)(transform.position.x / Map.CHUNK_SIZE);
        int currectY = (int)(transform.position.y / Map.CHUNK_SIZE);

        for (int x = -Map.VIEW_RANGE; x <= Map.VIEW_RANGE; x++)
        {
            for (int y = -Map.VIEW_RANGE; y <= Map.VIEW_RANGE; y++)
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

    public void Die()
    {
        int randomSlot = random.Next(9);
        inventory.DropItem((int)transform.position.x, (int)transform.position.y, randomSlot, inventory.slots[randomSlot] != null ? inventory.slots[randomSlot].amount : 0);
        transform.position = new Vector3(1, 1, 0);
        PlayerSend.SendInventoryMessage(this, currentClientId, 1);
    }
}

public struct Attack {

    public Attack(Item item, Vector2 direction, int remainingTicks)
    {
        this.item = item;
        this.direction = direction;
        this.remainingTicks = remainingTicks;
    }

    public Item item;
    public Vector2 direction;
    public int remainingTicks;
}
