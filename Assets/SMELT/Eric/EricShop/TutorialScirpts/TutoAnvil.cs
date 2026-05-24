using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoAnvil : MonoBehaviour
{
    [SerializeField] private GameObject craftUI;
    private bool canOpen = false;
    [SerializeField] private GameObject[] playerHouse;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            StartCoroutine(Coroutine());
        }
        canOpen = true;
    }

    private IEnumerator Coroutine()
    {
            playerHouse[0].SetActive(true);
        yield return new  WaitForSeconds(2.3f);
        playerHouse[0].SetActive(true);
    }

    private void Update()
    {
        if (playerHouse[0] == null) playerHouse[0] = GameObject.Find("Player");
        craftUI = GameObject.Find("WeaponCraftUI");
        if(craftUI == null) return;
        if (craftUI.activeSelf)
        {
            if (TutoManager.Instance.TutoSaying.makeWeapon1&&!TutoManager.Instance.TutoSaying.canGetOrder2)
            {
                TutoManager.Instance.TutoSaying.makeWeapon1 = false;
                StartCoroutine(
                    TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.makeWeapon1));
            }
        }
    }
}
