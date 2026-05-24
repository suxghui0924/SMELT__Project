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

    private void OnEnable()
    {
        craftUI = GameObject.Find("WeaponCraftUI");
    }

    private void Update()
    {
        if (!canOpen && Keyboard.current.eKey.wasPressedThisFrame)
        {
            craftUI.SetActive(true);
        }
    }
}
