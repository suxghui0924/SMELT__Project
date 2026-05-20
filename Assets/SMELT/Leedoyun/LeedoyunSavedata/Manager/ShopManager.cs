using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 운영 현황 관리 (판매, 수익 집계, 날짜별 해금).
/// 담당자: 이도윤
/// 무기 판매/가격계산/해금 테이블 추가: 이도윤  // 추가
/// </summary>
public class ShopManager : MonoBehaviour, ISaveable
{
    public static ShopManager Instance { get; private set; }

    [Header("폐업 후 이동할 씬 (비워두면 현재 씬 재로드)")]
    [SerializeField] private string _closeSceneName = "";

    [Header("폐업 시 파괴할 DontDestroyOnLoad 오브젝트들")]
    [SerializeField] private GameObject[] _persistentUIsToDestroy;

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

    // ─────────────────────────────────────────
    // 가게 열기 / 닫기 / 폐업
    // 버튼 OnClick에서 ShopManager.Instance.OpenShop() 형태로 호출하세요.
    // ─────────────────────────────────────────

    /// <summary>가게 상태 변경 이벤트. true = 열림, false = 닫힘.</summary>
    public static event System.Action<bool> OnShopToggled;

    /// <summary>현재 가게가 열려 있는지 여부.</summary>
    public bool IsShopOpen =>
        Leedoyun_SellManager.Instance != null && Leedoyun_SellManager.Instance.IsShopOpen;

    /// <summary>
    /// 가게 열기 — NPC 입장 및 주문 생성을 허용합니다.
    /// </summary>
    public void OpenShop()
    {
        Leedoyun_SellManager.Instance?.OpenShop();
        OnShopToggled?.Invoke(true);
    }

    /// <summary>
    /// 가게 닫기 — NPC 입장을 차단하고 현재 주문을 모두 만료시킵니다.
    /// </summary>
    public void CloseShop()
    {
        Leedoyun_SellManager.Instance?.CloseShop();
        OnShopToggled?.Invoke(false);
    }

    /// <summary>
    /// 가게 열기/닫기 토글 — 열려 있으면 닫고, 닫혀 있으면 엽니다.
    /// 하나의 버튼 OnClick에 연결하세요.
    /// </summary>
    public void ToggleShop()
    {
        if (IsShopOpen)
            CloseShop();
        else
            OpenShop();
    }

