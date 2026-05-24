using System;
using UnityEngine;

public class ore : MonoBehaviour
{
    public bool canHitBoss = false;
    private Transform playerTrm;
    private Transform bossTrm;
    private Rigidbody2D rb;

    private float speed = 4.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Paring")
        {
            canHitBoss = true;
        }
        else if (collision.gameObject.tag == "Player")
        {
            BossScenePlayerHP _playerHp = collision.gameObject.GetComponentInChildren<BossScenePlayerHP>();
            _playerHp.TakeDamage(7);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        
        playerTrm = GameObject.FindGameObjectWithTag("Player").transform;
        bossTrm = GameObject.FindGameObjectWithTag("Boss").transform;

    }

    

    private void FixedUpdate()
    {
        if (canHitBoss)
        {
            Vector2 direction = bossTrm.position - transform.position;
            rb.linearVelocity = direction * speed;

        }
        else
        {
            Vector2 dir = (playerTrm.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
        }
        
        
    }
}
