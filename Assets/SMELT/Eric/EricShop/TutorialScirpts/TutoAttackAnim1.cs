using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoAttackAnim1 : MonoBehaviour
{
    
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            animator.SetTrigger("l");
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            animator.SetTrigger("r");
            
        }
    }
}
