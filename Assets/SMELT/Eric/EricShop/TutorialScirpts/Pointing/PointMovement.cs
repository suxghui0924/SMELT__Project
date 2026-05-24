using System;
using UnityEngine;

public class PointMovement : MonoBehaviour
{
    
    [SerializeField] private Vector3 target;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject temp = GameObject.Find("Target");
        target = temp.transform.position;
    }

    private void OnDisable()
    {
        transform.position = new Vector3(0,0,0);
    }

    private void Update()
    {
        Debug.Log(Time.timeScale);
        rb.linearVelocity = ((target - transform.position))* speed;
    }
}
