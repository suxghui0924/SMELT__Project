using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 주문 HUD — 화면 우측 상단에 항상 표시.
/// 플레이어가 어느 구역에 있든 손님 주문을 확인하고 납품할 수 있습니다.
///
/// [레이아웃] 우측 상단 고정
///   ┌──────────────────────────────┐
///   │ 📋 손님 주문                 │
///   │ ┌──────────────────────────┐ │
///   │ │ [무기  ] 사과석 검        │ │
///   │ │ [이미지] 1,500 G  [납품]  │ │
///   │ │         ██████░░ (타이머) │ │
///   │ └──────────────────────────┘ │
///   │ ┌──────────────────────────┐ │
///   │ │       대기 중...          │ │
///   │ └──────────────────────────┘ │
///   └──────────────────────────────┘
///
/// [이미지 설정]
///   Inspector의 _weaponSprites (5개, 검/도끼/창/방망이/건틀릿 순)에
///   무기 스프라이트를 할당하면 주문 슬롯 아이콘에 표시됩니다.
///   비워두면 색상 박스로 대체됩니다.
///
/// 담당자: 이도윤
/// </summary>
public class OrderHUD : MonoBehaviour
{
    public static OrderHUD Instance { get; private set; }

    // ─────────────────────────────────────────
    // Inspector 설정
    // ─────────────────────────────────────────
    [Header("폰트")]
    [SerializeField] private TMP_FontAsset _koreanFont;
    private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    [Header("무기 이미지 (검 / 도끼 / 창 / 방망이 / 건틀릿 순)")]
    [Tooltip("할당하면 주문 슬롯 아이콘에 표시됩니다. 비워두면 색상 박스 사용.")]
    [SerializeField] private Sprite[] _weaponSprites = new Sprite[5];

    // ─────────────────────────────────────────
    // 레이아웃 상수
    // ─────────────────────────────────────────
    public  const float HUD_W       = 260f;
    public  const float PANEL_TOTAL_W = HUD_W + 16f; // SellZoneUI 위치 계산용
    private const float ORDER_H    = 96f;
    private const int   SLOT_COUNT = 3;
    private const float SLOT_GAP   = 5f;

    // ─────────────────────────────────────────
    // 색상 상수
    // ─────────────────────────────────────────
    private static readonly Color CLR_BG_PANEL     = new Color(0.05f, 0.07f, 0.05f, 0.92f);
    private static readonly Color CLR_ORDER_ACTIVE = new Color(0.10f, 0.18f, 0.10f, 1f);
    private static readonly Color CLR_ORDER_EMPTY  = new Color(0.07f, 0.09f, 0.07f, 0.85f);
    private static readonly Color CLR_BTN_DELIVER  = new Color(0.12f, 0.50f, 0.18f, 1f);
    private static readonly Color CLR_BTN_DISABLED = new Color(0.18f, 0.18f, 0.20f, 1f);
    private static readonly Color CLR_TIMER_OK     = new Color(0.22f, 0.80f, 0.28f, 1f);
    private static readonly Color CLR_TIMER_WARN   = new Color(0.90f, 0.65f, 0.10f, 1f);
    private static readonly Color CLR_TIMER_CRIT   = new Color(0.85f, 0.18f, 0.18f, 1f);

    // ─────────────────────────────────────────
    // 데이터 테이블
    // ─────────────────────────────────────────
    private static readonly string[] WEAPON_TYPE_IDS = { "sword", "axe", "spear", "bat", "gauntlet" };

    // ─────────────────────────────────────────
    // 내부 슬롯 클래스
    // ─────────────────────────────────────────
    private class OrderSlotUI
    {
        public Image             bg;
        public Image             weaponIcon;
        public TextMeshProUGUI   weaponTxt;
        public TextMeshProUGUI   goldTxt;
        public Image             timerFill;
        public GameObject        timerBg;
        public Button            btn;
        public Image             btnImg;
        public TextMeshProUGUI   btnTxt;
        public GameObject        emptyLabel;
        public Leedoyun_CustomerOrder order;
    }

