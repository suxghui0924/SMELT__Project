using System;
using UnityEngine;

public class TutoOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TutoManager.Instance.Triggered();
            Debug.Log(other.name);
            StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon2));
        }
    }
}
