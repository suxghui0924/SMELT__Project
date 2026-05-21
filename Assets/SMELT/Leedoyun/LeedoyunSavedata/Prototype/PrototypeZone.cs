using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;

public enum ZoneType { Mining, Crafting, Selling, SkillTree, MineEntrance }

/// <summary>
/// 각 구역의 동작 정의.
///
/// Mining   : 플레이어가 머무는 동안 1.5초마다 랜덤 과일석 +1 자동 채집
/// Crafting : E키 → 무기 제작 패널 열기/닫기
/// Selling  : E키 → 판매 패널 열기/닫기
/// SkillTree: E키 → 스킬 트리 패널 열기/닫기 (SkillTreeController)
/// </summary>
public class PrototypeZone : MonoBehaviour
{
    [SerializeField] private ZoneType _zoneType;
    public ZoneType ZoneType { get => _zoneType; set => _zoneType = value; }

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

        if (ZoneType == ZoneType.Crafting)
            WeaponCraftUI.Instance?.Hide();
        if (ZoneType == ZoneType.SkillTree)
            SkillTreeController.Instance?.Hide();
    }

    public void Interact()
    {
        if (ZoneType == ZoneType.Crafting)
        {
            if (WeaponCraftUI.Instance != null)
                WeaponCraftUI.Instance.Toggle();
            else
                PrototypeHUD.Instance?.ToggleCraftPanel();
        }
        if (ZoneType == ZoneType.SkillTree)
            SkillTreeController.Instance?.Toggle();
        if (ZoneType == ZoneType.MineEntrance)
        {
            GameManager.instance.ChangeState(new MiningState());
            if (!TimerAndReward.Instance.canEnter) return;
            TimerAndReward.Instance.db = true;
        }
    }
}
