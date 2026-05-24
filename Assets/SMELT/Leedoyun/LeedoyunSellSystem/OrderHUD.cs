using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 주문 HUD — 화면 우측 상단에 항상 표시.
/// 담당자: 이도윤
/// </summary>
public class OrderHUD : MonoBehaviour
{
    public static OrderHUD Instance { get; private set; }

    // ─────────────────────────────────────────
    // 외부 이벤트
    // ─────────────────────────────────────────
    /// <summary>새 주문이 HUD 슬롯에 등록될 때 발생합니다.</summary>
    public static event Action<Leedoyun_CustomerOrder> OnOrderCreated;
    /// <summary>주문이 납품 완료되거나 시간 초과로 종료될 때 발생합니다.</summary>
    public static event Action<Leedoyun_CustomerOrder> OnOrderEnded;

    // ─────────────────────────────────────────
    // Inspector 설정
    // ─────────────────────────────────────────
    [Header("폰트")]
    [SerializeField] private TMP_FontAsset _koreanFont;
    private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    [Header("무기 이미지 (검 / 도끼 / 창 / 망치 / 건틀릿 순)")]
    [SerializeField] private Sprite[] _weaponSprites = new Sprite[5];

    // ─────────────────────────────────────────
    // 레이아웃 상수
    // ─────────────────────────────────────────
    public  const float HUD_W         = 260f;
    public  const float PANEL_TOTAL_W = HUD_W + 16f;
    private const float ORDER_H       = 96f;
    private const int   SLOT_COUNT    = 3;
    private const float SLOT_GAP      = 5f;

    // ─────────────────────────────────────────
    // 색상 상수 (WeaponCraftUI 갈색 RPG 톤)
    // ─────────────────────────────────────────
    private static readonly Color CLR_BG_PANEL    = new Color(0.35f, 0.25f, 0.14f, 0.97f);
    private static readonly Color CLR_ORDER_ACTIVE = new Color(0.26f, 0.18f, 0.10f, 1f);
    private static readonly Color CLR_ORDER_EMPTY  = new Color(0.20f, 0.14f, 0.08f, 0.85f);
    private static readonly Color CLR_STOCK_OK     = new Color(0.18f, 0.50f, 0.22f, 1f);
    private static readonly Color CLR_STOCK_NONE   = new Color(0.32f, 0.24f, 0.15f, 1f);
    private static readonly Color CLR_LABEL        = new Color(0.93f, 0.85f, 0.65f);
    private static readonly Color CLR_GOLD         = new Color(1.00f, 0.85f, 0.20f);
    private static readonly Color CLR_DIVIDER      = new Color(0.60f, 0.45f, 0.25f, 0.6f);
    private static readonly Color CLR_TIMER_OK     = new Color(0.22f, 0.80f, 0.28f, 1f);
    private static readonly Color CLR_TIMER_WARN   = new Color(0.90f, 0.65f, 0.10f, 1f);
    private static readonly Color CLR_TIMER_CRIT   = new Color(0.85f, 0.18f, 0.18f, 1f);
    private static readonly Color CLR_HEADER_TXT   = new Color(0.93f, 0.85f, 0.65f);
    private static readonly Color CLR_EMPTY_TXT    = new Color(0.55f, 0.45f, 0.30f);

    // ─────────────────────────────────────────
    // 데이터 테이블
    // ─────────────────────────────────────────
    private static readonly string[] WEAPON_TYPE_IDS = { "sword", "axe", "spear", "hammer", "gauntlet" };

    // 광석 ID → 색상 폴백용 (WeaponCraftUI 없을 때 무기 실루엣에 색 입힘)
    private static readonly string[] ORE_SHORT_IDS = { "apple", "melon", "orange", "lemon", "grape" };
    private static readonly Color[] ORE_COLORS =
    {
        new Color(1.00f, 0.25f, 0.25f), // apple  - 빨강
        new Color(0.25f, 0.85f, 0.25f), // melon  - 초록
        new Color(1.00f, 0.55f, 0.15f), // orange - 주황
        new Color(1.00f, 0.90f, 0.15f), // lemon  - 노랑
        new Color(0.65f, 0.25f, 1.00f), // grape  - 보라
    };

