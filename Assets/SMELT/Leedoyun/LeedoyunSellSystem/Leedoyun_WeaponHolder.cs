using UnityEngine;

/// <summary>
/// 플레이어가 무기를 들고 다니는 스크립트.
/// 플레이어 오브젝트에 부착.
///
/// [흐름]
///   1. 제작 완료된 무기 테이블에 접근 → PickUp("weapon_sword_apple")
///   2. 손님 슬롯(Leedoyun_CustomerSlot)에 충돌 → 자동 납품
///   3. 납품 성공 or 실패 시 손에서 무기 제거
///
/// 담당자: 이도윤
/// </summary>
public class Leedoyun_WeaponHolder : MonoBehaviour
{
    public static Leedoyun_WeaponHolder Instance { get; private set; }

    // ─────────────────────────────────────────
    // 런타임 상태
    // ─────────────────────────────────────────
    /// <summary>현재 들고 있는 무기 ID. 없으면 빈 문자열.</summary>
    public string HeldWeaponId { get; private set; } = string.Empty;

    /// <summary>무기를 들고 있는지 여부.</summary>
    public bool IsHolding => !string.IsNullOrEmpty(HeldWeaponId);

    // ─────────────────────────────────────────
    // 시각적 표현 (선택)
    // ─────────────────────────────────────────
    [Header("시각 효과 (선택)")]
    [Tooltip("손에 들었을 때 표시할 무기 프리팹 위치")]
    [SerializeField] private Transform holdPoint;

    private GameObject _heldVisual; // 현재 표시 중인 무기 비주얼

    // ─────────────────────────────────────────
    // 이벤트
    // ─────────────────────────────────────────
    /// <summary>무기를 집었을 때. (무기 ID)</summary>
    public event System.Action<string> OnPickedUp;

    /// <summary>무기를 내려놓거나 납품했을 때. (무기 ID)</summary>
    public event System.Action<string> OnDropped;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // 무기 집기 / 내려놓기
    // ─────────────────────────────────────────

    /// <summary>
    /// 무기 집기.
    /// 인벤토리에 해당 무기가 있어야 함.
    /// 이미 다른 무기를 들고 있으면 false 반환.
    /// </summary>
    /// <param name="weaponItemId">집을 무기 인벤토리 ID</param>
    public bool PickUp(string weaponItemId)
    {
        if (IsHolding)
        {
            Debug.LogWarning($"[WeaponHolder] 이미 무기를 들고 있음: {HeldWeaponId}");
            return false;
        }

        if (!InventoryManager.Instance.HasItem(weaponItemId, 1))
        {
            Debug.LogWarning($"[WeaponHolder] 인벤토리에 없음: {weaponItemId}");
            return false;
        }

        HeldWeaponId = weaponItemId;
        OnPickedUp?.Invoke(HeldWeaponId);
        Debug.Log($"[WeaponHolder] 집음: {HeldWeaponId}");
        return true;
    }

    /// <summary>
    /// 무기 내려놓기 (납품 실패 or 취소).
    /// 인벤토리에는 그대로 유지.
    /// </summary>
    public void Drop()
    {
        if (!IsHolding) return;

        string dropped = HeldWeaponId;
        HeldWeaponId   = string.Empty;

        if (_heldVisual != null)
        {
            Destroy(_heldVisual);
            _heldVisual = null;
        }

        OnDropped?.Invoke(dropped);
        Debug.Log($"[WeaponHolder] 내려놓음: {dropped}");
    }

    /// <summary>
    /// 납품 성공 후 손에서 무기 제거 (내부에서 호출).
    /// Drop과 달리 인벤토리 차감은 SellManager에서 이미 처리됨.
    /// </summary>
    internal void ClearAfterDeliver()
    {
        string delivered = HeldWeaponId;
        HeldWeaponId     = string.Empty;

        if (_heldVisual != null)
        {
            Destroy(_heldVisual);
            _heldVisual = null;
        }

        OnDropped?.Invoke(delivered);
    }
}
