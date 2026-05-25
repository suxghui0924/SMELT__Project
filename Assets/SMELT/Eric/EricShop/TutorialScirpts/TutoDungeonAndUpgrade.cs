using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoDungeonAndUpgrade : MonoBehaviour
{
    private bool _canTouch;
    [SerializeField] private GameObject[] mining;
    [SerializeField] private GameObject[] playerHouse;
    [SerializeField] private bool canSpawn = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("EnterDungeon"))
            {
                TutoManager.Instance.Triggered();
                
                StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.enterDungeon1));
            }
            _canTouch = true;
        }
    }

    private void Update()
    {
        if (playerHouse[1] == null) playerHouse[1] = GameObject.Find("Player");
        if (_canTouch)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame&&TutoManager.Instance.Dialogue.canMove&&canSpawn)
            {
                if (gameObject.CompareTag("EnterDungeon"))
                {
                    TutoManager.Instance.Dialogue.canMakePoint = true;
                    StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.dungeon1));
                    playerHouse[0].gameObject.SetActive(false);
                    playerHouse[1].gameObject.SetActive(false);
                    mining[0].gameObject.SetActive(true);
                    mining[1].gameObject.SetActive(true);
                    canSpawn =  false;
                }
            }
        }
    }
}
