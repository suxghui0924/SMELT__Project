using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오버쿡즈 스타일 무기 납품 판매 시스템.
///
/// [흐름]
///   1. 손님이 특정 무기(타입 + 메인 광석)를 의뢰
///   2. 플레이어가 해당 무기를 인벤토리에서 납품(FulfillOrder)
///   3. 골드 지급 + 오늘/누적 수익 누산
///   4. 시간 내 납품 못 하면 주문 만료
///
/// [25가지 무기 조합]
///   5종류(검/도끼/창/망치/건틀릿) × 5광석(사과/멜론/귤/레몬/포도)
///   현재 날짜에 해금된 조합만 주문에 등장
///
/// 담당자: 이도윤
/// </summary>
public class Leedoyun_SellManager : MonoBehaviour, ISaveable
{
    public static Leedoyun_SellManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 설정값 (Inspector에서 조정 가능)
    // ─────────────────────────────────────────
    [Header("주문 설정")]
    [Tooltip("동시에 표시되는 최대 주문 수")]
    [SerializeField] private int   maxActiveOrders   = 3;
    [Tooltip("새 주문이 생성되는 주기(초)")]
    [SerializeField] private float orderSpawnInterval = 25f;
    [Tooltip("주문 1건의 제한 시간(초)")]
    [SerializeField] private float orderTimeLimit     = 60f;

    // ─────────────────────────────────────────
    // 런타임 데이터
    // ─────────────────────────────────────────
    private List<Leedoyun_CustomerOrder> _activeOrders = new List<Leedoyun_CustomerOrder>();
    private float _spawnTimer  = 0f;  // 다음 주문 생성까지 남은 시간
    private int   _todayGold   = 0;   // 오늘 판매 수익
    private int   _totalGold   = 0;   // 누적 판매 수익

    public int TodayGold  => _todayGold;
    public int TotalGold  => _totalGold;

    /// <summary>현재 활성 주문 목록 (읽기 전용 복사본)</summary>
    public IReadOnlyList<Leedoyun_CustomerOrder> ActiveOrders => _activeOrders;

    // ─────────────────────────────────────────
    // 이벤트 (UI 연동)
    // ─────────────────────────────────────────
    /// <summary>새 주문 추가됨. UI가 구독해서 주문 카드 표시.</summary>
    public event Action<Leedoyun_CustomerOrder> OnOrderAdded;

    /// <summary>주문 납품 완료. (완료된 주문, 지급 골드)</summary>
    public event Action<Leedoyun_CustomerOrder, int> OnOrderFulfilled;

    /// <summary>주문 시간 초과. UI에서 주문 카드 제거.</summary>
    public event Action<Leedoyun_CustomerOrder> OnOrderExpired;

    /// <summary>골드 변경. (이전값, 새값) — UI 수익 카운터 애니메이션용.</summary>
    public event Action<int, int> OnTodayGoldChanged;

    // ─────────────────────────────────────────
    // 해금 테이블 (날짜별 사용 가능한 조합)
    // ─────────────────────────────────────────

    // 광석 해금 일자
    private static readonly (string oreId, int unlockDay)[] _oreUnlockTable =
    {
        ("fruitstone_apple",  1), // 1일차: 사과석
        ("fruitstone_melon",  1), // 1일차: 멜론석
        ("fruitstone_orange", 2), // 2일차: 귤석
        ("fruitstone_lemon",  4), // 4일차: 레몬석
        ("fruitstone_grape",  6), // 6일차: 포도석
    };

    // 무기 해금 일자
    private static readonly (WeaponType type, int unlockDay)[] _weaponUnlockTable =
    {
        (WeaponType.Sword,    1), // 1일차: 검
        (WeaponType.Axe,      1), // 1일차: 도끼
        (WeaponType.Spear,    3), // 3일차: 창
        (WeaponType.Hammer,   5), // 5일차: 망치
        (WeaponType.Gauntlet, 7), // 7일차: 건틀릿
    };

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

