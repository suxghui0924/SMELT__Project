using System;                       // 추가
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 경제 시스템 + 인벤토리 + 테크트리 관리.
/// 담당자: 이도윤
/// </summary>
public class InventoryManager : MonoBehaviour, ISaveable
{
    public static InventoryManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 런타임 데이터
    // ─────────────────────────────────────────
    private Dictionary<string, int> _inventory = new Dictionary<string, int>();

    private int _currentDay      = 1;
    private int _gold            = 0;
    private int _maintenanceCost = 100;
    private int _techLevel       = 1;

    private List<string> _unlockedTechs = new List<string>();

    // ─────────────────────────────────────────
    // 이벤트 (UI가 구독해서 변경 감지)           // 추가
    // ─────────────────────────────────────────
    /// <summary>골드 변경 시 발생. (이전값, 새값)</summary>
    public event Action<int, int> OnGoldChanged;    // 추가

    /// <summary>아이템 수량 변경 시 발생. (itemId, 새 수량)</summary>
    public event Action<string, int> OnItemChanged; // 추가
    // 위와 같음 현재 일차, 유지비용 --이도윤씨가 만든거 사용함. by 박성희
    public event Action<int, int> OnDayChanged;

    // ─────────────────────────────────────────
    // 프로퍼티 (읽기 전용 - 외부 접근용)
    // ─────────────────────────────────────────
    public int CurrentDay      => _currentDay;
    public int Gold            => _gold;
    public int MaintenanceCost => _maintenanceCost;
    public int TechLevel       => _techLevel;

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
        SaveManager.Instance.Register(this);  // 세이브 시스템에 등록
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
        // 경제
        data.currentDay      = _currentDay;
        data.gold            = _gold;
        data.maintenanceCost = _maintenanceCost;

        // 테크트리
        data.currentTechLevel = _techLevel;
        data.unlockedTechs    = _unlockedTechs;

        // 과일석 전용 필드 저장                          // 추가
        data.fruitStoneApple  = GetQuantity("fruitstone_apple");  // 추가
        data.fruitStoneMelon  = GetQuantity("fruitstone_melon");  // 추가
        data.fruitStoneOrange = GetQuantity("fruitstone_orange"); // 추가
        data.fruitStoneLemon  = GetQuantity("fruitstone_lemon");  // 추가
        data.fruitStoneGrape  = GetQuantity("fruitstone_grape");  // 추가

