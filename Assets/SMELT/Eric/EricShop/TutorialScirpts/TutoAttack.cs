using System;
using UnityEngine;

public class TutoAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(gameObject.CompareTag("RightHit"))
            {
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon3));
            }
            if(gameObject.CompareTag("LeftHit"))  StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon4));

        }
    }
}