        // 게임 시작 시 첫 주문 즉시 생성
        TrySpawnOrder();
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
    }

    // ─────────────────────────────────────────
    // 매 프레임: 타이머 업데이트
    // ─────────────────────────────────────────
    private void Update()
    {
        float dt = Time.deltaTime;

        // 활성 주문 타이머 업데이트
        for (int i = _activeOrders.Count - 1; i >= 0; i--)
        {
            var order = _activeOrders[i];
            if (!order.IsActive) continue;

            order.elapsedTime += dt;

            // 시간 초과 처리
            if (order.elapsedTime >= order.timeLimit)
            {
                order.isExpired = true;
                OnOrderExpired?.Invoke(order);
                _activeOrders.RemoveAt(i);
                Debug.Log($"[SellManager] 주문 만료: {order.requestedWeaponId} (ID: {order.orderId})");
            }
        }

        // 새 주문 생성 타이머
        if (_activeOrders.Count < maxActiveOrders)
        {
            _spawnTimer += dt;
            if (_spawnTimer >= orderSpawnInterval)
            {
                _spawnTimer = 0f;
                TrySpawnOrder();
            }
        }
    }

    // ─────────────────────────────────────────
    // 세이브 / 로드
    // ─────────────────────────────────────────
    public void OnSave(SaveData data)
    {
        // todayEarned / totalEarned는 ShopManager와 별도로 이 매니저 전용 필드 사용
        // → SaveData에 leedoyunTodayGold / leedoyunTotalGold 추가 필요 (아래 항목 참조)
        data.leedoyunTodayGold  = _todayGold;  // 추가 필드
        data.leedoyunTotalGold  = _totalGold;  // 추가 필드
    }

    public void OnLoad(SaveData data)
    {
        _todayGold = data.leedoyunTodayGold;  // 추가 필드
        _totalGold = data.leedoyunTotalGold;  // 추가 필드
        OnTodayGoldChanged?.Invoke(_todayGold, _todayGold);
    }

    // ─────────────────────────────────────────
    // 납품 처리 (플레이어가 무기를 갖다 줄 때 호출)
    // ─────────────────────────────────────────

    /// <summary>
    /// 무기 납품 처리.
    /// 인벤토리에서 해당 무기를 1개 소모하고 골드 지급.
    ///
    /// ex) FulfillOrder("a1b2c3d4", "weapon_sword_apple")
    ///     → 주문 ID와 무기 ID가 일치하면 납품 완료
    /// </summary>
    /// <param name="orderId">납품할 주문의 ID</param>
    /// <param name="weaponItemId">인벤토리에서 납품할 무기 ID</param>
    /// <returns>납품 성공 여부</returns>
    public bool FulfillOrder(string orderId, string weaponItemId)
    {
        // 주문 찾기
        Leedoyun_CustomerOrder order = _activeOrders.Find(o => o.orderId == orderId);
        if (order == null)
        {
            Debug.LogWarning($"[SellManager] 주문 없음: {orderId}");
            return false;
        }

        // 요청 무기와 납품 무기 일치 확인
        if (order.requestedWeaponId != weaponItemId)
        {
            Debug.LogWarning($"[SellManager] 무기 불일치 " +
                             $"(요청: {order.requestedWeaponId}, 납품: {weaponItemId})");
            return false;
        }

        // 인벤토리에서 무기 소모
        if (!InventoryManager.Instance.RemoveItem(weaponItemId, 1))
        {
            Debug.LogWarning($"[SellManager] 인벤토리에 무기 없음: {weaponItemId}");
            return false;
        }

        // 골드 지급
        int prev = _todayGold;
        InventoryManager.Instance.AddGold(order.rewardGold);
        _todayGold += order.rewardGold;
        _totalGold += order.rewardGold;
        OnTodayGoldChanged?.Invoke(prev, _todayGold);

        // 주문 완료 처리
        order.isFulfilled = true;
        _activeOrders.Remove(order);
        OnOrderFulfilled?.Invoke(order, order.rewardGold);

        Debug.Log($"[SellManager] 납품 완료: {weaponItemId} → +{order.rewardGold}G " +
                  $"(오늘 합계: {_todayGold}G)");
        return true;
    }

    /// <summary>
    /// 인벤토리에 있는 무기를 주문에 자동 매칭하여 납품.
    /// 플레이어가 무기를 손님에게 드래그&드롭할 때 사용.
    /// ex) TryFulfillByWeapon("weapon_sword_apple")
    ///     → 해당 무기를 요청 중인 가장 오래된 주문에 납품
    /// </summary>
    public bool TryFulfillByWeapon(string weaponItemId)
    {
        // 해당 무기를 요청 중인 주문 중 가장 먼저 들어온 것 탐색
        Leedoyun_CustomerOrder match =
            _activeOrders.Find(o => o.IsActive && o.requestedWeaponId == weaponItemId);

        if (match == null)
        {
            Debug.LogWarning($"[SellManager] 해당 무기를 요청한 주문 없음: {weaponItemId}");
            return false;
        }

        return FulfillOrder(match.orderId, weaponItemId);
    }

    // ─────────────────────────────────────────
    // 하루 종료 처리
    // ─────────────────────────────────────────

    /// <summary>
    /// 하루 종료 시 호출.
    /// 오늘 수익 초기화 + 남은 주문 모두 제거.
    /// </summary>
    public void EndOfDay()
    {
        _todayGold = 0;
        _activeOrders.Clear();
        _spawnTimer = 0f;
        Debug.Log("[SellManager] 하루 종료 → 주문 초기화");
    }

    // ─────────────────────────────────────────
    // 주문 생성 (내부)
    // ─────────────────────────────────────────

    /// <summary>현재 날짜 기준으로 랜덤 주문 1건 생성.</summary>
    private void TrySpawnOrder()
    {
        if (_activeOrders.Count >= maxActiveOrders) return;

        int currentDay = InventoryManager.Instance.CurrentDay;

        // 해금된 광석 목록
        var unlockedOres = GetUnlockedOres(currentDay);
        // 해금된 무기 목록
        var unlockedWeapons = GetUnlockedWeapons(currentDay);

        if (unlockedOres.Count == 0 || unlockedWeapons.Count == 0) return;

        // 랜덤 조합 선택
        string    oreId      = unlockedOres[UnityEngine.Random.Range(0, unlockedOres.Count)];
        WeaponType weaponType = unlockedWeapons[UnityEngine.Random.Range(0, unlockedWeapons.Count)];

        // 판매 가격 계산 (ShopManager 공식 재사용)
        string oreShortName  = oreId.Replace("fruitstone_", "");
        string weaponTypeId  = GetWeaponTypeId(weaponType);
        string weaponItemId  = $"weapon_{weaponTypeId}_{oreShortName}";
        int    rewardGold    = ShopManager.Instance != null
                                 ? ShopManager.Instance.GetWeaponPrice(weaponItemId)
                                 : CalculateFallbackPrice(weaponType, oreId);

        // 주문 생성
        var order = new Leedoyun_CustomerOrder(weaponType, oreId, rewardGold, orderTimeLimit);
        _activeOrders.Add(order);
        OnOrderAdded?.Invoke(order);

        Debug.Log($"[SellManager] 새 주문: {order.requestedWeaponId} / {rewardGold}G / {orderTimeLimit}초");
    }

    /// <summary>ShopManager 없을 때 대비 fallback 가격 계산.</summary>
    private int CalculateFallbackPrice(WeaponType weaponType, string oreId)
    {
        if (!WeaponCraftManager.Recipes.TryGetValue(weaponType,
            out WeaponCraftManager.WeaponRecipe recipe)) return 0;
        if (!WeaponCraftManager.OreValues.TryGetValue(oreId, out int oreValue)) return 0;
        return recipe.basePrice + oreValue * recipe.mainCount;
    }

    // ─────────────────────────────────────────
    // 해금 헬퍼 (내부)
    // ─────────────────────────────────────────
    private List<string> GetUnlockedOres(int day)
    {
        var list = new List<string>();
        foreach (var entry in _oreUnlockTable)
            if (day >= entry.unlockDay) list.Add(entry.oreId);
        return list;
    }

    private List<WeaponType> GetUnlockedWeapons(int day)
    {
        var list = new List<WeaponType>();
        foreach (var entry in _weaponUnlockTable)
            if (day >= entry.unlockDay) list.Add(entry.type);
        return list;
    }

    private static string GetWeaponTypeId(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:    return "sword";
            case WeaponType.Axe:      return "axe";
            case WeaponType.Spear:    return "spear";
            case WeaponType.Hammer:   return "hammer";
            case WeaponType.Gauntlet: return "gauntlet";
            default:                  return type.ToString().ToLower();
        }
    }

    // ─────────────────────────────────────────
    // 디버그용 (에디터에서 테스트)
    // ─────────────────────────────────────────
#if UNITY_EDITOR
    [ContextMenu("Debug: 주문 강제 생성")]
    private void DebugSpawnOrder() => TrySpawnOrder();

    [ContextMenu("Debug: 주문 전체 출력")]
    private void DebugPrintOrders()
    {
        Debug.Log($"[SellManager] 활성 주문 {_activeOrders.Count}건:");
        foreach (var o in _activeOrders)
            Debug.Log($"  [{o.orderId}] {o.requestedWeaponId} / {o.rewardGold}G " +
                      $"/ 남은시간 {o.RemainingTime:F1}초");
    }
#endif
}
