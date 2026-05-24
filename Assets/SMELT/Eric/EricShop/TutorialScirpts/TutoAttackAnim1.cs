using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoAttackAnim1 : MonoBehaviour
{
    private Vector3 a;
    
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
            a = transform.localScale;
            transform.localScale = new Vector3(-a.x, a.y, a.z);
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            animator.SetTrigger("r");
            if(a.x < 0)
            {
                transform.localScale = new Vector3(-a.x, a.y, a.z);
            }
        }
    }
}
