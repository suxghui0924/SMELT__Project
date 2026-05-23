using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;

public enum ZoneType { Mining, Crafting, Selling, SkillTree, MineEntrance, StoreRadioZone, StoreStateZone, NextDay, Door }

public class PrototypeZone : MonoBehaviour
{
    [SerializeField] private ZoneType _zoneType;
    public ZoneType ZoneType { get => _zoneType; set => _zoneType = value; }

    [SerializeField] private GameObject _doorObject;

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
    private bool  _isDoorOpen = false;

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
        {
            if (SkillTreeController.Instance != null)
                SkillTreeController.Instance.Toggle();
            else if (UICanvasManager.instance != null)
                UICanvasManager.instance.ControlObject(ObjectType.ShopASkill, true);
        }
        if (ZoneType == ZoneType.MineEntrance)
        {
            if (TimerAndReward.Instance != null && !TimerAndReward.Instance.canEnter) return;
            if (TimerAndReward.Instance != null) TimerAndReward.Instance.db = true;
            if (GameManager.instance != null)
                GameManager.instance.ChangeState(new MiningState());
        }

        if (ZoneType == ZoneType.StoreRadioZone)
            UICanvasManager.instance.ControlObject(ObjectType.Radio, true);        
        if (ZoneType == ZoneType.StoreStateZone)
            UICanvasManager.instance.ControlObject(ObjectType.Radio, true);
        if (ZoneType == ZoneType.NextDay)
            UICanvasManager.instance.ControlObject(ObjectType.DayNext, true);

        if (ZoneType == ZoneType.Door)
        {
            _isDoorOpen = !_isDoorOpen;
            if (_doorObject != null)
                _doorObject.SetActive(!_isDoorOpen);
            if (ShopManager.Instance != null)
            {
                if (_isDoorOpen) ShopManager.Instance.OpenShop();
                else             ShopManager.Instance.CloseShop();
            }
            string hint = _isDoorOpen ? "문  —  [ E ] 문 닫기" : "문  —  [ E ] 문 열기";
            if (PrototypeHUD.Instance != null) PrototypeHUD.Instance.SetZoneHint(hint);
        }
    }
}
