using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// [이미지 설정]
///   _weaponSprites 25개: 사과×5 → 멜론×5 → 귤×5 → 레몬×5 → 포도×5
///   _oreSprites    5개 : 사과 / 멜론 / 귤 / 레몬 / 포도
///
/// [가격 공식]
///   (무기 기본금 + 메인 가치 × 메인 개수) × (1 + moreSell)
///
/// 담당자: 이도윤
/// </summary>
public class WeaponCraftUI : MonoBehaviour
{
    public static WeaponCraftUI Instance { get; private set; }

    // ─────────────────────────────────────────
    // Inspector 설정
    // ─────────────────────────────────────────
    [Header("폰트")]
    [Tooltip("한국어 TMP 폰트. 비워두면 에셋에서 자동 로드.")]
    [SerializeField] private TMP_FontAsset _koreanFont;

    [Header("뒷배경 이미지 (비워두면 단색 사용)")]
    [SerializeField] private Sprite _bgSprite;

    private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    [Header("무기 종류 버튼 아이콘 5개 (검 / 도끼 / 창 / 망치 / 건틀릿 — 흑백 실루엣 권장)")]
    [SerializeField] private Sprite[] _weaponTypeSprites = new Sprite[5];

    [Header("무기 이미지 25개 (사과_검~사과_건틀릿 / 멜론_검~멜론_건틀릿 / ... / 포도_검~포도_건틀릿)")]
    [Tooltip("순서: 광석(5) × 무기(5) = 25개.  사과 5개 → 멜론 5개 → 귤 5개 → 레몬 5개 → 포도 5개")]
    [SerializeField] private Sprite[] _weaponSprites = new Sprite[25];

    [Header("과일석 이미지 (사과 / 멜론 / 귤 / 레몬 / 포도 순)")]
    [SerializeField] private Sprite[] _oreSprites = new Sprite[5];

    // ─────────────────────────────────────────
    // 레이아웃 상수
    // ─────────────────────────────────────────
    private const float PANEL_W  = 860f;
    private const float PANEL_H  = 500f;
    private const float BTN_SIZE = 80f;
    private const float BTN_GAP  = 10f;
    private const float SEC_LEFT = -155f;
    private const float PREV_X   = 295f;
    private const float PREV_W   = 200f;
    private const float PREV_H   = 400f;

    // ─────────────────────────────────────────
    // 데이터 테이블
    // ─────────────────────────────────────────
    private static readonly WeaponType[] WEAPON_TYPES =
    {
        WeaponType.Sword, WeaponType.Axe, WeaponType.Spear,
        WeaponType.Hammer, WeaponType.Gauntlet
    };
    private static readonly string[] WEAPON_NAMES  = { "검", "도끼", "창", "망치", "건틀릿" };
    private static readonly Color[]  WEAPON_COLORS =
    {
        new Color(0.85f, 0.35f, 0.35f),
        new Color(0.65f, 0.45f, 0.20f),
        new Color(0.30f, 0.60f, 0.90f),
        new Color(0.55f, 0.30f, 0.80f),
        new Color(0.25f, 0.75f, 0.50f),
    };

    private static readonly string[] ORE_IDS =
    {
        "fruitstone_apple", "fruitstone_melon", "fruitstone_orange",
        "fruitstone_lemon",  "fruitstone_grape"
    };
    private static readonly string[] ORE_NAMES  = { "사과석", "멜론석", "귤석", "레몬석", "포도석" };
    private static readonly Color[]  ORE_COLORS =
    {
        new Color(1.00f, 0.25f, 0.25f),
        new Color(0.25f, 0.85f, 0.30f),
        new Color(1.00f, 0.58f, 0.10f),
        new Color(0.95f, 0.92f, 0.20f),
        new Color(0.60f, 0.20f, 0.95f),
    };

    // 수정: 가격 공식에 쓰이는 광석 가치 (사과/멜론/귤/레몬/포도)
    private static readonly int[] ORE_VALUES = { 1000, 2000, 3000, 4000, 5000 };

