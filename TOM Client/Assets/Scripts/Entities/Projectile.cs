using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public SpriteRenderer spriteRenderer;
    public GameObject player;

    public void ConfigureProjectile(float _speed, int _damage, Sprite sprite, GameObject _player)
    {
        speed = _speed;
        damage = _damage;
        spriteRenderer.sprite = sprite;
        player = _player;
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "myPlayer")
        {
            Destroy(gameObject);
        }
    }
}
