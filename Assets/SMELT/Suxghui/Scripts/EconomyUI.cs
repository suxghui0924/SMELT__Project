using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using static UnityEngine.Rendering.DebugUI;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _appleText;
    [SerializeField] TextMeshProUGUI _lemonText;
    [SerializeField] TextMeshProUGUI _grapeText;
    [SerializeField] TextMeshProUGUI _orangeText;
    [SerializeField] TextMeshProUGUI _melonText;
    [SerializeField] TextMeshProUGUI _curDayText;
    [SerializeField] TextMeshProUGUI _maintenanceText;
    List<TextMeshProUGUI> itemTexts = new List<TextMeshProUGUI>();
    List<string> itemIds = new List<string> { "fruitstone_apple", "fruitstone_melon", "fruitstone_orange", "fruitstone_lemon", "fruitstone_grape" };

    void Awake()
    {
        itemTexts = new List<TextMeshProUGUI> { _appleText, _melonText, _lemonText, _grapeText, _orangeText };
    }
    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnGoldChanged += UpdateUIGoldState;
            UpdateUIGoldState(0, InventoryManager.Instance.Gold);
            InventoryManager.Instance.OnItemChanged += UpdateUIItemState;
            InitItemTexts();
            InventoryManager.Instance.OnDayChanged += UpdateUICurDayMaintenanceCost;
            //UpdateUIItemState(0, InventoryManager.Instance.Gold); 
        }
    }

    private void InitItemTexts()
    {
        for (int i = 0; i < itemTexts.Count; i++)
        {
            int qty = InventoryManager.Instance.GetQuantity(itemIds[i]);
            UpdateUIItemState(itemIds[i], qty);
        }
    }
    // 아래의 스크립트는 빠른 테스트를 위해 테스트 케이스를 만듬 { Gemini }
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            InventoryManager.Instance.AddGold(1000000);
        }

        if (Input.GetKey(KeyCode.Alpha1))
            InventoryManager.Instance.AddItem("fruitstone_apple", 10);

        if (Input.GetKey(KeyCode.Alpha2))
            InventoryManager.Instance.AddItem("fruitstone_melon", 10);

        if (Input.GetKey(KeyCode.Alpha3))
            InventoryManager.Instance.AddItem("fruitstone_orange", 10);

        if (Input.GetKey(KeyCode.Alpha4))
            InventoryManager.Instance.AddItem("fruitstone_lemon", 10);

        if (Input.GetKey(KeyCode.Alpha5))
            InventoryManager.Instance.AddItem("fruitstone_grape", 10);

        // [R] 키: 모든 과일석 100개씩 추가 (폭풍 테스트용)
        if (Input.GetKey(KeyCode.R))
        {
            foreach (var id in itemIds)
            {
                InventoryManager.Instance.AddItem(id, 100);
            }
        }

        // [Enter] 키 또는 [N] 키: 하루 종료 (날짜 증가 + 유지비 차감)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.N))
        {
            if (InventoryManager.Instance != null)
            {
                // EndOfDay는 유지비가 부족하면 false를 반환하도록 설계되어 있습니다.
                bool success = InventoryManager.Instance.EndOfDay();

                if (success)
                {
                    Debug.Log($"[Economy] {InventoryManager.Instance.CurrentDay}일차가 되었습니다.");
                }
                else
                {
                    Debug.LogWarning("[Economy] 골드가 부족하여 다음 날로 넘어갈 수 없습니다!");
                }
            }
        }
    }

    private void UpdateUIGoldState(int curGold, int newGold)
    {
        _goldText.text = $"{Mathf.Clamp(newGold, 0, 200000000).ToString("N0")}";
    }
    private void UpdateUIItemState(string itemId, int newValue)
    {
        if (itemIds.IndexOf(itemId) != -1)
        {
            itemTexts[itemIds.IndexOf(itemId)].text = $"{Mathf.Clamp(newValue, 0, 10000).ToString("N0")}";
        }
        //_goldText.text = $"{newGold.ToString("N0")}";
    }

    public void UpdateUICurDayMaintenanceCost(int curDay, int maintenance)
    {
        _curDayText.text = $"{curDay} 일차";
        _maintenanceText.text = $"유지비용 : {maintenance.ToString("N0")}";
    }
}
