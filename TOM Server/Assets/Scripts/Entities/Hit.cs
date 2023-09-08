using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    public PolygonCollider2D slashCollider;
    public PolygonCollider2D stabCollider;
    public PolygonCollider2D crushCollider;

    public List<Collider2D> hitPlayerColliders = new List<Collider2D>();
    public List<Collider2D> hitStructureColliders = new List<Collider2D>();

    private PolygonCollider2D GetAttackCollider(MeleeAttackTypes type)
    {
        switch (type)
        {
            case MeleeAttackTypes.Left: return slashCollider;
            case MeleeAttackTypes.Right: return slashCollider;
            case MeleeAttackTypes.Up: return crushCollider;
            default: return stabCollider;
        }
    }

    public void EnableAttackCollider(MeleeAttackTypes type)
    {
        PolygonCollider2D attackCollider = GetAttackCollider(type);
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider(MeleeAttackTypes type)
    {
        PolygonCollider2D attackCollider = GetAttackCollider(type);
        hitPlayerColliders = new List<Collider2D>();
        hitStructureColliders = new List<Collider2D>();
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && collider.gameObject != transform.parent.transform.parent.gameObject)
        {
            hitPlayerColliders.Add(collider);
        }
        if (collider.gameObject.tag == "Structure")
        {
            hitStructureColliders.Add(collider);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && collider.gameObject != transform.parent.transform.parent.gameObject)
        {
            hitPlayerColliders.Remove(collider);
        }
        if (collider.gameObject.tag == "Structure")
        {
            hitStructureColliders.Remove(collider);
        }
    }

}