    // ─────────────────────────────────────────
    // 색상 상수 (따뜻한 나무/모래 RPG 톤)
    // ─────────────────────────────────────────
    private static readonly Color CLR_BG         = new Color(0.35f, 0.25f, 0.14f, 0.97f);
    private static readonly Color CLR_PREVIEW_BG = new Color(0.26f, 0.18f, 0.10f);
    private static readonly Color CLR_BTN_NORMAL = new Color(0.48f, 0.35f, 0.20f);
    private static readonly Color CLR_BTN_SEL    = new Color(0.68f, 0.52f, 0.20f);
    private static readonly Color CLR_BTN_DIM    = new Color(0.26f, 0.18f, 0.10f);
    private static readonly Color CLR_OUTLINE_ON  = new Color(1.00f, 0.85f, 0.20f);
    private static readonly Color CLR_OUTLINE_OFF = new Color(0f, 0f, 0f, 0f);
    private static readonly Color CLR_CRAFT_OK   = new Color(0.18f, 0.50f, 0.22f);
    private static readonly Color CLR_CRAFT_FAIL = new Color(0.32f, 0.24f, 0.15f);
    private static readonly Color CLR_LABEL      = new Color(0.93f, 0.85f, 0.65f);
    private static readonly Color CLR_DIVIDER    = new Color(0.60f, 0.45f, 0.25f, 0.6f);

    // ─────────────────────────────────────────
    // 선택 상태
    // ─────────────────────────────────────────
    private int _selWeapon = 0;
    private int _selMain   = 0;
    private int _selSub    = 1;

    // ─────────────────────────────────────────
    // UI 참조
    // ─────────────────────────────────────────
    private GameObject      _rootPanel;
    private Button[]        _weaponBtns = new Button[5];
    private Outline[]       _weaponOuts = new Outline[5];
    private Button[]        _mainBtns   = new Button[5];
    private Outline[]       _mainOuts   = new Outline[5];
    private Button[]        _subBtns    = new Button[5];
    private Outline[]       _subOuts    = new Outline[5];

    private Image           _previewImg;
    private TextMeshProUGUI _previewName;
    private TextMeshProUGUI _previewMat;
    private TextMeshProUGUI _previewPrice;
    private TextMeshProUGUI _previewHold;
    private Button          _craftBtn;
    private Image           _craftBtnImg;
    private TextMeshProUGUI _craftBtnTxt;
    private TextMeshProUGUI _statusTxt;

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
        RefreshAll();
        _rootPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show()   { if (_rootPanel != null) { LeedoyunUIManager.NotifyOpen(Hide); _rootPanel.SetActive(true); RefreshAll(); } }
    public void Hide()   { if (_rootPanel != null) _rootPanel.SetActive(false); }
    public void Toggle() { if (_rootPanel != null && _rootPanel.activeSelf) Hide(); else Show(); }

    // ─────────────────────────────────────────
    // UI 빌드
    // ─────────────────────────────────────────
    private void BuildUI()
    {
#if UNITY_EDITOR
        if (_koreanFont == null)
            _koreanFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
#endif
        Transform canvasT = BuildCanvas();

        _rootPanel = MakePanel(canvasT, "WeaponCraftUI",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(PANEL_W, PANEL_H), CLR_BG);
        if (_bgSprite != null)
        {
            var bgImg = _rootPanel.GetComponent<Image>();
            bgImg.sprite = _bgSprite;
            bgImg.type   = Image.Type.Sliced;
            bgImg.color  = new Color(1f, 1f, 1f, 0.97f);
        }
        var root = _rootPanel.transform;

        float topY = PANEL_H / 2f - 28f;

        var title = MakeTxt(root, "대장간", 24, FontStyles.Bold, Color.white,
            new Vector2(-10f, topY - 18f), new Vector2(700f, 40f));
        title.alignment = TextAlignmentOptions.Left;

        MakeBtn(root, "X",
            new Vector2(PANEL_W / 2f - 30f, topY), new Vector2(40f, 40f),
            new Color(0.45f, 0.12f, 0.12f), Hide);

        MakePanel(root, "HLine",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, topY - 26f), new Vector2(PANEL_W - 20f, 1f), CLR_DIVIDER);

        const float LBL_CENTER_X = -370f;
        const float LBL_W        = 90f;
        float rowStartX = LBL_CENTER_X + LBL_W / 2f + 10f + BTN_SIZE / 2f;

        float FIRST_ROW_Y = topY - 140f;
        float ROW_GAP     = BTN_SIZE + 18f;

        // 무기 종류 행 — 흑백 실루엣 아이콘 사용
        MakeSecLabel(root, "무기 종류", new Vector2(LBL_CENTER_X, FIRST_ROW_Y), LBL_W);
        BuildButtonRow(root, _weaponBtns, _weaponOuts,
            WEAPON_NAMES, _weaponTypeSprites, WEAPON_COLORS,
            rowStartX, FIRST_ROW_Y, i => OnWeaponSelected(i));

