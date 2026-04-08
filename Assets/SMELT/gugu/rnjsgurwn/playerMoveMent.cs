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

    //시작버튼을 누르면 플레이어가 한 번만 오른쪽으로 이동하기


    private void FixedUpdate()
    {
        //나의 위치가 이동한다.

        Rigid.linearVelocity = moveDir * speed;

    }

    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        
    }

   

}
