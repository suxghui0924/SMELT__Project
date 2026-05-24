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
        a = transform.localScale;
    }

    private void Start()
    {
        a = transform.localScale;
    }

    private void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            animator.SetTrigger("l");
            transform.localScale = new Vector3(a.x, a.y, a.z);
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            animator.SetTrigger("r");
                transform.localScale = new Vector3(-a.x, a.y, a.z);
        }
    }
}
