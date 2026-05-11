using UnityEngine;
using System.Collections;

public class juice : MonoBehaviour
{
    private Transform playerTrm;

    private Rigidbody2D rb;

    private float speed = 9f;


    private void Awake()
    {
        playerTrm = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 dir = (playerTrm.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }
}