    // ─────────────────────────────────────────
    // 내부 슬롯 클래스
    // ─────────────────────────────────────────
    private class OrderSlotUI
    {
        public Image             bg;
        public GameObject        weaponIconFrame;
        public Image             weaponIcon;
        public TextMeshProUGUI   weaponTxt;
        public TextMeshProUGUI   goldTxt;
        public Image             timerFill;
        public GameObject        timerBg;
        public TextMeshProUGUI   timeTxt;
        public Image             stockBg;
        public TextMeshProUGUI   stockTxt;
        public GameObject        emptyLabel;
        public Leedoyun_CustomerOrder order;
    }

    private readonly OrderSlotUI[] _slots = new OrderSlotUI[SLOT_COUNT];
    private GameObject _hudRoot;
    private const string HOUSE_SCENE = "House";

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
        BuildHUD();
        SubscribeEvents();
        SyncExistingOrders();
        InvokeRepeating(nameof(RefreshStockAll), 0.5f, 0.5f);
        SceneManager.activeSceneChanged += OnSceneChanged;

        bool isHouseNow = SceneManager.GetActiveScene().name == HOUSE_SCENE;
        if (_hudRoot != null)
            _hudRoot.SetActive(isHouseNow);
        if (isHouseNow && UICanvasManager.instance != null)
        {
            UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
            UICanvasManager.instance.ControlObject(ObjectType.Bottom, true);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        UnsubscribeEvents();
        SceneManager.activeSceneChanged -= OnSceneChanged;
        if (_hudRoot != null) Destroy(_hudRoot);
    }

    private void OnSceneChanged(Scene _, Scene next)
    {
        bool isHouse = next.name == HOUSE_SCENE;
        if (_hudRoot != null)
            _hudRoot.SetActive(isHouse);

        if (UICanvasManager.instance == null) return;

        if (isHouse)
        {
            UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
            UICanvasManager.instance.ControlObject(ObjectType.Bottom, true);
        }

        if (next.name == "Lobby")
            UICanvasManager.instance.SetCanvasActive(CanvasType.Title, true);
    }

    private void Update()
    {
        UpdateTimerBars();
    }

    // ─────────────────────────────────────────
    // 공개 API
    // ─────────────────────────────────────────
    public void RefreshAll()
    {
        SyncExistingOrders();
    }

    // ─────────────────────────────────────────
    // 이벤트 구독
    // ─────────────────────────────────────────
    private void SubscribeEvents()
    {
        var sm = Leedoyun_SellManager.Instance;
        if (sm == null) return;
        sm.OnOrderAdded     += HandleOrderAdded;
        sm.OnOrderFulfilled += HandleOrderFulfilled;
        sm.OnOrderExpired   += HandleOrderExpired;
    }

    private void UnsubscribeEvents()
    {
        var sm = Leedoyun_SellManager.Instance;
        if (sm == null) return;
        sm.OnOrderAdded     -= HandleOrderAdded;
        sm.OnOrderFulfilled -= HandleOrderFulfilled;
        sm.OnOrderExpired   -= HandleOrderExpired;
    }

