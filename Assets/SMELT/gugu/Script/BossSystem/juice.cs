using System;
using UnityEngine;
using System.Collections;

public class juice : MonoBehaviour
{
    public bool canHitBoss = false;
    private Transform playerTrm;
    private Transform bossTrm;
    private Rigidbody2D rb;
    private bool Attack = false;

    private float speed = 9f;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerTrm = GameObject.FindGameObjectWithTag("Player").transform;
        bossTrm = GameObject.FindGameObjectWithTag("Boss").transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paring"))
        {
            canHitBoss = true;
        }
    }

   

    private void FixedUpdate()
    {
        if (canHitBoss)
        {
            Vector2 direction = (bossTrm.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            Vector2 dir = (playerTrm.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
        }
    }
}
