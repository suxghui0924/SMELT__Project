using UnityEngine;

/// <summary>
/// 플레이어가 무기를 들고 있는 상태를 관리.
/// 제작 완료 시 SetHeldItem()으로 무기 스프라이트를 설정.
/// 담당자: 이도윤
/// </summary>
public class HeldItemController : MonoBehaviour
{
    public static HeldItemController Instance { get; private set; }

    [Tooltip("플레이어 기준 무기 스프라이트 오프셋.")]
    [SerializeField] private Vector3 _holdOffset = new(0.3f, 0.1f, 0f);
    [Tooltip("무기 스프라이트 로컬 스케일.")]
    [SerializeField] private float _itemScale = 0.25f;
    [Tooltip("무기 스프라이트 소팅 오더.")]
    [SerializeField] private int _itemSortingOrder = 100;
    [Tooltip("무기를 들었을 때 플레이어 스케일 배율.")]
    [SerializeField] private float _holdingScaleMultiplier = 0.75f;

    [Header("들기 애니메이션 클립 — Inspector에서 할당")]
    [SerializeField] private AnimationClip _holdIdleClip;
    [SerializeField] private AnimationClip _holdFrontClip;
    [SerializeField] private AnimationClip _holdBackClip;
    [SerializeField] private AnimationClip _holdRightClip;

    private SpriteRenderer _itemRenderer;
    private SpriteRenderer _playerRenderer;
    private Animator       _playerAnim;
    private Transform      _playerTransform;
    private Vector3        _originalScale;
    private bool           _playerFound;

    private static readonly int HashMoveX = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY = Animator.StringToHash("MoveY");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (!_playerFound) TryFindPlayer();

        if (_itemRenderer != null && _itemRenderer.enabled && _playerRenderer != null)
            _itemRenderer.sortingOrder = _playerRenderer.sortingOrder + 1;
    }

    // Animator보다 늦게 실행되어 홀딩 애니메이션을 덮어씀
    private void LateUpdate()
    {
        if (_itemRenderer == null || !_itemRenderer.enabled) return;
        if (_playerTransform == null) return;

        float mx = _playerAnim != null ? _playerAnim.GetFloat(HashMoveX) : 0f;
        float my = _playerAnim != null ? _playerAnim.GetFloat(HashMoveY) : 0f;

        AnimationClip clip = SelectClip(mx, my);
        if (clip == null) return;

        clip.SampleAnimation(_playerTransform.gameObject, Time.time % clip.length);

        // 왼쪽 이동 시 Right 클립을 좌우 반전해서 재사용
        if (_playerRenderer != null)
            _playerRenderer.flipX = mx < -0.1f;
    }

    private AnimationClip SelectClip(float mx, float my)
    {
        bool moving = Mathf.Abs(mx) > 0.1f || Mathf.Abs(my) > 0.1f;
        if (!moving) return _holdIdleClip != null ? _holdIdleClip : _holdFrontClip;

        if (my > 0.1f)             return _holdBackClip  != null ? _holdBackClip  : _holdFrontClip;
        if (my < -0.1f)            return _holdFrontClip != null ? _holdFrontClip : _holdBackClip;
        if (Mathf.Abs(mx) > 0.1f) return _holdRightClip != null ? _holdRightClip : _holdFrontClip;
        return _holdFrontClip;
    }

    private void TryFindPlayer()
    {
        GameObject playerGO = null;

        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) playerGO = pm.gameObject;
        else
        {
            var pp = FindFirstObjectByType<PrototypePlayer>();
            if (pp != null) playerGO = pp.gameObject;
        }

        if (playerGO == null) return;

        _playerFound     = true;
        _playerTransform = playerGO.transform;
        _originalScale   = _playerTransform.localScale;
        _playerRenderer  = playerGO.GetComponentInChildren<SpriteRenderer>(true);
        _playerAnim      = playerGO.GetComponentInChildren<Animator>(true);

        var child = new GameObject("HeldItemSprite");
        child.transform.SetParent(playerGO.transform, false);
        child.transform.localPosition = _holdOffset;
        child.transform.localScale    = new Vector3(_itemScale, _itemScale, 1f);

        _itemRenderer = child.AddComponent<SpriteRenderer>();
        _itemRenderer.sortingLayerName = _playerRenderer != null ? _playerRenderer.sortingLayerName : "Default";
        _itemRenderer.sortingOrder     = _itemSortingOrder;
        _itemRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetHeldItem(Sprite sprite)
    {
        if (sprite == null) { ClearHeldItem(); return; }

        if (_itemRenderer != null)
        {
            _itemRenderer.sprite  = sprite;
            _itemRenderer.enabled = true;
        }

        if (_playerTransform != null)
            _playerTransform.localScale = _originalScale * _holdingScaleMultiplier;
    }

    public void ClearHeldItem()
    {
        if (_itemRenderer != null)
        {
            _itemRenderer.sprite  = null;
            _itemRenderer.enabled = false;
        }

        if (_playerTransform != null)
            _playerTransform.localScale = _originalScale;

        if (_playerRenderer != null)
            _playerRenderer.flipX = false;
    }

    public bool IsHolding => _itemRenderer != null && _itemRenderer.enabled;
}
