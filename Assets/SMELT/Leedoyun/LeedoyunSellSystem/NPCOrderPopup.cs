using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// NPC와 E키 상호작용 시 열리는 주문 팝업.
/// 담당자: 이도윤
/// </summary>
public class NPCOrderPopup : MonoBehaviour
{
    public static NPCOrderPopup Instance { get; private set; }

    [Header("폰트")]
    [SerializeField] private TMP_FontAsset _koreanFont;
    private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    // ─────────────────────────────────────────
    // 색상
    // ─────────────────────────────────────────
    private static readonly Color CLR_BG          = new Color(0.05f, 0.07f, 0.05f, 0.96f);
    private static readonly Color CLR_ORDER_BG    = new Color(0.10f, 0.18f, 0.10f, 1f);
    private static readonly Color CLR_BTN_DELIVER = new Color(0.12f, 0.50f, 0.18f, 1f);
    private static readonly Color CLR_BTN_DISABLE = new Color(0.18f, 0.18f, 0.20f, 1f);
    private static readonly Color CLR_TIMER_OK    = new Color(0.22f, 0.80f, 0.28f, 1f);
    private static readonly Color CLR_TIMER_WARN  = new Color(0.90f, 0.65f, 0.10f, 1f);
    private static readonly Color CLR_TIMER_CRIT  = new Color(0.85f, 0.18f, 0.18f, 1f);
    private static readonly Color CLR_CLOSE       = new Color(0.45f, 0.12f, 0.12f, 1f);

    // ─────────────────────────────────────────
    // UI 참조
    // ─────────────────────────────────────────
    private GameObject           _root;
    private TextMeshProUGUI      _weaponTxt;
    private TextMeshProUGUI      _goldTxt;
    private TextMeshProUGUI      _timerTxt;
    private Image                _timerFill;
    private Button               _deliverBtn;
    private Image                _deliverBtnImg;
    private TextMeshProUGUI      _deliverBtnTxt;

