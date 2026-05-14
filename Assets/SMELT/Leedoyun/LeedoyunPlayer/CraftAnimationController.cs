using System.Collections;
using UnityEngine;

/// <summary>
/// 무기 제작 성공 시 재생되는 애니메이션 컨트롤러.
/// 플레이어를 자동으로 찾아 따라다니며 Offset만큼 위에 표시.
///
/// 담당자: 이도윤
/// </summary>
public class CraftAnimationController : MonoBehaviour
{
    public static CraftAnimationController Instance { get; private set; }

    [Header("제작 애니메이션")]
    [Tooltip("재생할 애니메이션의 Animator 컴포넌트를 연결하세요.")]
    [SerializeField] private Animator _craftAnimator;

    [Tooltip("Animator Controller Trigger 파라미터 이름.")]
    [SerializeField] private string _animationTrigger = "CraftPlay";

    [Header("위치 설정")]
    [Tooltip("플레이어 위치 기준 오프셋 (플레이어 머리 위쪽).")]
    [SerializeField] private Vector3 _offset = new(0f, 1.5f, 0f);

    private static readonly int HashNewState = Animator.StringToHash("New State");

    private Transform _followTarget;
    private SpriteRenderer _playerSr;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        var player = FindFirstObjectByType<PrototypePlayer>();
        if (player != null)
        {
            _followTarget = player.transform;
            _playerSr = player.GetComponent<SpriteRenderer>();
        }
        else
            Debug.LogWarning("[CraftAnim] PrototypePlayer를 찾을 수 없습니다.");
    }

    private void LateUpdate()
    {
        if (_followTarget != null)
            transform.position = _followTarget.position + _offset;
    }

    /// <summary>제작 성공 시 호출 — 플레이어 투명화 후 애니메이션 재생, 완료되면 복귀.</summary>
    public void PlayCraftAnimation()
    {
        if (_craftAnimator == null)
        {
            Debug.LogWarning("[CraftAnim] Animator가 연결되지 않았습니다.");
            return;
        }
        StopAllCoroutines();
        SetPlayerVisible(false);
        _craftAnimator.SetTrigger(_animationTrigger);
        StartCoroutine(ReturnToIdle());
    }

    private IEnumerator ReturnToIdle()
    {
        // 애니메이션 상태로 전환될 때까지 대기
        yield return new WaitUntil(() =>
            _craftAnimator.GetCurrentAnimatorStateInfo(0).IsName("Craft Animation"));

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() =>
            _craftAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        _craftAnimator.Play(HashNewState);
        SetPlayerVisible(true);
    }

    private void SetPlayerVisible(bool visible)
    {
        if (_playerSr == null) return;
        var c = _playerSr.color;
        c.a = visible ? 1f : 0f;
        _playerSr.color = c;
    }
}
