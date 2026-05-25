using _01_Scripts._Core._States;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;

public enum ZoneType { Mining, Crafting, Selling, SkillTree, MineEntrance, StoreRadioZone, StoreStateZone, NextDay, Door, Portal, StoreBroken }

public class PrototypeZone : MonoBehaviour
{
    [SerializeField] private ZoneType _zoneType;
    public ZoneType ZoneType { get => _zoneType; set => _zoneType = value; }

    [SerializeField] private GameObject        _doorObject;
    [SerializeField] private SkillTreeZoneVisual _skillTreeVisual;

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
    private bool  _isDoorOpen       = false;
    private bool  _isRadioOpen     = false;
    private bool  _isDayNextOpen   = false;
    private bool  _isSkillOpen     = false;
    private bool  _isStoreBrokenOpen = false;

    private void Start()
    {
        if (_zoneType == ZoneType.Door)
        {
            _isDoorOpen = ShopManager.Instance != null && ShopManager.Instance.IsShopOpen;
            if (_doorObject != null)
                _doorObject.SetActive(!_isDoorOpen);
        }
    }

    private void OnEnable()
    {
        LeedoyunUIManager.OnAnyUIOpened += OnLeedoyunUIOpened;
        Leedoyun_SellManager.OnShopClosed += OnShopForceClosed;
        Leedoyun_SellManager.OnShopOpened += OnShopForceOpened;
    }

    private void OnDisable()
    {
        LeedoyunUIManager.OnAnyUIOpened -= OnLeedoyunUIOpened;
        Leedoyun_SellManager.OnShopClosed -= OnShopForceClosed;
        Leedoyun_SellManager.OnShopOpened -= OnShopForceOpened;
    }

    private void OnShopForceClosed()
    {
        if (_zoneType != ZoneType.Door) return;
        _isDoorOpen = false;
        if (_doorObject != null)
            _doorObject.SetActive(true); // 문 닫힘 = 오브젝트 활성
    }

    private void OnShopForceOpened()
    {
        if (_zoneType != ZoneType.Door) return;
        _isDoorOpen = true;
        if (_doorObject != null)
            _doorObject.SetActive(false); // 문 열림 = 오브젝트 비활성
    }

    // Leedoyun UI가 열릴 때 라디오·달력·스킬 팝업을 닫고 상태 초기화
    private void OnLeedoyunUIOpened()
    {
        if (_isRadioOpen)
        {
            _isRadioOpen = false;
            UICanvasManager.instance?.ControlObject(ObjectType.Radio, false);
        }
        if (_isDayNextOpen)
        {
            _isDayNextOpen = false;
            UICanvasManager.instance?.ControlObject(ObjectType.DayNext, false);
        }
        if (_isSkillOpen)
        {
            _isSkillOpen = false;
            UICanvasManager.instance?.ControlObject(ObjectType.ShopASkill, false);
        }
    }

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

        if (ZoneType == ZoneType.Door && PrototypeHUD.Instance != null)
        {
            string hint = _isDoorOpen ? "문  —  [ E ] 문 닫기" : "문  —  [ E ] 문 열기";
            PrototypeHUD.Instance.SetZoneHint(hint);
        }
    }

    public void OnPlayerExit()
    {
        _playerInside = false;
        PrototypeHUD.Instance?.OnZoneExit(ZoneType);

        // Zone을 벗어나면 모든 UI 닫기
        LeedoyunUIManager.CloseAll();
        WeaponCraftUI.Instance?.Hide();
        NPCOrderPopup.Instance?.Close();
        SkillTreeController.Instance?.Hide();
        SettingUI.Instance?.ClosePanel();

        UICanvasManager.instance?.ControlObject(ObjectType.ShopASkill, false);
        UICanvasManager.instance?.ControlObject(ObjectType.Radio, false);
        UICanvasManager.instance?.ControlObject(ObjectType.DayNext, false);
        UICanvasManager.instance?.ControlObject(ObjectType.Setting, false);
        if (UICanvasManager.instance != null)
            UICanvasManager.instance.ControlObject(ObjectType.StoreBroken, false);
        _isSkillOpen       = false;
        _isRadioOpen       = false;
        _isDayNextOpen     = false;
        _isStoreBrokenOpen = false;

        if (ZoneType == ZoneType.SkillTree)
            _skillTreeVisual?.SetInteracting(false);
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
            {
                SkillTreeController.Instance.Toggle();
                _skillTreeVisual?.SetInteracting(SkillTreeController.Instance.IsOpen);
            }
            else if (UICanvasManager.instance != null)
            {
                _isSkillOpen = !_isSkillOpen;
                UICanvasManager.instance.ControlObject(ObjectType.ShopASkill, _isSkillOpen);
            }
        }
        if (ZoneType == ZoneType.MineEntrance)
        {
            if (TimerAndReward.Instance != null && !TimerAndReward.Instance.canEnter) return;
            if (TimerAndReward.Instance != null) TimerAndReward.Instance.db = true;
            /*if (InventoryManager.Instance != null && GameManager.instance != null)
            {
                if (InventoryManager.Instance.CurrentDay == 7)
                    AchievementManager.Instance.AlarmPopUp("채광장이 막혀있습니다.", "오늘은 채광을 할 수 없습니다, 포탈로 이동하여 보스전을 진행하세요.");
                else
                    GameManager.instance.ChangeState(new MiningState());
            }*/
            if (GameManager.instance != null)
                GameManager.instance.ChangeState(new MiningState());
        }

        if (ZoneType == ZoneType.StoreRadioZone || ZoneType == ZoneType.StoreStateZone)
        {
            _isRadioOpen = !_isRadioOpen;
            if (_isRadioOpen) LeedoyunUIManager.CloseAll();
            UICanvasManager.instance?.ControlObject(ObjectType.Radio, _isRadioOpen);
        }
        if (ZoneType == ZoneType.NextDay)
        {
            _isDayNextOpen = !_isDayNextOpen;
            if (_isDayNextOpen) LeedoyunUIManager.CloseAll();
            UICanvasManager.instance?.ControlObject(ObjectType.DayNext, _isDayNextOpen);
        }

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

        if (ZoneType == ZoneType.Portal)
        {
            if (GameManager.instance != null)
                GameManager.instance.ChangeState(new BossState());
        }

        if (ZoneType == ZoneType.StoreBroken)
        {
            _isStoreBrokenOpen = !_isStoreBrokenOpen;
            if (_isStoreBrokenOpen) LeedoyunUIManager.CloseAll();
            if (UICanvasManager.instance != null)
                UICanvasManager.instance.ControlObject(ObjectType.StoreBroken, _isStoreBrokenOpen);
        }
    }
}
