using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _appleText;
    [SerializeField] TextMeshProUGUI _melonText;
    [SerializeField] TextMeshProUGUI _orangeText;
    [SerializeField] TextMeshProUGUI _lemonText;
    [SerializeField] TextMeshProUGUI _grapeText;
    [SerializeField] TextMeshProUGUI _curDayText;
    [SerializeField] TextMeshProUGUI _maintenanceText;

    // itemIds 순서와 반드시 일치해야 함
    private List<TextMeshProUGUI> _itemTexts;
    private readonly List<string> _itemIds = new()
    {
        "fruitstone_apple", "fruitstone_melon", "fruitstone_orange",
        "fruitstone_lemon",  "fruitstone_grape"
    };

    private void Awake()
    {
        _itemTexts = new List<TextMeshProUGUI>
        {
            _appleText, _melonText, _orangeText, _lemonText, _grapeText
        };
    }

    private void Start()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        inv.OnGoldChanged += UpdateUIGoldState;
        inv.OnItemChanged += UpdateUIItemState;
        inv.OnDayChanged  += UpdateUICurDayMaintenanceCost;

        // 로드 후 UI 전체 갱신
        if (SaveManager.Instance != null)
            SaveManager.Instance.OnLoadResult += OnLoaded;

        RefreshAll();
    }

    private void OnDestroy()
    {
        var inv = InventoryManager.Instance;
        if (inv != null)
        {
            inv.OnGoldChanged -= UpdateUIGoldState;
            inv.OnItemChanged -= UpdateUIItemState;
            inv.OnDayChanged  -= UpdateUICurDayMaintenanceCost;
        }

        if (SaveManager.Instance != null)
            SaveManager.Instance.OnLoadResult -= OnLoaded;
    }

    private void OnLoaded(bool ok, string _)
    {
        if (ok) RefreshAll();
    }

    private void RefreshAll()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        UpdateUIGoldState(0, inv.Gold);
        for (int i = 0; i < _itemIds.Count; i++)
            UpdateUIItemState(_itemIds[i], inv.GetQuantity(_itemIds[i]));
        UpdateUICurDayMaintenanceCost(inv.CurrentDay, inv.MaintenanceCost);
    }

    private void UpdateUIGoldState(int _, int newGold)
    {
        if (_goldText != null)
            _goldText.text = Mathf.Clamp(newGold, 0, 200_000_000).ToString("N0");
    }

    private void UpdateUIItemState(string itemId, int newValue)
    {
        int idx = _itemIds.IndexOf(itemId);
        if (idx >= 0 && _itemTexts[idx] != null)
            _itemTexts[idx].text = Mathf.Clamp(newValue, 0, 10_000).ToString("N0");
    }

    public void UpdateUICurDayMaintenanceCost(int curDay, int maintenance)
    {
        if (_curDayText != null)      _curDayText.text      = $"{curDay} 일차";
        if (_maintenanceText != null) _maintenanceText.text = $"유지비용 : {maintenance:N0}";
    }

    // ─── 테스트 입력 (빠른 확인용) ───────────────────────────────
    private void Update()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        if (Input.GetKeyDown(KeyCode.Space))        inv.AddGold(1_000_000);
        if (Input.GetKeyDown(KeyCode.Alpha1))       inv.AddItem("fruitstone_apple",  10);
        if (Input.GetKeyDown(KeyCode.Alpha2))       inv.AddItem("fruitstone_melon",  10);
        if (Input.GetKeyDown(KeyCode.Alpha3))       inv.AddItem("fruitstone_orange", 10);
        if (Input.GetKeyDown(KeyCode.Alpha4))       inv.AddItem("fruitstone_lemon",  10);
        if (Input.GetKeyDown(KeyCode.Alpha5))       inv.AddItem("fruitstone_grape",  10);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.N))
        {
            if (!inv.EndOfDay())
                Debug.LogWarning("[Economy] 골드 부족 - 다음 날로 넘어갈 수 없습니다.");
        }
    }
}