    /// <summary>
    /// 가게 폐업 — 가게를 닫고 모든 세이브 데이터를 초기화합니다.
    /// "가게 폐업하기" 버튼 OnClick에 연결하세요.
    /// </summary>
    public void CloseShopPermanently()
    {
        CloseShop();
        SaveManager.Instance?.ResetAllData();

        if (_persistentUIsToDestroy != null)
            foreach (var ui in _persistentUIsToDestroy)
                if (ui != null) ui.SetActive(false);

        Debug.Log("[ShopManager] 가게 폐업 — 데이터 초기화 완료");
        string scene = string.IsNullOrEmpty(_closeSceneName)
            ? UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            : _closeSceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    /// <summary>오늘 판매된 아이템 목록 반환 (UI 표시용).</summary>
    public List<string> GetTodaySalesHistory() => new List<string>(_salesHistory);

    // ─────────────────────────────────────────
    // 무기 판매 (가격 공식 적용)               // 추가
    // ─────────────────────────────────────────

    /// <summary>
    /// 무기 판매.                              // 추가
    /// 가격 공식: (무기 기본금 + 메인 가치 × 메인 개수) × (1 + moreSell)
    ///
    /// ex) SellWeapon("weapon_sword_apple")
    ///     → (500 + 1000 × 1) × (1 + moreSell) 만큼 골드 지급
    /// </summary>
    /// <param name="weaponItemId">인벤토리 무기 ID ("weapon_{type}_{mainOre}" 형식)</param>
    /// <returns>판매 성공 여부</returns>
    public bool SellWeapon(string weaponItemId)                                          // 추가
    {
        // 무기 ID 파싱 (WeaponType + mainOreId 추출)
        if (!WeaponCraftManager.TryParseWeaponItemId(weaponItemId,
            out WeaponType weaponType, out string mainOreId))
        {
            Debug.LogWarning($"[ShopManager] 유효하지 않은 무기 ID: {weaponItemId}");
            return false;
        }

        // 인벤토리에서 무기 제거
        if (!InventoryManager.Instance.RemoveItem(weaponItemId, 1))
            return false;

        // 가격 계산 후 골드 지급
        int price = GetWeaponPrice(weaponItemId);
        InventoryManager.Instance.AddGold(price);
        _todayEarned += price;
        _totalEarned += price;
        _salesHistory.Add(weaponItemId);

        Debug.Log($"[ShopManager] 무기 판매: {weaponItemId} → {price}G");
        return true;
    }

    /// <summary>
    /// 무기 판매 예상 가격 계산 (실제 소모 없음, UI 표시용).  // 추가
    /// 가격 공식: (무기 기본금 + 메인 가치 × 메인 개수) × (1 + moreSell)
    /// </summary>
    /// <param name="weaponItemId">인벤토리 무기 ID</param>
    /// <returns>계산된 판매 가격 (파싱 실패 시 0)</returns>
    public int GetWeaponPrice(string weaponItemId)                                       // 추가
    {
        if (!WeaponCraftManager.TryParseWeaponItemId(weaponItemId,
            out WeaponType weaponType, out string mainOreId))
            return 0;

        if (!WeaponCraftManager.Recipes.TryGetValue(weaponType,
            out WeaponCraftManager.WeaponRecipe recipe))
            return 0;

        if (!WeaponCraftManager.OreValues.TryGetValue(mainOreId, out int oreValue))
            return 0;

        float moreSell = PlayerStatManager.Instance != null
            ? PlayerStatManager.Instance.UpMoreSell
            : 0f;

        // (무기 기본금 + 메인 가치 × 메인 개수) × (1 + moreSell)
        int rawPrice = recipe.basePrice + oreValue * recipe.mainCount;
        return Mathf.RoundToInt(rawPrice * (1f + moreSell));
    }

    // ─────────────────────────────────────────
    // 날짜별 해금 테이블                        // 추가
    // ─────────────────────────────────────────

    /// <summary>
    /// 현재 날짜 기준 해금된 광석 ID 목록 반환.  // 추가
    /// 1일차: 사과/멜론 / 2일차: +귤 / 4일차: +레몬 / 6일차: +포도
    /// </summary>
    public List<string> GetUnlockedOres()                                                // 추가
    {
        int day = InventoryManager.Instance.CurrentDay;
        var ores = new List<string>();

        // 1일차부터 해금
        if (day >= 1) { ores.Add("fruitstone_apple");  ores.Add("fruitstone_melon"); }
        // 2일차: 귤석 해금
        if (day >= 2)   ores.Add("fruitstone_orange");
        // 4일차: 레몬석 해금
        if (day >= 4)   ores.Add("fruitstone_lemon");
        // 6일차: 포도석 해금
        if (day >= 6)   ores.Add("fruitstone_grape");

        return ores;
    }

    /// <summary>
    /// 현재 날짜 기준 해금된 무기 타입 목록 반환.  // 추가
    /// 1일차: 검/도끼 / 3일차: +창 / 5일차: +망치 / 7일차: +건틀릿
    /// </summary>
    public List<WeaponType> GetUnlockedWeapons()                                         // 추가
    {
        int day = InventoryManager.Instance.CurrentDay;
        var weapons = new List<WeaponType>();

        // 1일차부터 해금
        if (day >= 1) { weapons.Add(WeaponType.Sword); weapons.Add(WeaponType.Axe); }
        // 3일차: 창 해금
        if (day >= 3)   weapons.Add(WeaponType.Spear);
        // 5일차: 망치 해금
        if (day >= 5)   weapons.Add(WeaponType.Hammer);
        // 7일차: 건틀릿 해금
        if (day >= 7)   weapons.Add(WeaponType.Gauntlet);

        return weapons;
    }

    /// <summary>
    /// 해당 광석이 현재 날짜에 해금되어 있는지 확인.  // 추가
    /// </summary>
    public bool IsOreUnlocked(string oreId) => GetUnlockedOres().Contains(oreId);       // 추가

    /// <summary>
    /// 해당 무기가 현재 날짜에 해금되어 있는지 확인.  // 추가
    /// </summary>
    public bool IsWeaponUnlocked(WeaponType type) => GetUnlockedWeapons().Contains(type); // 추가
}
