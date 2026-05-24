using System;
using UnityEngine;

public class TutoAttack : MonoBehaviour
{
    public Collider2D otherCollider;
    private Rigidbody2D rb;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out rb))
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
            otherCollider = other;
            if(gameObject.CompareTag("RightHit"))
            {
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon3));
                TutoManager.Instance.TutoEnemySpawn.Spawn(false);
            }
            if(gameObject.CompareTag("LeftHit"))
            {
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon4));
            }
        }
    }
}
