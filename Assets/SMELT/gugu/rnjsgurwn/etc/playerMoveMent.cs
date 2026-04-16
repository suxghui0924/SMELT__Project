using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D Rigid;
    [SerializeField] private float speed = 7f;
    public Vector2 moveDir;
   


    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        

    }

    


    private void FixedUpdate()
    {
       

        Rigid.linearVelocity = moveDir * speed;

    }

    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        
    }

   

}