    private OrderSlotUI[] _slots = new OrderSlotUI[SLOT_COUNT];

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
        // 0.5초마다 납품 버튼 상태 갱신 (인벤토리 변경 반영)
        InvokeRepeating(nameof(RefreshDeliverButtons), 0.5f, 0.5f);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        UnsubscribeEvents();
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
                return;
            }
        }
    }

    private void HandleOrderFulfilled(Leedoyun_CustomerOrder order, int gold) => RemoveOrder(order);
    private void HandleOrderExpired(Leedoyun_CustomerOrder order) => RemoveOrder(order);

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
        slot.btn.gameObject.SetActive(hasOrder);
        slot.weaponIcon.gameObject.SetActive(hasOrder);

        if (!hasOrder) return;

        slot.weaponTxt.text = WeaponDisplayName(slot.order.requestedWeaponId);
        slot.goldTxt.text   = $"{slot.order.rewardGold:N0} G";

        // 무기 아이콘 (Inspector에서 _weaponSprites 배열에 스프라이트 할당 시 표시)
        int typeIdx = GetWeaponTypeIndex(slot.order.requestedWeaponId);
        Sprite spr = (typeIdx >= 0 && _weaponSprites != null && typeIdx < _weaponSprites.Length)
            ? _weaponSprites[typeIdx] : null;
        if (spr != null)
        {
            slot.weaponIcon.sprite = spr;
            slot.weaponIcon.color  = Color.white;
        }

        RefreshDeliverButton(i);
    }

    private void RefreshDeliverButtons()
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order != null && _slots[i].order.IsActive)
                RefreshDeliverButton(i);
        }
    }

    private void RefreshDeliverButton(int i)
    {
        var slot = _slots[i];
        if (slot.order == null || slot.btn == null) return;
        bool canDeliver = InventoryManager.Instance != null &&
                          InventoryManager.Instance.HasItem(slot.order.requestedWeaponId);
        slot.btnImg.color     = canDeliver ? CLR_BTN_DELIVER : CLR_BTN_DISABLED;
        slot.btnTxt.text      = canDeliver ? "납품" : "재고 없음";
        slot.btn.interactable = canDeliver;
    }

    private void UpdateTimerBars()
    {
        foreach (var slot in _slots)
        {
            if (slot.order == null || !slot.order.IsActive || slot.timerFill == null) continue;
            float ratio = slot.order.RemainingRatio;
            // anchorMax.x를 비율로 조정 → 바가 왼쪽부터 줄어드는 효과
            var rt = slot.timerFill.rectTransform;
            rt.anchorMax  = new Vector2(ratio, 1f);
            rt.offsetMax  = Vector2.zero; // offset 초기화(안 하면 늘어날 수 있음)
            slot.timerFill.color = ratio > 0.5f ? CLR_TIMER_OK :
                                   ratio > 0.25f ? CLR_TIMER_WARN : CLR_TIMER_CRIT;
        }
    }

    // ─────────────────────────────────────────
    // 납품 처리
    // ─────────────────────────────────────────
    private void OnDeliverClicked(int slotIdx)
    {
        var slot = _slots[slotIdx];
        if (slot.order == null || !slot.order.IsActive) return;

        Leedoyun_SellManager.Instance?.FulfillOrder(
            slot.order.orderId, slot.order.requestedWeaponId);
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

        Canvas canvas = FindFirstObjectByType<Canvas>();
        Transform canvasT = canvas != null ? canvas.transform : BuildCanvas();

        float headerH = 30f;
        float panelW  = HUD_W + 16f;
        float panelH  = headerH + 8f + SLOT_COUNT * ORDER_H + (SLOT_COUNT - 1) * SLOT_GAP + 8f;

        // 루트 패널 — 우측 상단에 고정 (anchor & pivot 모두 top-right)
        var rootGO = MakePanel(canvasT, "OrderHUD",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-panelW / 2f - 10f, -panelH / 2f - 10f),
            new Vector2(panelW, panelH), CLR_BG_PANEL);
        rootGO.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        // pivot 변경 후 위치 재조정
        rootGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, -160f);

        var root = rootGO.transform;

        // 헤더
        var header = MakeTxt(root, "손님 주문", 14, FontStyles.Bold,
            new Color(0.5f, 0.9f, 0.5f),
            new Vector2(0f, panelH / 2f - headerH / 2f - 4f),
            new Vector2(HUD_W, headerH));
        header.alignment = TextAlignmentOptions.Center;

        // 슬롯 배치 (헤더 아래부터 아래로 순서대로)
        float firstSlotCenterY = panelH / 2f - headerH - 8f - ORDER_H / 2f;
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            float slotCenterY = firstSlotCenterY - i * (ORDER_H + SLOT_GAP);
            _slots[i] = BuildSlot(root, i, new Vector2(0f, slotCenterY));
        }
    }

    private OrderSlotUI BuildSlot(Transform parent, int idx, Vector2 centerPos)
    {
        int captured = idx;

        var cardGO = MakePanel(parent, $"OrderSlot_{idx}",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            centerPos, new Vector2(HUD_W, ORDER_H), CLR_ORDER_EMPTY);
        var p    = cardGO.transform;
        var slot = new OrderSlotUI { bg = cardGO.GetComponent<Image>() };

        // 빈 상태 라벨
        var empty = MakeTxt(p, "대기 중...", 12, FontStyles.Normal,
            new Color(0.30f, 0.32f, 0.30f), Vector2.zero, new Vector2(HUD_W - 16f, 24f));
        empty.alignment = TextAlignmentOptions.Center;
        slot.emptyLabel = empty.gameObject;

        // ── 무기 아이콘 ──────────────────────────
        // Inspector의 _weaponSprites 배열에 스프라이트를 할당하면 이 영역에 표시됩니다.
        float iconSize = ORDER_H - 12f;
        float iconCenterX = -HUD_W / 2f + iconSize / 2f + 6f;

        var iconGO = MakePanel(p, "WeaponIcon",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(iconCenterX, 0f),
            new Vector2(iconSize, iconSize),
            new Color(0.12f, 0.16f, 0.12f));
        slot.weaponIcon = iconGO.GetComponent<Image>();
        slot.weaponIcon.preserveAspect = true;
        slot.weaponIcon.gameObject.SetActive(false);

        // ── 텍스트 / 타이머 영역 ─────────────────
        float btnW      = 64f;
        float textLeft  = iconCenterX + iconSize / 2f + 6f;
        float textRight = HUD_W / 2f - btnW - 8f;
        float textCX    = (textLeft + textRight) / 2f;
        float textW2    = textRight - textLeft;

        slot.weaponTxt = MakeTxt(p, "", 12, FontStyles.Bold, Color.white,
            new Vector2(textCX, 28f), new Vector2(textW2, 18f));
        slot.weaponTxt.alignment = TextAlignmentOptions.Left;
        slot.weaponTxt.gameObject.SetActive(false);

        slot.goldTxt = MakeTxt(p, "", 11, FontStyles.Bold, new Color(1f, 0.85f, 0.2f),
            new Vector2(textCX, 10f), new Vector2(textW2, 16f));
        slot.goldTxt.alignment = TextAlignmentOptions.Left;
        slot.goldTxt.gameObject.SetActive(false);

        // 타이머 바 (anchorMax.x 방식 — Image.Type.Filled보다 확실하게 동작)
        var timerBgGO = MakePanel(p, "TimerBg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(textCX, -8f), new Vector2(textW2, 7f),
            new Color(0.08f, 0.08f, 0.08f));
        slot.timerBg = timerBgGO;
        timerBgGO.SetActive(false);

        // 채움 영역: anchorMin=(0,0) anchorMax=(1,1) → Update에서 anchorMax.x=ratio로 조정
        var timerFillGO = new GameObject("Fill");
        timerFillGO.transform.SetParent(timerBgGO.transform, false);
        var fillRt = timerFillGO.AddComponent<RectTransform>();
        fillRt.anchorMin  = Vector2.zero;
        fillRt.anchorMax  = Vector2.one;
        fillRt.offsetMin  = fillRt.offsetMax = Vector2.zero;
        slot.timerFill = timerFillGO.AddComponent<Image>();
        slot.timerFill.color = CLR_TIMER_OK;

        // ── 납품 버튼 ────────────────────────────
        float btnCX = HUD_W / 2f - btnW / 2f - 5f;
        slot.btn = MakeBtn(p, "납품",
            new Vector2(btnCX, 0f), new Vector2(btnW, ORDER_H - 14f),
            CLR_BTN_DELIVER, () => OnDeliverClicked(captured));
        slot.btnImg = slot.btn.GetComponent<Image>();
        slot.btnTxt = slot.btn.GetComponentInChildren<TextMeshProUGUI>();
        slot.btn.gameObject.SetActive(false);

        return slot;
    }

    // ─────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────
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
            "bat"      => "방망이",
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
        c.renderMode   = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 10;
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

    private Button MakeBtn(Transform parent, string label, Vector2 pos, Vector2 size,
        Color color, System.Action onClick)
    {
        var go  = MakePanel(parent, "Btn",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size, color);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        btn.onClick.AddListener(() => onClick());

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(go.transform, false);
        var rt = txtGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        if (_koreanFont != null) tmp.font = _koreanFont;
        tmp.text = label; tmp.fontSize = 12f; tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.Center;
        return btn;
    }
}
