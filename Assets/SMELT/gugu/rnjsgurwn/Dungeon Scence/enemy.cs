using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform playerTrm;

    private Rigidbody2D rb;

    private float speed = 5f;
    

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }
    void OnDestroy()
    {        
        TimerAndReward.Instance.ReduceFatigue(2);       
    }

}
