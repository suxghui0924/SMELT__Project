using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoDungeonAndUpgrade : MonoBehaviour
{
    private bool _canTouch;
    [SerializeField] private GameObject mining;
    [SerializeField] private GameObject playerHouse;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("EnterDungeon"))
            {
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.enterDungeon1));
            }
            else if (gameObject.CompareTag("EnterUpgrade"))
            {
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.store2));
            }
            _canTouch = true;
        }
    }

    private void Update()
    {
        if (_canTouch)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame&&TutoManager.Instance.Dialogue.canMove)
            {
                if (gameObject.CompareTag("EnterDungeon"))
                {
                    StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon1));
                    playerHouse.gameObject.SetActive(false);
                    mining.gameObject.SetActive(true);
                    TutoManager.Instance.TutoEnemySpawn.Spawn(true);
                }
                else if (gameObject.CompareTag("EnterUpgrade"))
                {
                    
                }
            }
        }
    }
}
