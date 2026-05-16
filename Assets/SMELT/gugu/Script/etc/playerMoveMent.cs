using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D Rigid;
    private Animator _anim;
    [SerializeField] private float speed = 7f;
    public Vector2 moveDir;

    private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
    private static readonly int HashMoveX    = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY    = Animator.StringToHash("MoveY");

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    public static bool IsLocked;

    private void Update()
    {
        if (_anim == null) return;

        bool moving = !IsLocked && moveDir.sqrMagnitude > 0f;
        _anim.SetBool(HashIsMoving, moving);

        if (moving)
        {
            _anim.SetFloat(HashMoveX, moveDir.x);
            _anim.SetFloat(HashMoveY, moveDir.y);
        }
    }

    private void FixedUpdate()
    {
        Rigid.linearVelocity = IsLocked ? Vector2.zero : moveDir * speed;
    }

    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }
}
