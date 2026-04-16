using UnityEngine;

/// <summary>
/// 플레이어 이동 + 구역 감지.
/// WASD / 방향키로 이동, E키로 상호작용.
/// </summary>
public class PrototypePlayer : MonoBehaviour
{
    private const float SPEED = 5f;

    private Rigidbody2D   _rb;
    private PrototypeZone _currentZone;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 이동
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _rb.linearVelocity = new Vector2(h, v).normalized * SPEED;

        // 상호작용 (대장간 / 상점)
        if (Input.GetKeyDown(KeyCode.E) && _currentZone != null)
            _currentZone.Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var zone = other.GetComponent<PrototypeZone>();
        if (zone == null) return;
        _currentZone = zone;
        zone.OnPlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var zone = other.GetComponent<PrototypeZone>();
        if (zone == null || zone != _currentZone) return;
        zone.OnPlayerExit();
        _currentZone = null;
    }
}