    private void HandleOrderAdded(Leedoyun_CustomerOrder order)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order == null)
            {
                _slots[i].order = order;
                RefreshSlot(i);
                OnOrderCreated?.Invoke(order);
                return;
            }
        }
    }

    private void HandleOrderFulfilled(Leedoyun_CustomerOrder order, int gold)
    {
        RemoveOrder(order);
        OnOrderEnded?.Invoke(order);
    }

    private void HandleOrderExpired(Leedoyun_CustomerOrder order)
    {
        RemoveOrder(order);
        OnOrderEnded?.Invoke(order);
    }

    private void RemoveOrder(Leedoyun_CustomerOrder order)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order == order)
            {
                _slots[i].order = null;
                RefreshSlot(i);
                return;
            }
        }
    }

    private void SyncExistingOrders()
    {
        for (int i = 0; i < SLOT_COUNT; i++) _slots[i].order = null;
        var sm = Leedoyun_SellManager.Instance;
        if (sm == null) return;
        int idx = 0;
        foreach (var order in sm.ActiveOrders)
        {
            if (idx >= SLOT_COUNT) break;
            _slots[idx++].order = order;
        }
        for (int i = 0; i < SLOT_COUNT; i++) RefreshSlot(i);
    }

    // ─────────────────────────────────────────
    // 슬롯 갱신
    // ─────────────────────────────────────────
    private void RefreshSlot(int i)
    {
        var slot = _slots[i];
        bool hasOrder = slot.order != null && slot.order.IsActive;

        slot.bg.color = hasOrder ? CLR_ORDER_ACTIVE : CLR_ORDER_EMPTY;
        slot.emptyLabel.SetActive(!hasOrder);
        slot.weaponTxt.gameObject.SetActive(hasOrder);
        slot.goldTxt.gameObject.SetActive(hasOrder);
        slot.timerBg.SetActive(hasOrder);
        slot.timeTxt.gameObject.SetActive(hasOrder);
        slot.stockBg.gameObject.SetActive(hasOrder);
        slot.weaponIconFrame.SetActive(hasOrder);

        if (!hasOrder) return;

        slot.weaponTxt.text = WeaponDisplayName(slot.order.requestedWeaponId);

        float salesMult = PlayerStatManager.Instance != null
            ? (1f + PlayerStatManager.Instance.UpMoreSell) : 1f;
        int displayGold = Mathf.RoundToInt(slot.order.rewardGold * salesMult);
        slot.goldTxt.text = $"{displayGold:N0} G";

        SetWeaponIcon(slot, slot.order.requestedWeaponId);

        RefreshStock(i);
    }

    private void RefreshStockAll()
    {
        float salesMult = PlayerStatManager.Instance != null
            ? (1f + PlayerStatManager.Instance.UpMoreSell) : 1f;

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order == null || !_slots[i].order.IsActive) continue;
            RefreshStock(i);
            int displayGold = Mathf.RoundToInt(_slots[i].order.rewardGold * salesMult);
            _slots[i].goldTxt.text = $"{displayGold:N0} G";

            // WeaponCraftUI가 이제 생겼으면 고화질 스프라이트로 교체
            if (WeaponCraftUI.Instance != null)
            {
                Sprite spr = WeaponCraftUI.Instance.GetWeaponSprite(_slots[i].order.requestedWeaponId);
                if (spr != null && _slots[i].weaponIcon.sprite != spr)
                {
                    _slots[i].weaponIcon.sprite = spr;
                    _slots[i].weaponIcon.color  = Color.white;
                }
            }
        }
    }

    private void RefreshStock(int i)
    {
        var slot = _slots[i];
        if (slot.order == null || slot.stockBg == null) return;

        bool hasStock = InventoryManager.Instance != null &&
                        InventoryManager.Instance.HasItem(slot.order.requestedWeaponId);

        slot.stockBg.color   = hasStock ? CLR_STOCK_OK : CLR_STOCK_NONE;
        slot.stockTxt.text   = hasStock ? "재고 있음" : "재고 없음";
        slot.stockTxt.color  = hasStock ? Color.white : new Color(0.65f, 0.55f, 0.40f);
    }

    private void UpdateTimerBars()
    {
        foreach (var slot in _slots)
        {
            if (slot.order == null || !slot.order.IsActive || slot.timerFill == null) continue;
            float ratio = slot.order.RemainingRatio;
            var rt = slot.timerFill.rectTransform;
            rt.anchorMax  = new Vector2(ratio, 1f);
            rt.offsetMax  = Vector2.zero;
            slot.timerFill.color = ratio > 0.5f ? CLR_TIMER_OK :
                                   ratio > 0.25f ? CLR_TIMER_WARN : CLR_TIMER_CRIT;

            if (slot.timeTxt != null)
            {
                int sec = Mathf.CeilToInt(slot.order.RemainingTime);
                slot.timeTxt.text  = $"{sec}초 남음";
                slot.timeTxt.color = ratio > 0.5f ? CLR_TIMER_OK :
                                     ratio > 0.25f ? CLR_TIMER_WARN : CLR_TIMER_CRIT;
            }
        }
    }

    // ─────────────────────────────────────────
    // UI 빌드
    // ─────────────────────────────────────────
    private void BuildHUD()
    {
#if UNITY_EDITOR
        if (_koreanFont == null)
            _koreanFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
#endif

        Transform canvasT = BuildCanvas();

        float headerH = 30f;
        float panelW  = HUD_W + 16f;
        float panelH  = headerH + 8f + SLOT_COUNT * ORDER_H + (SLOT_COUNT - 1) * SLOT_GAP + 8f;

        _hudRoot = MakePanel(canvasT, "OrderHUD",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-panelW / 2f - 10f, -panelH / 2f - 10f),
            new Vector2(panelW, panelH), CLR_BG_PANEL);
        _hudRoot.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        _hudRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -160f);

        var root = _hudRoot.transform;

        var header = MakeTxt(root, "손님 주문", 14, FontStyles.Bold,
            CLR_HEADER_TXT,
            new Vector2(0f, panelH / 2f - headerH / 2f - 4f),
            new Vector2(HUD_W, headerH));
        header.alignment = TextAlignmentOptions.Center;

        MakePanel(root, "HLine",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, panelH / 2f - headerH - 2f),
            new Vector2(HUD_W, 1f), CLR_DIVIDER);

        float firstSlotCenterY = panelH / 2f - headerH - 8f - ORDER_H / 2f;
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            float slotCenterY = firstSlotCenterY - i * (ORDER_H + SLOT_GAP);
            _slots[i] = BuildSlot(root, i, new Vector2(0f, slotCenterY));
        }
    }

    private OrderSlotUI BuildSlot(Transform parent, int idx, Vector2 centerPos)
    {
        var cardGO = MakePanel(parent, $"OrderSlot_{idx}",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            centerPos, new Vector2(HUD_W, ORDER_H), CLR_ORDER_EMPTY);
        var p    = cardGO.transform;
        var slot = new OrderSlotUI { bg = cardGO.GetComponent<Image>() };

        // 빈 상태 라벨
        var empty = MakeTxt(p, "대기 중...", 12, FontStyles.Normal,
            CLR_EMPTY_TXT, Vector2.zero, new Vector2(HUD_W - 16f, 24f));
        empty.alignment = TextAlignmentOptions.Center;
        slot.emptyLabel = empty.gameObject;

        // 무기 아이콘
        float iconSize    = ORDER_H - 12f;
        float iconCenterX = -HUD_W / 2f + iconSize / 2f + 6f;

        // 배경 프레임
        var iconFrameGO = MakePanel(p, "WeaponIconFrame",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(iconCenterX, 0f),
            new Vector2(iconSize, iconSize),
            new Color(0.18f, 0.12f, 0.06f));
        slot.weaponIconFrame = iconFrameGO;
        iconFrameGO.SetActive(false);

        // 스프라이트 Image — 프레임 내부에 padding 4 적용, 비율 유지
        var iconSpriteGO = new GameObject("WeaponSprite");
        iconSpriteGO.transform.SetParent(iconFrameGO.transform, false);
        var sprRt = iconSpriteGO.AddComponent<RectTransform>();
        sprRt.anchorMin = Vector2.zero;
        sprRt.anchorMax = Vector2.one;
        sprRt.offsetMin = new Vector2(4f, 4f);
        sprRt.offsetMax = new Vector2(-4f, -4f);
        slot.weaponIcon = iconSpriteGO.AddComponent<Image>();
        slot.weaponIcon.preserveAspect = true;
        slot.weaponIcon.color = new Color(0f, 0f, 0f, 0f);

        // 재고 상태 뱃지 (우측)
        float badgeW  = 60f;
        float badgeCX = HUD_W / 2f - badgeW / 2f - 4f;

        var stockGO = MakePanel(p, "StockBadge",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(badgeCX, 0f), new Vector2(badgeW, ORDER_H - 14f),
            CLR_STOCK_NONE);
        slot.stockBg  = stockGO.GetComponent<Image>();
        slot.stockTxt = MakeTxt(stockGO.transform, "재고 없음", 10, FontStyles.Bold,
            new Color(0.65f, 0.55f, 0.40f),
            Vector2.zero, new Vector2(badgeW - 4f, ORDER_H - 18f));
        slot.stockTxt.alignment = TextAlignmentOptions.Center;
        slot.stockBg.gameObject.SetActive(false);

        // 텍스트 영역
        float textLeft  = iconCenterX + iconSize / 2f + 6f;
        float textRight = HUD_W / 2f - badgeW - 8f;
        float textCX    = (textLeft + textRight) / 2f;
        float textW     = textRight - textLeft;

        slot.weaponTxt = MakeTxt(p, "", 12, FontStyles.Bold, CLR_LABEL,
            new Vector2(textCX, 28f), new Vector2(textW, 18f));
        slot.weaponTxt.alignment = TextAlignmentOptions.Left;
        slot.weaponTxt.gameObject.SetActive(false);

        slot.goldTxt = MakeTxt(p, "", 11, FontStyles.Bold, CLR_GOLD,
            new Vector2(textCX, 10f), new Vector2(textW, 16f));
        slot.goldTxt.alignment = TextAlignmentOptions.Left;
        slot.goldTxt.gameObject.SetActive(false);

        // 타이머 바
        var timerBgGO = MakePanel(p, "TimerBg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(textCX, -8f), new Vector2(textW, 7f),
            new Color(0.12f, 0.08f, 0.04f));
        slot.timerBg = timerBgGO;
        timerBgGO.SetActive(false);

        var timerFillGO = new GameObject("Fill");
        timerFillGO.transform.SetParent(timerBgGO.transform, false);
        var fillRt = timerFillGO.AddComponent<RectTransform>();
        fillRt.anchorMin  = Vector2.zero;
        fillRt.anchorMax  = Vector2.one;
        fillRt.offsetMin  = fillRt.offsetMax = Vector2.zero;
        slot.timerFill = timerFillGO.AddComponent<Image>();
        slot.timerFill.color = CLR_TIMER_OK;

        slot.timeTxt = MakeTxt(p, "", 10, FontStyles.Normal, CLR_TIMER_OK,
            new Vector2(textCX, -22f), new Vector2(textW, 14f));
        slot.timeTxt.alignment = TextAlignmentOptions.Left;
        slot.timeTxt.gameObject.SetActive(true);

        return slot;
    }

    // ─────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────

    /// <summary>
    /// 무기 아이콘 표시.
    /// WeaponCraftUI가 있으면 25종 고화질 스프라이트 사용,
    /// 없으면 무기 실루엣(_weaponSprites)에 광석 색상을 입혀 표시.
    /// </summary>
    private void SetWeaponIcon(OrderSlotUI slot, string weaponItemId)
    {
        slot.weaponIcon.sprite = null;
        slot.weaponIcon.color  = new Color(0f, 0f, 0f, 0f);

        // 1순위: WeaponCraftUI의 25종 스프라이트
        if (WeaponCraftUI.Instance != null)
        {
            Sprite spr = WeaponCraftUI.Instance.GetWeaponSprite(weaponItemId);
            if (spr != null)
            {
                slot.weaponIcon.sprite = spr;
                slot.weaponIcon.color  = Color.white;
                return;
            }
        }

        // 2순위: 무기 실루엣 + 광석 색상 폴백
        string[] p = weaponItemId.Split('_');
        if (p.Length < 3) return;

        int wi = System.Array.IndexOf(WEAPON_TYPE_IDS, p[1]);
        int oi = System.Array.IndexOf(ORE_SHORT_IDS,   p[2]);

        if (wi >= 0 && _weaponSprites != null && wi < _weaponSprites.Length && _weaponSprites[wi] != null)
        {
            slot.weaponIcon.sprite = _weaponSprites[wi];
            slot.weaponIcon.color  = oi >= 0 ? ORE_COLORS[oi] : Color.white;
        }
    }

    private static int GetWeaponTypeIndex(string weaponItemId)
    {
        string[] p = weaponItemId.Split('_');
        if (p.Length < 2) return -1;
        for (int i = 0; i < WEAPON_TYPE_IDS.Length; i++)
            if (WEAPON_TYPE_IDS[i] == p[1]) return i;
        return -1;
    }

    private static string WeaponDisplayName(string weaponItemId)
    {
        string[] p = weaponItemId.Split('_');
        if (p.Length < 3) return weaponItemId;
        string weapon = p[1] switch
        {
            "sword"    => "검",
            "axe"      => "도끼",
            "spear"    => "창",
            "hammer"   => "망치",
            "gauntlet" => "건틀릿",
            _          => p[1],
        };
        string ore = p[2] switch
        {
            "apple"  => "사과석",
            "melon"  => "멜론석",
            "orange" => "귤석",
            "lemon"  => "레몬석",
            "grape"  => "포도석",
            _        => p[2],
        };
        return $"{ore} {weapon}";
    }

    // ─────────────────────────────────────────
    // UI 헬퍼
    // ─────────────────────────────────────────
    private Transform BuildCanvas()
    {
        var go = new GameObject("Canvas");
        var c  = go.AddComponent<Canvas>();
        c.renderMode        = RenderMode.ScreenSpaceOverlay;
        c.sortingLayerName  = "UI";
        c.sortingOrder      = 10;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight  = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return go.transform;
    }

    private GameObject MakePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.AddComponent<Image>().color = color;
        return go;
    }

    private TextMeshProUGUI MakeTxt(Transform parent, string text, float size,
        FontStyles style, Color color, Vector2 pos, Vector2 sizeDelta)
    {
        var go = new GameObject("Txt");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = sizeDelta;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (_koreanFont != null) tmp.font = _koreanFont;
        tmp.text = text; tmp.fontSize = size; tmp.fontStyle = style;
        tmp.color = color; tmp.raycastTarget = false;
        return tmp;
    }
}
