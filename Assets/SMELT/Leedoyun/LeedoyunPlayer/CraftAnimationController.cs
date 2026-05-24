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

    [Tooltip("파티클·사운드 발동 타이밍 (0 = 애니메이션 시작, 1 = 끝). 치는 순간에 맞게 조절.")]
    [SerializeField] [Range(0f, 1f)] private float _sparkNormalizedTime = 0.5f;

    [Header("타격 사운드")]
    [Tooltip("망치 타격음을 재생할 AudioSource 컴포넌트를 연결하세요.")]
    [SerializeField] private AudioSource _audioSource;

    [Tooltip("타격 사운드 클립. 클립 길이에 맞게 애니메이션 속도가 자동 조정됩니다.")]
    [SerializeField] private AudioClip _hammerSound;

    [Tooltip("전체 타격 속도 배율. 1.0 = 사운드 기준 자동 속도, 낮출수록 느려집니다.")]
    [SerializeField] [Range(0.5f, 1.5f)] private float _speedMultiplier = 1f;

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
    }

    private void LateUpdate()
    {
        if (_followTarget != null)
            transform.position = _followTarget.position + _offset;
    }

    public void PlayCraftAnimation(Sprite resultSprite = null)
    {
        if (_craftAnimator == null)
            return;
        StopAllCoroutines();
        PlayerMovement.IsLocked = true;
        if (_followTarget != null)
            _followTarget.position = new Vector3(_craftPosition.x, _craftPosition.y, _followTarget.position.z);
        HeldItemController.Instance?.HideForCraft();
        SetPlayerVisible(false);
        StartCoroutine(ReturnToIdle(resultSprite));
    }

    private IEnumerator ReturnToIdle(Sprite resultSprite)
    {
        const int hitCount = 4;

        // 무기 제작속도 스킬 배율 (WeaponUp 업그레이드, 기본 1.0)
        float statSpeed = PlayerStatManager.Instance != null
            ? PlayerStatManager.Instance.UpMakeSpeedWeapon : 1f;

        // 사운드 전체를 한 번에 재생 (스킬 속도 비례 피치 적용)
        if (_audioSource != null && _hammerSound != null)
        {
            _audioSource.pitch = statSpeed;
            _audioSource.PlayOneShot(_hammerSound);
        }

        for (int i = 0; i < hitCount; i++)
        {
            _craftAnimator.SetTrigger(_animationTrigger);

            // 애니메이션 상태로 전환될 때까지 대기
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).IsName("Craft Animation"));

            // 속도 계산: 4사이클이 사운드 전체 길이에 맞도록, 배율 및 스킬 속도 적용
            if (i == 0 && _hammerSound != null)
            {
                float animLength = _craftAnimator.GetCurrentAnimatorStateInfo(0).length;
                _craftAnimator.speed = (animLength * hitCount) / _hammerSound.length * _speedMultiplier * statSpeed;
            }

            // 치는 타이밍에 파티클 발동
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= _sparkNormalizedTime);

            if (CraftSparkParticle.Instance != null)
                CraftSparkParticle.Instance.Play();

            // 애니메이션이 끝날 때까지 대기
            yield return new WaitUntil(() =>
                _craftAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        _craftAnimator.speed = 1f;
        if (_audioSource != null) _audioSource.pitch = 1f;
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
