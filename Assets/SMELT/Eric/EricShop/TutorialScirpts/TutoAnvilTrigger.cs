using System;
using UnityEngine;

public class TutoAnvilTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                if (!TutoManager.Instance.TutoSaying.canGetOrder1) return;
                TutoManager.Instance.TutoSaying.canGetOrder2 = true;    
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.getOrder2));
        }
    }
}
