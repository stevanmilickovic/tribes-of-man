using UnityEngine;
public class StructureObject : MonoBehaviour
{

    public Structure structure;
    public int health;
    public bool collapsed;
    public int collapsedHealth;

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

    public void TakeDamage(int damage)
    {

        Tile tile = MapUtil.GetTile(transform.position);

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
                }
            }
        } 
        else
        {
            collapsedHealth -= damage;
            if (collapsedHealth <= 0)
            {
                tile.DestroyStructure();
            }
        }

        MapSend.SendTileMessage(tile);
    }
}
