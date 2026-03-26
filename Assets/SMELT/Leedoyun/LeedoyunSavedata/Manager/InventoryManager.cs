using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경제 시스템 + 인벤토리 + 테크트리 관리.
/// 담당자: 미
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

        // 인벤토리
        data.inventory.Clear();
        foreach (var pair in _inventory)
            data.inventory.Add(new ItemSaveData { itemId = pair.Key, quantity = pair.Value });
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

        Debug.Log($"[Inventory] 추가: {itemId} x{amount}");
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
        if (_inventory[itemId] <= 0)
            _inventory.Remove(itemId);
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
        _gold += amount;
        Debug.Log($"[Economy] 골드 +{amount} → 현재: {_gold}");
    }

    /// <summary>골드 차감. 부족하면 false 반환.</summary>
    public bool SpendGold(int amount)
    {
        if (_gold < amount)
        {
            Debug.LogWarning($"[Economy] 골드 부족 (필요: {amount}, 보유: {_gold})");
            return false;
        }
        _gold -= amount;
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
        Debug.Log($"[Economy] {_currentDay}일차 시작. 유지비: {_maintenanceCost}");
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
        Debug.Log($"[Tech] 해금: {techId} → 현재 레벨: {_techLevel}");
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
        Debug.Log($"[Craft] {inputId} x{inputAmount} → {outputId} x{outputAmount}");
        return true;
    }
}
