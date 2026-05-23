using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// 플레이어 이동 + 구역 감지.
/// WASD / 방향키로 이동, E키로 상호작용.
///
/// [Animator 파라미터]
///   IsMoving  (bool)  — 이동 중 여부
///   MoveX     (float) — 수평 방향 (-1 / 0 / 1)
///   MoveY     (float) — 수직 방향 (-1 / 0 / 1)
/// </summary>
public class PrototypePlayer : MonoBehaviour
{
    private const float SPEED = 5f;

    // Y축 정렬 기준값 — 이 값에서 Y 좌표를 빼서 sortingOrder 결정
    // UI는 별도 "UI" 레이어이므로 sortingOrder 값에 관계없이 UI가 항상 위에 그려짐
    private const int SORT_BASE = 100;
            
    private Rigidbody2D   _rb;
    private Animator      _anim;
    private SpriteRenderer _sr;
    private PrototypeZone _currentZone;
    private PrototypeZone _openDoorZone;

    // Animator 파라미터 해시 (문자열 조회보다 빠름)
    private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
    private static readonly int HashMoveX    = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY    = Animator.StringToHash("MoveY");

    private void Start()
    {
        _rb   = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr   = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (PlayerMovement.IsLocked)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_anim != null) _anim.SetBool(HashIsMoving, false);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        var dir = new Vector2(h, v).normalized;

        _rb.linearVelocity = dir * SPEED;

        // 애니메이션 구동
        if (_anim != null)
        {
            bool moving = dir.sqrMagnitude > 0f;
            _anim.SetBool(HashIsMoving, moving);

            if (moving)
            {
                _anim.SetFloat(HashMoveX, h);
                _anim.SetFloat(HashMoveY, v);
            }
        }

        // 좌우 이동 시 스프라이트 반전
        if (_sr != null && h != 0f)
            _sr.flipX = h < 0f;

        // Y축 기준 정렬 — Y가 낮을수록(화면 아래) 앞에 그려짐
        if (_sr != null)
            _sr.sortingOrder = SORT_BASE - Mathf.RoundToInt(transform.position.y * 10);

        // 상호작용 (대장간 / 상점)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_currentZone != null)
                _currentZone.Interact();
            else if (_openDoorZone != null)
            {
                _openDoorZone.Interact();
                _openDoorZone = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var zone = other.GetComponent<PrototypeZone>();
        if (zone == null) return;
        _currentZone = zone;
        zone.OnPlayerEnter();
        if (zone.ZoneType == ZoneType.Door)
            _openDoorZone = zone;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var zone = other.GetComponent<PrototypeZone>();
        if (zone == null || zone != _currentZone) return;
        zone.OnPlayerExit();
        _currentZone = null;
    }
}
