using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile : Enemy_Damage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private BoxCollider2D coll;

    private bool hit;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision); 
        coll.enabled = false;
        
            gameObject.SetActive(false); 
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
