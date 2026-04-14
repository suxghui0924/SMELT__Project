using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 손님 한 명의 자리 (트리거 콜라이더 기반 납품).
/// 각 손님 오브젝트에 부착.
///
/// [씬 세팅]
///   - 이 스크립트를 붙인 오브젝트에 Collider 추가 → Is Trigger 체크
///   - 플레이어는 Leedoyun_WeaponHolder 보유 + Rigidbody 필요
///   - Leedoyun_SellManager.OnOrderAdded 이벤트로 AssignOrder() 호출
///
/// [납품 흐름]
///   플레이어가 무기 들고 이 콜라이더에 진입
///   → 요청 무기와 일치하면 납품 완료
///   → 불일치면 경고 효과만 (무기 유지)
///
/// 담당자: 이도윤
/// </summary>
[RequireComponent(typeof(Collider))]
public class Leedoyun_CustomerSlot : MonoBehaviour
{
    // ─────────────────────────────────────────
    // UI 연결 (Inspector)
    // ─────────────────────────────────────────
    [Header("UI 연결")]
    [Tooltip("요청 무기 이름 텍스트")]
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [Tooltip("남은 시간 게이지 바")]
    [SerializeField] private Image timerBar;

    [Tooltip("보상 골드 텍스트")]
    [SerializeField] private TextMeshProUGUI rewardText;

    [Tooltip("주문 없을 때 표시할 오브젝트 (빈 슬롯 UI)")]
    [SerializeField] private GameObject emptySlotUI;

    [Tooltip("주문 있을 때 표시할 오브젝트 (주문 카드 UI)")]
    [SerializeField] private GameObject orderCardUI;

    // ─────────────────────────────────────────
    // 납품 피드백 (선택)
    // ─────────────────────────────────────────
    [Header("납품 피드백 (선택)")]
    [Tooltip("납품 성공 시 재생할 파티클")]
    [SerializeField] private ParticleSystem successEffect;

    [Tooltip("납품 실패(불일치) 시 재생할 파티클")]
    [SerializeField] private ParticleSystem failEffect;

    // ─────────────────────────────────────────
    // 상태
    // ─────────────────────────────────────────
    private Leedoyun_CustomerOrder _order; // 현재 할당된 주문

    /// <summary>현재 주문이 있는지 여부.</summary>
    public bool HasOrder => _order != null && _order.IsActive;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Start()
    {
        RefreshUI();
    }

    // ─────────────────────────────────────────
    // 매 프레임: 타이머 바 갱신
    // ─────────────────────────────────────────
    private void Update()
    {
        if (!HasOrder) return;

        // 타이머 바 갱신
        if (timerBar != null)
            timerBar.fillAmount = _order.RemainingRatio;

        // 만료 감지 (SellManager가 이미 처리하지만 UI 정리도 여기서)
        if (_order.isExpired)
            ClearOrder();
    }

    // ─────────────────────────────────────────
    // 주문 할당 / 해제
    // ─────────────────────────────────────────

    /// <summary>
    /// 이 슬롯에 주문 할당.
    /// Leedoyun_SellManager.OnOrderAdded 이벤트에서 호출.
    /// </summary>
    public void AssignOrder(Leedoyun_CustomerOrder order)
    {
        _order = order;
        RefreshUI();
        Debug.Log($"[CustomerSlot:{name}] 주문 할당: {order.requestedWeaponId}");
    }

    /// <summary>주문 해제 (납품 완료 or 만료).</summary>
    public void ClearOrder()
    {
        _order = null;
        RefreshUI();
    }

    // ─────────────────────────────────────────
    // 충돌 납품 처리
    // ─────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!HasOrder) return;

        // 플레이어에게 WeaponHolder가 있는지 확인
        Leedoyun_WeaponHolder holder = other.GetComponent<Leedoyun_WeaponHolder>();
        if (holder == null) return;
        if (!holder.IsHolding) return;

        // 납품 시도
        bool success = Leedoyun_SellManager.Instance.FulfillOrder(
            _order.orderId, holder.HeldWeaponId);

        if (success)
        {
            // 납품 성공
            holder.ClearAfterDeliver();
            successEffect?.Play();
            ClearOrder();
        }
        else
        {
            // 무기 불일치 — 무기는 유지, 경고 효과만
            failEffect?.Play();
            Debug.Log($"[CustomerSlot:{name}] 납품 실패: " +
                      $"요청={_order.requestedWeaponId}, 들고있음={holder.HeldWeaponId}");
        }
    }

    // ─────────────────────────────────────────
    // UI 갱신
    // ─────────────────────────────────────────
    private void RefreshUI()
    {
        bool hasOrder = HasOrder;

        if (emptySlotUI != null) emptySlotUI.SetActive(!hasOrder);
        if (orderCardUI  != null) orderCardUI.SetActive(hasOrder);

        if (!hasOrder) return;

        if (weaponNameText != null)
            weaponNameText.text = GetWeaponDisplayName(_order.requestedWeaponId);

        if (rewardText != null)
            rewardText.text = $"{_order.rewardGold}G";

        if (timerBar != null)
            timerBar.fillAmount = 1f;
    }

    // ─────────────────────────────────────────
    // 표시용 이름 변환
    // ─────────────────────────────────────────

    /// <summary>"weapon_sword_apple" → "사과 검" 형태로 변환.</summary>
    private static string GetWeaponDisplayName(string weaponItemId)
    {
        string[] parts = weaponItemId.Split('_');
        if (parts.Length < 3) return weaponItemId;

        string weaponKor = GetWeaponKorName(parts[1]);
        string oreKor    = GetOreKorName(parts[2]);
        return $"{oreKor} {weaponKor}";
    }

    private static string GetWeaponKorName(string typeId)
    {
        switch (typeId)
        {
            case "sword":    return "검";
            case "axe":      return "도끼";
            case "spear":    return "창";
            case "bat":      return "방망이";
            case "gauntlet": return "건틀릿";
            default:         return typeId;
        }
    }

    private static string GetOreKorName(string oreShortName)
    {
        switch (oreShortName)
        {
            case "apple":  return "사과";
            case "melon":  return "멜론";
            case "orange": return "귤";
            case "lemon":  return "레몬";
            case "grape":  return "포도";
            default:       return oreShortName;
        }
    }
}
