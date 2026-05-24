using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoAnvil : MonoBehaviour
{
    [SerializeField] private GameObject craftUI;
    private bool canOpen = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        canOpen = true;
    }
    private void Update()
    {
        craftUI = GameObject.Find("WeaponCraftUI");
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