    private Leedoyun_CustomerOrder _currentOrder;
    private bool _isOpen;

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
        BuildUI();
        _root.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (_root != null) Destroy(_root);
    }

    // ─────────────────────────────────────────
    // 공개 API
    // ─────────────────────────────────────────
    public void Open(Leedoyun_CustomerOrder order)
    {
        if (order == null || !order.IsActive) return;
        _currentOrder = order;
        _isOpen = true;
        _root.SetActive(true);
        PlayerMovement.IsLocked = true;
        Refresh();
    }

    public void Close()
    {
        _isOpen = false;
        _currentOrder = null;
        _root.SetActive(false);
        PlayerMovement.IsLocked = false;
    }

    // ─────────────────────────────────────────
    // 업데이트
    // ─────────────────────────────────────────
    private void Update()
    {
        if (!_isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            Close();
            return;
        }

        if (_currentOrder == null || !_currentOrder.IsActive)
        {
            Close();
            return;
        }

        UpdateTimerBar();
        RefreshDeliverButton();
    }

    // ─────────────────────────────────────────
    // 갱신
    // ─────────────────────────────────────────
    private void Refresh()
    {
        if (_currentOrder == null) return;
        _weaponTxt.text = WeaponDisplayName(_currentOrder.requestedWeaponId);
        _goldTxt.text   = $"보상: {_currentOrder.rewardGold:N0} G";
        UpdateTimerBar();
        RefreshDeliverButton();
    }

    private void UpdateTimerBar()
    {
        if (_timerFill == null || _currentOrder == null) return;
        float ratio = _currentOrder.RemainingRatio;
        var rt = _timerFill.rectTransform;
        rt.anchorMax = new Vector2(ratio, 1f);
        rt.offsetMax = Vector2.zero;
        _timerFill.color = ratio > 0.5f ? CLR_TIMER_OK :
                           ratio > 0.25f ? CLR_TIMER_WARN : CLR_TIMER_CRIT;

        if (_timerTxt != null)
            _timerTxt.text = $"남은 시간: {Mathf.CeilToInt(_currentOrder.RemainingTime)}초";
    }

    private void RefreshDeliverButton()
    {
        bool canDeliver = _currentOrder != null &&
                          InventoryManager.Instance != null &&
                          InventoryManager.Instance.HasItem(_currentOrder.requestedWeaponId);
        _deliverBtnImg.color     = canDeliver ? CLR_BTN_DELIVER : CLR_BTN_DISABLE;
        _deliverBtnTxt.text      = canDeliver ? "납품" : "재고 없음";
        _deliverBtn.interactable = canDeliver;
    }

    // ─────────────────────────────────────────
    // 납품
    // ─────────────────────────────────────────
    private void OnDeliverClicked()
    {
        if (_currentOrder == null || !_currentOrder.IsActive) return;
        Leedoyun_SellManager.Instance?.FulfillOrder(
            _currentOrder.orderId, _currentOrder.requestedWeaponId);
        Close();
    }

    // ─────────────────────────────────────────
    // UI 빌드
    // ─────────────────────────────────────────
    private void BuildUI()
    {
#if UNITY_EDITOR
        if (_koreanFont == null)
            _koreanFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
#endif

        // 전용 Canvas 생성 (기존 Canvas에 붙이면 sort order 문제 발생)
        Transform canvasT = BuildCanvas();

        const float W = 320f;
        const float H = 240f;

        _root = MakePanel(canvasT, "NPCOrderPopup",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(W, H), CLR_BG);
        _root.GetComponent<RectTransform>().SetAsLastSibling();

        var root = _root.transform;

        // 헤더
        var header = MakeTxt(root, "손님 주문", 16, FontStyles.Bold,
            new Color(0.5f, 0.9f, 0.5f),
            new Vector2(-20f, H / 2f - 20f), new Vector2(W - 60f, 30f));
        header.alignment = TextAlignmentOptions.Center;

        // 닫기 버튼
        MakeBtn(root, "X",
            new Vector2(W / 2f - 22f, H / 2f - 20f), new Vector2(32f, 32f),
            CLR_CLOSE, Close);

        // 구분선
        MakePanel(root, "HLine",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, H / 2f - 42f), new Vector2(W - 20f, 1f),
            new Color(0.28f, 0.30f, 0.28f, 0.6f));

        // 주문 내용 영역
        var orderBg = MakePanel(root, "OrderBg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, 20f), new Vector2(W - 20f, 120f), CLR_ORDER_BG);
        var orderT = orderBg.transform;

        _weaponTxt = MakeTxt(orderT, "", 15, FontStyles.Bold, Color.white,
            new Vector2(0f, 36f), new Vector2(W - 40f, 24f));
        _weaponTxt.alignment = TextAlignmentOptions.Center;

        _goldTxt = MakeTxt(orderT, "", 13, FontStyles.Normal, new Color(1f, 0.85f, 0.2f),
            new Vector2(0f, 12f), new Vector2(W - 40f, 20f));
        _goldTxt.alignment = TextAlignmentOptions.Center;

        _timerTxt = MakeTxt(orderT, "", 11, FontStyles.Normal, new Color(0.7f, 0.7f, 0.7f),
            new Vector2(0f, -10f), new Vector2(W - 40f, 18f));
        _timerTxt.alignment = TextAlignmentOptions.Center;

        // 타이머 바
        var timerBgGO = MakePanel(orderT, "TimerBg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, -38f), new Vector2(W - 60f, 8f),
            new Color(0.08f, 0.08f, 0.08f));

        var timerFillGO = new GameObject("Fill");
        timerFillGO.transform.SetParent(timerBgGO.transform, false);
        var fillRt = timerFillGO.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
        _timerFill = timerFillGO.AddComponent<Image>();
        _timerFill.color = CLR_TIMER_OK;

        // 납품 버튼
        _deliverBtn = MakeBtn(root, "납품",
            new Vector2(0f, -H / 2f + 34f), new Vector2(W - 40f, 44f),
            CLR_BTN_DELIVER, OnDeliverClicked);
        _deliverBtnImg = _deliverBtn.GetComponent<Image>();
        _deliverBtnTxt = _deliverBtn.GetComponentInChildren<TextMeshProUGUI>();

        // 안내 텍스트
        var closeTxt = MakeTxt(root, "[E] / [ESC]  닫기", 10, FontStyles.Normal,
            new Color(0.45f, 0.45f, 0.45f),
            new Vector2(0f, -H / 2f + 9f), new Vector2(W - 20f, 14f));
        closeTxt.alignment = TextAlignmentOptions.Center;
    }

    // ─────────────────────────────────────────
    // UI 헬퍼
    // ─────────────────────────────────────────
    private Transform BuildCanvas()
    {
        var go = new GameObject("Canvas");
        var c  = go.AddComponent<Canvas>();
        c.renderMode   = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 300;
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
        var go  = MakePanel(parent, $"Btn_{label}",
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
        tmp.text = label; tmp.fontSize = 14; tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.Center;
        return btn;
    }

    // ─────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────
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
}
