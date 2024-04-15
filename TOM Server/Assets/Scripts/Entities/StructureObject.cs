using UnityEngine;
public class StructureObject : MonoBehaviour
{

    public Structure structure;
    public int health;
    public bool collapsed;
    public int collapsedHealth;
    public Tile tile;

    public StructureObject(Structure _structure, bool _collapsed, int _health, int _collapsedHealth)
    {
        structure = _structure;
        health = _health;
        collapsed = _collapsed;
        collapsedHealth = _collapsedHealth;
    }

    public StructureObject(Structure _structure)
    {
        structure = _structure;
        collapsed = false;
        health = _structure.maxHealth;
        collapsedHealth = _structure.maxCollapsedHealth;
    }

    public void MakeBuildingProgress(int progress)
    {
        if (health == structure.maxHealth) return;
        if (collapsedHealth < structure.maxCollapsedHealth)
        {
            collapsedHealth += progress;
            progress = 0;
            if (collapsedHealth > structure.maxCollapsedHealth)
            {
                progress = collapsedHealth - structure.maxCollapsedHealth;
                collapsedHealth = structure.maxCollapsedHealth;
                collapsed = false;
            }
        }
        if (progress > 0)
        {
            health += progress;
            collapsed = health <= 0;
            if (health > structure.maxHealth) health = structure.maxHealth;
        }
        MapSend.SendTileMessageToPlayersInRange(tile);
    }

    public void TakeDamage(int damage)
    {
        if (!collapsed)
        {
            health -= damage;
            if (health <= 0)
            {
                if (structure.collapsable)
                {
                    health = 0;
                    collapsed = true;
                    gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                }
                else
                {
                    tile.DestroyStructure();
                    tile.itemObject = new ItemObject(structure.item, structure.itemAmount);
                }
            }
        } 
        else
        {
            collapsedHealth -= damage;
            if (collapsedHealth <= 0)
            {
                tile.DestroyStructure();
                tile.itemObject = new ItemObject(structure.item, structure.itemAmount);
            }
        }

        MapSend.SendTileMessageToPlayersInRange(tile);
    }
}