        // 인벤토리 (과일석 제외한 나머지 아이템)        // 수정
        data.inventory.Clear();
        foreach (var pair in _inventory)
        {
            if (pair.Key.StartsWith("fruitstone_")) continue; // 과일석은 전용 필드로 저장 // 추가
            data.inventory.Add(new ItemSaveData { itemId = pair.Key, quantity = pair.Value });
        }
    }

    public void OnLoad(SaveData data)
    {
        _currentDay      = data.currentDay;
        _gold            = data.gold;
        _maintenanceCost = data.maintenanceCost;
        _techLevel       = data.currentTechLevel;
        _unlockedTechs   = data.unlockedTechs;

        _inventory.Clear();
        foreach (var saved in data.inventory)
            _inventory[saved.itemId] = saved.quantity;

        // 과일석 전용 필드 로드                                          // 추가
        if (data.fruitStoneApple  > 0) _inventory["fruitstone_apple"]  = data.fruitStoneApple;  // 추가
        if (data.fruitStoneMelon  > 0) _inventory["fruitstone_melon"]  = data.fruitStoneMelon;  // 추가
        if (data.fruitStoneOrange > 0) _inventory["fruitstone_orange"] = data.fruitStoneOrange; // 추가
        if (data.fruitStoneLemon  > 0) _inventory["fruitstone_lemon"]  = data.fruitStoneLemon;  // 추가
        if (data.fruitStoneGrape  > 0) _inventory["fruitstone_grape"]  = data.fruitStoneGrape;  // 추가

        // 로드 완료 후 UI에 전체 갱신 알림                  // 추가
        OnGoldChanged?.Invoke(_gold, _gold);                  // 추가
        foreach (var pair in _inventory)                      // 추가
            OnItemChanged?.Invoke(pair.Key, pair.Value);      // 추가
    }

    // ─────────────────────────────────────────
    // 인벤토리 조작
    // ─────────────────────────────────────────

    /// <summary>아이템 추가. ex) AddItem("fruitstone_strawberry", 3)</summary>
    public void AddItem(string itemId, int amount = 1)
    {
        if (_inventory.ContainsKey(itemId))
            _inventory[itemId] += amount;
        else
            _inventory[itemId] = amount;

        OnItemChanged?.Invoke(itemId, _inventory[itemId]); // 추가
    }

    /// <summary>아이템 소모. 수량 부족이면 false 반환.</summary>
    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (!HasItem(itemId, amount))
        {
            Debug.LogWarning($"[Inventory] 수량 부족: {itemId} (필요: {amount}, 보유: {GetQuantity(itemId)})");
            return false;
        }
        _inventory[itemId] -= amount;
        int remaining = _inventory[itemId];
        if (remaining <= 0)
            _inventory.Remove(itemId);

        OnItemChanged?.Invoke(itemId, remaining <= 0 ? 0 : remaining); // 추가
        return true;
    }

    /// <summary>수량 조회.</summary>
    public int GetQuantity(string itemId)
        => _inventory.TryGetValue(itemId, out int qty) ? qty : 0;

    /// <summary>해당 아이템을 amount개 이상 보유 중인지 확인.</summary>
    public bool HasItem(string itemId, int amount = 1)
        => GetQuantity(itemId) >= amount;

    // ─────────────────────────────────────────
    // 경제 조작
    // ─────────────────────────────────────────

    /// <summary>골드 추가.</summary>
    public void AddGold(int amount)
    {
        int prev = _gold;   // 추가
        _gold += amount;
        OnGoldChanged?.Invoke(prev, _gold); // 추가
    }

    /// <summary>골드 차감. 부족하면 false 반환.</summary>
    public bool SpendGold(int amount)
    {
        if (_gold < amount)
        {
            Debug.LogWarning($"[Economy] 골드 부족 (필요: {amount}, 보유: {_gold})");
            return false;
        }
        int prev = _gold;   // 추가
        _gold -= amount;
        OnGoldChanged?.Invoke(prev, _gold); // 추가
        return true;
    }

    /// <summary>하루 종료 처리. 유지비 차감 + 날짜 증가 + 유지비 상승.</summary>
    public bool EndOfDay()
    {
        if (!SpendGold(_maintenanceCost))
        {
            Debug.LogWarning("[Economy] 유지비 부족 → 게임 오버 처리 필요");
            return false;  // 골드 부족 → 게임 오버
        }

        _currentDay++;
        _maintenanceCost = Mathf.RoundToInt(_maintenanceCost * 1.2f);  // 유지비 20% 증가
        OnDayChanged?.Invoke(_currentDay, _maintenanceCost); // 추가
        return true;
    }

    // ─────────────────────────────────────────
    // 테크트리
    // ─────────────────────────────────────────

    /// <summary>기술 해금. ex) UnlockTech("tech_juice")</summary>
    public bool UnlockTech(string techId)
    {
        if (_unlockedTechs.Contains(techId)) return false;
        _unlockedTechs.Add(techId);
        _techLevel++;
        return true;
    }

    public bool IsTechUnlocked(string techId) => _unlockedTechs.Contains(techId);

    // ─────────────────────────────────────────
    // 제작 (패링 → 제련/착즙)
    // ─────────────────────────────────────────

    /// <summary>
    /// 제작 처리.
    /// ex) Craft("fruitstone_strawberry", 2, "weapon_sword", 1)
    ///     → 딸기석 2개 소모 → 검 1개 획득
    /// </summary>
    public bool Craft(string inputId, int inputAmount, string outputId, int outputAmount)
    {
        if (!RemoveItem(inputId, inputAmount)) return false;
        AddItem(outputId, outputAmount);
        return true;
    }
}
