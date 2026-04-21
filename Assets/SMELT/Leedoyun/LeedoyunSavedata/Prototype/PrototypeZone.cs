using UnityEngine;

public enum ZoneType { Mining, Crafting, Selling }

/// <summary>
/// 각 구역의 동작 정의.
///
/// Mining  : 플레이어가 머무는 동안 1.5초마다 랜덤 과일석 +1 자동 채집
/// Crafting: E키 → 제작 패널 열기/닫기
/// Selling : E키 → 판매 패널 열기/닫기
/// </summary>
public class PrototypeZone : MonoBehaviour
{
    public ZoneType ZoneType { get; set; }

    private const float GATHER_INTERVAL = 1.5f;

    private static readonly string[] ORE_IDS =
    {
        "fruitstone_apple",
        "fruitstone_melon",
        "fruitstone_orange",
        "fruitstone_lemon",
        "fruitstone_grape",
    };

    private bool  _playerInside;
    private float _gatherTimer;

    private void Update()
    {
        if (ZoneType != ZoneType.Mining || !_playerInside) return;

        _gatherTimer += Time.deltaTime;
        if (_gatherTimer < GATHER_INTERVAL) return;

        _gatherTimer = 0f;
        string id = ORE_IDS[Random.Range(0, ORE_IDS.Length)];
        InventoryManager.Instance?.AddItem(id, 1);
        PrototypeHUD.Instance?.OnOreGathered(id);
    }

    public void OnPlayerEnter()
    {
        _playerInside = true;
        _gatherTimer  = 0f;
        PrototypeHUD.Instance?.OnZoneEnter(ZoneType);
    }

    public void OnPlayerExit()
    {
        _playerInside = false;
        PrototypeHUD.Instance?.OnZoneExit(ZoneType);

        // 제작 UI는 대장간에서 나가면 닫기 (판매 UI는 항상 표시)
        if (ZoneType == ZoneType.Crafting)
            WeaponCraftUI.Instance?.Hide();
    }

    public void Interact()
    {
        if (ZoneType == ZoneType.Crafting)
        {
            // 새 WeaponCraftUI가 씬에 있으면 우선 사용, 없으면 기존 프로토타입 패널 fallback
            if (WeaponCraftUI.Instance != null)
                WeaponCraftUI.Instance.Toggle();
            else
                PrototypeHUD.Instance?.ToggleCraftPanel();
        }
        // 판매 UI는 항상 우측에 표시되므로 별도 토글 불필요
    }
}
