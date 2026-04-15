using UnityEngine;

/// <summary>
/// 피로도 관리 싱글톤.
/// 던전 입장(-1), 과일석 처치(-2), 하루 종료 시 100 회복.
/// 담당자: 이도윤
/// </summary>
public class StaminaManager : MonoBehaviour, ISaveable
{
    public static StaminaManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 상수
    // ─────────────────────────────────────────
    public const int MAX_STAMINA          = 100; // 최대 피로도
    public const int DUNGEON_ENTER_COST   = 1;   // 던전 입장 시 소모량
    public const int FRUITSTONE_KILL_COST = 2;   // 과일석 몬스터 처치 시 소모량

    // ─────────────────────────────────────────
    // 런타임 데이터
    // ─────────────────────────────────────────
    private int _stamina = MAX_STAMINA; // 현재 피로도

    /// <summary>현재 피로도 (읽기 전용)</summary>
    public int Stamina => _stamina;

    // ─────────────────────────────────────────
    // 이벤트 (UI가 구독해서 변경 감지)
    // ─────────────────────────────────────────
    /// <summary>피로도 변경 시 발생. (이전값, 새값)</summary>
    public event System.Action<int, int> OnStaminaChanged;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SaveManager.Instance.Register(this); // 세이브 시스템에 등록
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
    }

    // ─────────────────────────────────────────
    // 세이브 / 로드
    // ─────────────────────────────────────────
    public void OnSave(SaveData data)
    {
        data.stamina = _stamina; // 추가
    }

    public void OnLoad(SaveData data)
    {
        _stamina = data.stamina; // 추가
        OnStaminaChanged?.Invoke(_stamina, _stamina); // UI 갱신 알림
    }

    // ─────────────────────────────────────────
    // 피로도 조작
    // ─────────────────────────────────────────

    /// <summary>
    /// 피로도 소모.
    /// 현재 피로도가 amount보다 작으면 false 반환 (소모하지 않음).
    /// ex) UseStamina(1) → 던전 입장
    ///     UseStamina(2) → 과일석 몬스터 처치
    /// </summary>
    /// <param name="amount">소모할 피로도 양</param>
    /// <returns>소모 성공 여부 (false면 입장/행동 불가)</returns>
    public bool UseStamina(int amount)
    {
        if (_stamina < amount)
        {
            Debug.LogWarning($"[Stamina] 피로도 부족 (필요: {amount}, 현재: {_stamina})");
            return false;
        }

        int prev = _stamina;
        _stamina -= amount;
        _stamina  = Mathf.Max(0, _stamina); // 0 아래로 내려가지 않도록 보정
        OnStaminaChanged?.Invoke(prev, _stamina);
        Debug.Log($"[Stamina] 소모 {amount} → 남은 피로도: {_stamina}");
        return true;
    }

    /// <summary>
    /// 던전 입장 가능 여부 확인.
    /// 피로도 1 이상이면 true.
    /// </summary>
    public bool CanEnterDungeon() => _stamina >= DUNGEON_ENTER_COST;

    /// <summary>
    /// 피로도 완전 회복 (100으로 복구).
    /// 하루 종료(EndOfDay) 시 호출.
    /// </summary>
    public void RecoverStamina()
    {
        int prev = _stamina;
        _stamina  = MAX_STAMINA;
        OnStaminaChanged?.Invoke(prev, _stamina);
        Debug.Log("[Stamina] 피로도 완전 회복 → 100");
    }
}
