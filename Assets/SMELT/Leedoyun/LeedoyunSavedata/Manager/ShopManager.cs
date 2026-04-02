using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 운영 현황 관리 (판매, 수익 집계).
/// 담당자: 박성희
/// </summary>
public class ShopManager : MonoBehaviour, ISaveable
{
    public static ShopManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 런타임 데이터
    // ─────────────────────────────────────────
    private int          _totalEarned  = 0;   // 누적 총 수익
    private int          _todayEarned  = 0;   // 오늘 번 돈
    private List<string> _salesHistory = new List<string>();  // 오늘 판매된 아이템 ID 목록

    public int TotalEarned => _totalEarned;
    public int TodayEarned => _todayEarned;

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
        SaveManager.Instance.Register(this);
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
        data.totalEarned  = _totalEarned;
        data.todayEarned  = _todayEarned;
        data.salesHistory = _salesHistory;
    }

    public void OnLoad(SaveData data)
    {
        _totalEarned  = data.totalEarned;
        _todayEarned  = data.todayEarned;
        _salesHistory = data.salesHistory;
    }

    // ─────────────────────────────────────────
    // 판매 처리
    // ─────────────────────────────────────────

    /// <summary>
    /// 아이템 판매 처리.
    /// ex) SellItem("weapon_sword") → ItemDatabase에서 sellPrice 조회 후 골드 지급
    /// </summary>
    public bool SellItem(string itemId)
    {
        // 인벤토리에서 아이템 제거
        if (!InventoryManager.Instance.RemoveItem(itemId, 1))
            return false;

        // 아이템 가격 조회
        ItemData itemData = ItemDatabase.Instance.Get(itemId);
        if (itemData == null) return false;

        // 판매 수익 보너스 적용 (이윤건 담당 PlayerStatManager)
        int finalPrice = PlayerStatManager.Instance.ApplySalesBonus(itemData.sellPrice);

        // 골드 지급 및 기록
        InventoryManager.Instance.AddGold(finalPrice);
        _todayEarned  += finalPrice;
        _totalEarned  += finalPrice;
        _salesHistory.Add(itemId);

        return true;
    }

    /// <summary>하루 종료 시 오늘 수익 초기화.</summary>
    public void EndOfDay()
    {
        _todayEarned = 0;
        _salesHistory.Clear();
    }

    /// <summary>오늘 판매된 아이템 목록 반환 (UI 표시용).</summary>
    public List<string> GetTodaySalesHistory() => new List<string>(_salesHistory);
}