        float row2Y = FIRST_ROW_Y - ROW_GAP;
        MakeSecLabel(root, "메인 과일석", new Vector2(LBL_CENTER_X, row2Y), LBL_W);
        BuildButtonRow(root, _mainBtns, _mainOuts,
            ORE_NAMES, _oreSprites, ORE_COLORS,
            rowStartX, row2Y, i => OnMainOreSelected(i));

        float row3Y = row2Y - ROW_GAP;
        MakeSecLabel(root, "서브 과일석", new Vector2(LBL_CENTER_X, row3Y), LBL_W);
        BuildButtonRow(root, _subBtns, _subOuts,
            ORE_NAMES, _oreSprites, ORE_COLORS,
            rowStartX, row3Y, i => OnSubOreSelected(i));

        _statusTxt = MakeTxt(root, "", 14, FontStyles.Normal,
            new Color(0.7f, 0.7f, 0.7f),
            new Vector2(SEC_LEFT, -PANEL_H / 2f + 22f), new Vector2(580f, 22f));
        _statusTxt.alignment = TextAlignmentOptions.Left;

        BuildPreviewPanel(root);
    }

    // 수정: iconImgsOut 파라미터 제거 (동적 아이콘 교체 제거로 버튼 사라짐 버그 수정)
    private void BuildButtonRow(Transform root,
        Button[] btns, Outline[] outs,
        string[] names, Sprite[] sprites, Color[] colors,
        float startX, float y, System.Action<int> onSelect)
    {
        for (int i = 0; i < 5; i++)
        {
            int idx = i;
            float x = startX + i * (BTN_SIZE + BTN_GAP);
            var (btn, outline) = BuildIconBtn(root,
                names[i], SafeSprite(sprites, i), colors[i],
                new Vector2(x, y), new Vector2(BTN_SIZE, BTN_SIZE),
                () => onSelect(idx));
            btns[i] = btn;
            outs[i] = outline;
        }
    }

    private void BuildPreviewPanel(Transform root)
    {
        var panel = MakePanel(root, "PreviewPanel",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(PREV_X, -10f), new Vector2(PREV_W, PREV_H),
            CLR_PREVIEW_BG).transform;

        float imgSize = 100f;
        float imgY    = PREV_H / 2f - imgSize / 2f - 16f;
        var imgGO = MakePanel(panel, "WeaponImg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, imgY), new Vector2(imgSize, imgSize),
            new Color(0.22f, 0.15f, 0.08f));
        _previewImg = imgGO.GetComponent<Image>();
        _previewImg.preserveAspect = true;

        float txtY = imgY - imgSize / 2f - 20f;

        _previewName = MakeTxt(panel, "???", 19, FontStyles.Bold, Color.white,
            new Vector2(0f, txtY), new Vector2(PREV_W - 16f, 28f));
        _previewName.alignment = TextAlignmentOptions.Center;
        txtY -= 36f;

        MakePanel(panel, "MatLine",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, txtY + 6f), new Vector2(PREV_W - 24f, 1f), CLR_DIVIDER);
        txtY -= 8f;

        _previewMat = MakeTxt(panel, "", 13, FontStyles.Normal,
            new Color(0.55f, 0.90f, 0.60f),
            new Vector2(0f, txtY), new Vector2(PREV_W - 16f, 46f));
        _previewMat.alignment = TextAlignmentOptions.Center;
        txtY -= 56f;

        MakePanel(panel, "PriceLine",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, txtY + 6f), new Vector2(PREV_W - 24f, 1f), CLR_DIVIDER);
        txtY -= 8f;

        _previewPrice = MakeTxt(panel, "", 15, FontStyles.Bold,
            new Color(1f, 0.85f, 0.20f),
            new Vector2(0f, txtY), new Vector2(PREV_W - 16f, 24f));
        _previewPrice.alignment = TextAlignmentOptions.Center;
        txtY -= 28f;

        _previewHold = MakeTxt(panel, "", 12, FontStyles.Normal,
            new Color(0.60f, 0.60f, 0.65f),
            new Vector2(0f, txtY), new Vector2(PREV_W - 16f, 20f));
        _previewHold.alignment = TextAlignmentOptions.Center;

        float btnY = -PREV_H / 2f + 34f;
        _craftBtn    = MakeBtn(panel, "제작",
            new Vector2(0f, btnY), new Vector2(PREV_W - 20f, 40f),
            CLR_CRAFT_OK, OnCraftClicked);
        _craftBtnImg = _craftBtn.GetComponent<Image>();
        _craftBtnTxt = _craftBtn.GetComponentInChildren<TextMeshProUGUI>();
    }

    // ─────────────────────────────────────────
    // 아이콘 버튼 생성
    // ─────────────────────────────────────────
    // 수정: 반환 타입을 (Button, Outline) 으로 복구 — Image 반환 불필요
    private (Button, Outline) BuildIconBtn(Transform parent, string label,
        Sprite sprite, Color fallbackColor, Vector2 pos, Vector2 size,
        System.Action onClick)
    {
        var go  = MakePanel(parent, $"Btn_{label}",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            pos, size, CLR_BTN_NORMAL);
        var img = go.GetComponent<Image>();
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => onClick());

        var t = go.transform;

        float iconH   = size.y - 20f;
        Color bgColor = sprite != null ? new Color(0f, 0f, 0f, 0f) : fallbackColor;
        var iconGO  = MakePanel(t, "Icon",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -12f), new Vector2(size.x - 10f, iconH),
            bgColor);
        var iconImg = iconGO.GetComponent<Image>();
        iconImg.raycastTarget = false;
        if (sprite != null)
        {
            iconImg.sprite = sprite;
            iconImg.color  = Color.white;
            iconImg.preserveAspect = true;
        }

        var lbl = MakeTxt(t, label, 11, FontStyles.Bold, Color.white,
            new Vector2(0f, -(size.y / 2f - 8f)), new Vector2(size.x - 4f, 16f));
        lbl.alignment = TextAlignmentOptions.Center;

        var outline = go.AddComponent<Outline>();
        outline.effectColor    = CLR_OUTLINE_OFF;
        outline.effectDistance = new Vector2(3f, -3f);

        return (btn, outline);
    }

    // ─────────────────────────────────────────
    // 선택 이벤트
    // ─────────────────────────────────────────
    private void OnWeaponSelected(int idx)
    {
        _selWeapon = idx;
        RefreshAll();
    }

    private void OnMainOreSelected(int idx)
    {
        _selMain = idx;
        if (_selSub == _selMain)
            _selSub = (_selMain + 1) % 5;
        RefreshAll();
    }

    private void OnSubOreSelected(int idx)
    {
        _selSub = idx;
        RefreshAll();
    }

    // ─────────────────────────────────────────
    // 제작
    // ─────────────────────────────────────────
    private void OnCraftClicked()
    {
        if (WeaponCraftManager.Instance == null) return;

        bool ok = WeaponCraftManager.Instance.CraftWeapon(
            ORE_IDS[_selMain], ORE_IDS[_selSub], WEAPON_TYPES[_selWeapon]);

        if (ok)
        {
            RefreshAll();
            OrderHUD.Instance?.RefreshAll();
            Sprite weaponSprite = SafeSprite(_weaponSprites, _selMain * 5 + _selWeapon);
            Hide();
            CraftAnimationController.Instance?.PlayCraftAnimation(weaponSprite);
        }
    }

    // ─────────────────────────────────────────
    // 전체 갱신
    // ─────────────────────────────────────────
    private void RefreshAll()
    {
        RefreshHighlights();
        RefreshPreview();
    }

    private void RefreshHighlights()
    {
        for (int i = 0; i < 5; i++)
            ApplyHighlight(_weaponBtns[i], _weaponOuts[i], i == _selWeapon, false);

        for (int i = 0; i < 5; i++)
            ApplyHighlight(_mainBtns[i], _mainOuts[i], i == _selMain, false);

        for (int i = 0; i < 5; i++)
            ApplyHighlight(_subBtns[i], _subOuts[i], i == _selSub, i == _selMain && i != _selSub);
    }

    private static void ApplyHighlight(Button btn, Outline outline, bool sel, bool dim)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img != null)
            img.color = sel ? CLR_BTN_SEL : (dim ? CLR_BTN_DIM : CLR_BTN_NORMAL);
        if (outline != null)
            outline.effectColor = sel ? CLR_OUTLINE_ON : CLR_OUTLINE_OFF;
    }

    private void RefreshPreview()
    {
        var recipe  = WeaponCraftManager.Recipes[WEAPON_TYPES[_selWeapon]];
        string mainId = ORE_IDS[_selMain];
        string subId  = ORE_IDS[_selSub];

        // 이름
        if (_previewName != null)
            _previewName.text = $"{ORE_NAMES[_selMain]} {WEAPON_NAMES[_selWeapon]}";

        // 수정: 미리보기 이미지 — 인덱스 = 광석(행) * 5 + 무기(열)
        if (_previewImg != null)
        {
            Sprite spr = SafeSprite(_weaponSprites, _selMain * 5 + _selWeapon);
            _previewImg.sprite = spr;
            _previewImg.color  = spr != null ? Color.white : ORE_COLORS[_selMain];
        }

        // 재료
        if (_previewMat != null)
            _previewMat.text = $"\n\n{ORE_NAMES[_selMain]} ×{recipe.mainCount}\n{ORE_NAMES[_selSub]} ×{recipe.subCount}";

        // 수정: 가격 공식 = (basePrice + oreValue * mainCount) * (1 + moreSell)
        string weaponItemId = BuildWeaponId(_selWeapon, _selMain);
        int price;
        if (ShopManager.Instance != null)
        {
            price = (int)ShopManager.Instance.GetWeaponPrice(weaponItemId);
        }
        else
        {
            float moreSell = PlayerStatManager.Instance != null
                ? PlayerStatManager.Instance.UpMoreSell : 0f;
            float raw = recipe.basePrice + ORE_VALUES[_selMain] * (float)recipe.mainCount;
            price = Mathf.RoundToInt(raw * (1f + moreSell));
        }

        if (_previewPrice != null)
            _previewPrice.text = $"판매가: {price:N0} G";

        // 보유
        int held = InventoryManager.Instance != null
            ? InventoryManager.Instance.GetQuantity(weaponItemId) : 0;
        if (_previewHold != null)
            _previewHold.text = $"보유: {held}개";

        // 제작 버튼 활성화 여부
        bool canCraft = WeaponCraftManager.Instance != null &&
                        WeaponCraftManager.Instance.CanCraft(mainId, subId, WEAPON_TYPES[_selWeapon]);
        if (_craftBtnImg != null)
            _craftBtnImg.color = canCraft ? CLR_CRAFT_OK : CLR_CRAFT_FAIL;
        if (_craftBtnTxt != null)
            _craftBtnTxt.text = canCraft ? "제작" : "재료 부족";
        if (_craftBtn != null)
            _craftBtn.interactable = canCraft;
    }

    private void SetStatus(string msg, Color color)
    {
        if (_statusTxt == null) return;
        _statusTxt.text  = msg;
        _statusTxt.color = color;
        CancelInvoke(nameof(ClearStatus));
        Invoke(nameof(ClearStatus), 3f);
    }
    private void ClearStatus() { if (_statusTxt != null) _statusTxt.text = ""; }

    // ─────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────
    private static string BuildWeaponId(int weaponIdx, int mainOreIdx)
    {
        string typeId = WeaponCraftManager.GetWeaponTypeId(WEAPON_TYPES[weaponIdx]);
        string oreId  = ORE_IDS[mainOreIdx].Replace("fruitstone_", "");
        return $"weapon_{typeId}_{oreId}";
    }

    private static readonly string[] _weaponTypeIds = { "sword", "axe", "spear", "hammer", "gauntlet" };
    private static readonly string[] _oreIds        = { "apple", "melon", "orange", "lemon", "grape" };

    public Sprite GetWeaponSprite(string weaponItemId)
    {
        string[] p = weaponItemId.Split('_');
        if (p.Length < 3) return null;
        int wi = System.Array.IndexOf(_weaponTypeIds, p[1]);
        int oi = System.Array.IndexOf(_oreIds, p[2]);
        if (wi < 0 || oi < 0) return null;
        return SafeSprite(_weaponSprites, oi * 5 + wi);
    }

    private static Sprite SafeSprite(Sprite[] arr, int idx)
        => (arr != null && idx >= 0 && idx < arr.Length) ? arr[idx] : null;

    // ─────────────────────────────────────────
    // UI 헬퍼
    // ─────────────────────────────────────────
    private Transform BuildCanvas()
    {
        var go = new GameObject("Canvas");
        var c  = go.AddComponent<Canvas>();
        c.renderMode   = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 200;
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

    private void MakeSecLabel(Transform parent, string text, Vector2 pos, float width = 200f)
    {
        var lbl = MakeTxt(parent, text, 15, FontStyles.Bold, CLR_LABEL,
            pos, new Vector2(width, 22f));
        lbl.alignment = TextAlignmentOptions.Left;
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
        tmp.text = label; tmp.fontSize = 16; tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.Center;
        return btn;
    }
}
