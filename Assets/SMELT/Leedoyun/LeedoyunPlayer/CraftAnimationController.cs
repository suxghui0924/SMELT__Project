using System.Collections;
using UnityEngine;

/// <summary>
/// 무기 제작 성공 시 재생되는 애니메이션 컨트롤러.
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

    [Tooltip("파티클 발동 타이밍 (0 = 애니메이션 시작, 1 = 끝). 치는 순간에 맞게 조절.")]
    [SerializeField] [Range(0f, 1f)] private float _sparkNormalizedTime = 0.5f;

    [Header("위치 설정")]
    [Tooltip("제작 시 플레이어가 이동할 고정 위치.")]
    [SerializeField] private Vector3 _craftPosition = new(-5.35f, 4.035428f, 0f);
    [Tooltip("플레이어 위치 기준 애니메이션 오프셋.")]
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
        GameObject playerGO = null;

        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null)
            playerGO = pm.gameObject;
        else
        {
            var pp = FindFirstObjectByType<PrototypePlayer>();
            if (pp != null) playerGO = pp.gameObject;
        }

        if (playerGO != null)
        {
            _followTarget = playerGO.transform;
            _playerSr    = playerGO.GetComponent<SpriteRenderer>();
        }
        else
            Debug.LogWarning("[CraftAnim] 플레이어를 찾을 수 없습니다.");
    }

    private void LateUpdate()
    {
        if (_followTarget != null)
            transform.position = _followTarget.position + _offset;
    }

    public void PlayCraftAnimation(Sprite resultSprite = null)
    {
        if (_craftAnimator == null)
        {
            Debug.LogWarning("[CraftAnim] Animator가 연결되지 않았습니다.");
            return;
        }
        StopAllCoroutines();
        PlayerMovement.IsLocked = true;
        if (_followTarget != null)
            _followTarget.position = new Vector3(_craftPosition.x, _craftPosition.y, _followTarget.position.z);
        SetPlayerVisible(false);
        StartCoroutine(ReturnToIdle(resultSprite));
    }

    private IEnumerator ReturnToIdle(Sprite resultSprite)
    {
        for (int i = 0; i < 3; i++)
        {
            _craftAnimator.SetTrigger(_animationTrigger);

            // 애니메이션 상태로 전환될 때까지 대기
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).IsName("Craft Animation"));

            // 치는 타이밍에 파티클 발동
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= _sparkNormalizedTime);

            if (CraftSparkParticle.Instance != null)
                CraftSparkParticle.Instance.Play();

            // 애니메이션이 끝날 때까지 대기
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        _craftAnimator.Play(HashNewState);
        SetPlayerVisible(true);
        PlayerMovement.IsLocked = false;

        // 3회 애니메이션 완료 후 무기 스프라이트 표시
        HeldItemController.Instance?.SetHeldItem(resultSprite);
    }

    private void SetPlayerVisible(bool visible)
    {
        if (_playerSr == null) return;
        var c = _playerSr.color;
        c.a = visible ? 1f : 0f;
        _playerSr.color = c;
    }
}
