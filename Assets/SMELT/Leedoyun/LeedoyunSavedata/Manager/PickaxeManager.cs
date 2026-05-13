using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 곡괭이 구매 / 장착 / 저장 관리.
/// 담당자: 이도윤
/// </summary>
public class PickaxeManager : MonoBehaviour, ISaveable
{
    public static PickaxeManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // 인스펙터 연결
    // ─────────────────────────────────────────
    [SerializeField] private PickaxeDataListSO _dataList; // fruitTypes 매핑용

    // ─────────────────────────────────────────
    // 런타임 데이터
    // ─────────────────────────────────────────
    private PickaxeDataSO    _equippedPickaxe;
    private List<string>     _purchasedPickaxeIds = new();

    // 로드 후 SO 참조 복원 전까지 임시 보관
    private string _pendingEquippedId = "pickaxe_default";

    public PickaxeDataSO EquippedPickaxe => _equippedPickaxe;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 기본 곡괭이는 항상 보유
        if (!_purchasedPickaxeIds.Contains("pickaxe_default"))
            _purchasedPickaxeIds.Add("pickaxe_default");

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
        data.equippedPickaxeId   = _equippedPickaxe != null ? _equippedPickaxe.pickaxeId : "pickaxe_default"; // 추가
        data.purchasedPickaxeIds = new List<string>(_purchasedPickaxeIds);                                    // 추가
    }

    public void OnLoad(SaveData data)
    {
        // 구매 목록 복원 // 추가
        _purchasedPickaxeIds = (data.purchasedPickaxeIds != null && data.purchasedPickaxeIds.Count > 0)
            ? new List<string>(data.purchasedPickaxeIds)
            : new List<string> { "pickaxe_default" };

        // 장착 ID 임시 보관 (SO 복원은 LoadEquippedPickaxe()로 처리) // 추가
        _pendingEquippedId = string.IsNullOrEmpty(data.equippedPickaxeId)
            ? "pickaxe_default"
            : data.equippedPickaxeId;
    }

    // ─────────────────────────────────────────
    // 곡괭이 구매
    // ─────────────────────────────────────────
    /// <summary>
    /// 곡괭이 구매.
    /// InventoryManager.SpendGold()로 골드 차감,
    /// InventoryManager.RemoveItem()으로 광석 차감.
    /// 재료 부족 시 false 반환 + Debug.LogWarning 출력.
    /// </summary>
    public bool BuyPickaxe(PickaxeDataSO pickaxe) // 수정 (PickaxeSO → PickaxeDataSO)
    {
        if (pickaxe == null)
        {
            Debug.LogWarning("[PickaxeManager] 구매 대상 곡괭이가 null입니다.");
            return false;
        }

        if (_purchasedPickaxeIds.Contains(pickaxe.pickaxeId))
        {
            Debug.LogWarning($"[PickaxeManager] 이미 구매한 곡괭이: {pickaxe.pickaxeId}");
            return false;
        }

        var inv = InventoryManager.Instance;

        // 골드 사전 확인
        if (pickaxe.goldPrice > 0 && inv.Gold < pickaxe.goldPrice)
        {
            Debug.LogWarning($"[PickaxeManager] 골드 부족 (필요: {pickaxe.goldPrice}G, 보유: {inv.Gold}G)");
            return false;
        }

        // 광석 사전 확인 (fruitPrice 배열 순회) // 추가
        if (_dataList != null && pickaxe.fruitPrice != null)
        {
            for (int i = 0; i < pickaxe.fruitPrice.Length; i++)
            {
                if (pickaxe.fruitPrice[i] <= 0) continue;

                string itemId = (_dataList.fruitTypes != null && i < _dataList.fruitTypes.Length)
                    ? _dataList.fruitTypes[i]
                    : null;

                if (string.IsNullOrEmpty(itemId)) continue;

                if (!inv.HasItem(itemId, pickaxe.fruitPrice[i]))
                {
                    Debug.LogWarning($"[PickaxeManager] 광석 부족: {itemId} " +
                                     $"(필요: {pickaxe.fruitPrice[i]}, 보유: {inv.GetQuantity(itemId)})");
                    return false;
                }
            }
        }

        // 골드 차감
        if (pickaxe.goldPrice > 0)
            inv.SpendGold(pickaxe.goldPrice);

        // 광석 차감 // 추가
        if (_dataList != null && pickaxe.fruitPrice != null)
        {
            for (int i = 0; i < pickaxe.fruitPrice.Length; i++)
            {
                if (pickaxe.fruitPrice[i] <= 0) continue;

                string itemId = (_dataList.fruitTypes != null && i < _dataList.fruitTypes.Length)
                    ? _dataList.fruitTypes[i]
                    : null;

                if (!string.IsNullOrEmpty(itemId))
                    inv.RemoveItem(itemId, pickaxe.fruitPrice[i]);
            }
        }

        _purchasedPickaxeIds.Add(pickaxe.pickaxeId);
        Debug.Log($"[PickaxeManager] 구매 완료: {pickaxe.pickaxeId}");
        return true;
    }

    // ─────────────────────────────────────────
    // 곡괭이 장착
    // ─────────────────────────────────────────
    /// <summary>
    /// 구매한 곡괭이만 장착 가능.
    /// </summary>
    public bool EquipPickaxe(PickaxeDataSO pickaxe) // 수정 (PickaxeSO → PickaxeDataSO)
    {
        if (pickaxe == null)
        {
            Debug.LogWarning("[PickaxeManager] 장착 대상 곡괭이가 null입니다.");
            return false;
        }

        if (!_purchasedPickaxeIds.Contains(pickaxe.pickaxeId))
        {
            Debug.LogWarning($"[PickaxeManager] 구매하지 않은 곡괭이: {pickaxe.pickaxeId}");
            return false;
        }

        _equippedPickaxe = pickaxe;
        Debug.Log($"[PickaxeManager] 장착: {pickaxe.pickaxeId}");
        return true;
    }

    // ─────────────────────────────────────────
    // 유틸리티
    // ─────────────────────────────────────────

    /// <summary>해당 곡괭이를 이미 구매했는지 확인.</summary>
    public bool IsPickaxePurchased(string pickaxeId) => _purchasedPickaxeIds.Contains(pickaxeId);

    /// <summary>
    /// 로드 후 SO 배열을 받아 장착 곡괭이 참조 복원.
    /// 씬 진입 또는 Bootstrap에서 전체 SO 배열과 함께 호출.
    /// </summary>
    public void LoadEquippedPickaxe(PickaxeDataSO[] allPickaxes) // 수정 (PickaxeSO[] → PickaxeDataSO[])
    {
        foreach (var p in allPickaxes)
        {
            if (p.pickaxeId == _pendingEquippedId)
            {
                _equippedPickaxe = p;
                Debug.Log($"[PickaxeManager] 장착 곡괭이 복원: {p.pickaxeId}");
                return;
            }
        }
        Debug.LogWarning($"[PickaxeManager] 장착 곡괭이 SO를 찾지 못했습니다: {_pendingEquippedId}");
    }
}