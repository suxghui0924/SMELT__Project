using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 무기 미리보기 패널.
/// 무기 종류 / 메인 과일석 / 서브 과일석을 선택하면 해당 무기 이미지를 표시합니다.
/// 화면 우측에 주문 HUD 아래 자동 배치됩니다.
/// </summary>
public class WeaponPreviewPanel : MonoBehaviour
{
    [Header("폰트")]
    [SerializeField] private TMP_FontAsset _koreanFont;
    private const string FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    // PrototypeBootstrap에서 Start() 전에 주입
    public Sprite[] WeaponSprites = new Sprite[25];

    // ─────────────────────────────────────────
    // 레이아웃 상수
    // ─────────────────────────────────────────
    private const float PANEL_W  = 276f;
    private const float BTN_W    = 46f;
    private const float BTN_H    = 26f;
    private const float ROW_GAP  = 8f;
    private const float IMG_SIZE = 90f;

    // ─────────────────────────────────────────
    // 색상 테이블
    // ─────────────────────────────────────────
    private static readonly Color[] WEAPON_COLORS = {
        new Color(0.85f, 0.25f, 0.25f),
        new Color(0.25f, 0.70f, 0.30f),
        new Color(1.00f, 0.55f, 0.10f),
        new Color(0.95f, 0.90f, 0.20f),
        new Color(0.55f, 0.18f, 0.85f),
    };
    private static readonly Color[] ORE_COLORS = {
        new Color(0.95f, 0.35f, 0.35f),
        new Color(0.30f, 0.80f, 0.35f),
        new Color(1.00f, 0.60f, 0.15f),
        new Color(0.95f, 0.92f, 0.22f),
        new Color(0.60f, 0.20f, 0.90f),
    };

    private static readonly string[] WEAPON_KOR = { "검", "도끼", "창", "망치", "건틀릿" };
    private static readonly string[] ORE_KOR    = { "사과석", "멜론석", "귤석", "레몬석", "포도석" };

    // ─────────────────────────────────────────
    // 선택 상태
    // ─────────────────────────────────────────
    private int _selWeapon = 0;
    private int _selMain   = 0;
    private int _selSub    = 0;

    private Button[]        _weaponBtns = new Button[5];
    private Button[]        _mainBtns   = new Button[5];
    private Button[]        _subBtns    = new Button[5];
    private Image           _previewImg;
    private TextMeshProUGUI _previewLbl;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Start()
    {
        BuildPanel();
        UpdatePreview();
    }

    // ─────────────────────────────────────────
    // UI 빌드
    // ─────────────────────────────────────────
    private void BuildPanel()
    {
        if (_koreanFont == null) _koreanFont = FontLoader.Galmuri9;
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Transform canvasT = canvas != null ? canvas.transform : BuildCanvas();

        const float PAD     = 10f;
        const float HEADER  = 28f;
        const float LABEL_H = 16f;
        float rowBlockH = LABEL_H + 2f + BTN_H + ROW_GAP;
        float panelH    = PAD + HEADER + 3f * rowBlockH + IMG_SIZE + 20f + PAD;

        // 주문 HUD 높이에 맞춰 아래 배치
        float orderHudH = 30f + 8f + 3f * 96f + 2f * 5f + 8f;
        float anchorY   = -(160f + orderHudH + 8f);

        var rootGO = MakePanel(canvasT, "WeaponPreviewPanel",
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            Vector2.zero, new Vector2(PANEL_W, panelH),
            new Color(0.05f, 0.05f, 0.08f, 0.93f));
        var rt = rootGO.GetComponent<RectTransform>();
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-10f, anchorY);

        var root = rootGO.transform;

        // 헤더
        var hdr = MakeTxt(root, "무기 미리보기", 13, FontStyles.Bold,
            new Color(0.6f, 0.85f, 1f),
            new Vector2(0f, panelH / 2f - PAD - HEADER / 2f),
            new Vector2(PANEL_W - 16f, HEADER));
        hdr.alignment = TextAlignmentOptions.Center;

        float curY = panelH / 2f - PAD - HEADER - 4f;

        BuildRow(root, ref curY, "무기 종류", WEAPON_KOR, WEAPON_COLORS, _weaponBtns,
            i => { _selWeapon = i; UpdatePreview(); HighlightAll(); });

        BuildRow(root, ref curY, "메인 과일석", ORE_KOR, ORE_COLORS, _mainBtns,
            i => { _selMain = i; UpdatePreview(); HighlightAll(); });

        BuildRow(root, ref curY, "서브 과일석", ORE_KOR, ORE_COLORS, _subBtns,
            i => { _selSub = i; UpdatePreview(); HighlightAll(); });

        // 이미지 영역
        curY -= 6f;
        var imgBgGO = MakePanel(root, "ImgBg",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, curY - IMG_SIZE / 2f - 4f),
            new Vector2(IMG_SIZE + 12f, IMG_SIZE + 12f),
            new Color(0.08f, 0.08f, 0.12f));

        var imgGO = MakePanel(imgBgGO.transform, "Img",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(IMG_SIZE, IMG_SIZE), Color.white);
        _previewImg = imgGO.GetComponent<Image>();
        _previewImg.preserveAspect = true;

        _previewLbl = MakeTxt(root, "", 11, FontStyles.Normal, new Color(0.65f, 0.65f, 0.65f),
            new Vector2(0f, curY - IMG_SIZE - 16f), new Vector2(PANEL_W - 16f, 18f));
        _previewLbl.alignment = TextAlignmentOptions.Center;

        HighlightAll();
    }

    private void BuildRow(Transform parent, ref float curY, string label,
        string[] names, Color[] colors, Button[] btns, System.Action<int> onSelect)
    {
        const float LABEL_H = 16f;
        MakeTxt(parent, label, 10, FontStyles.Normal, new Color(0.50f, 0.50f, 0.55f),
            new Vector2(0f, curY - LABEL_H / 2f), new Vector2(PANEL_W - 16f, LABEL_H))
            .alignment = TextAlignmentOptions.Center;
        curY -= LABEL_H + 2f;

        float totalW = 5f * BTN_W + 4f * 4f;
        float startX = -totalW / 2f + BTN_W / 2f;
        for (int i = 0; i < 5; i++)
        {
            int ci = i;
            float bx = startX + i * (BTN_W + 4f);
            btns[i] = MakeBtn(parent, names[i],
                new Vector2(bx, curY - BTN_H / 2f), new Vector2(BTN_W, BTN_H),
                new Color(colors[i].r * 0.3f, colors[i].g * 0.3f, colors[i].b * 0.3f),
                () => onSelect(ci));
        }
        curY -= BTN_H + ROW_GAP;
    }

    // ─────────────────────────────────────────
    // 미리보기 갱신
    // ─────────────────────────────────────────
    private void UpdatePreview()
    {
        // 스프라이트 배열 인덱스: 무기타입 * 5 + 메인광석
        int idx = _selWeapon * 5 + _selMain;
        Sprite spr = (WeaponSprites != null && idx < WeaponSprites.Length) ? WeaponSprites[idx] : null;

        if (spr != null)
        {
            _previewImg.sprite = spr;
            _previewImg.color  = Color.white;
        }
        else
        {
            _previewImg.sprite = null;
            Color w = WEAPON_COLORS[_selWeapon];
            Color o = ORE_COLORS[_selMain];
            _previewImg.color = Color.Lerp(w, o, 0.4f);
        }

        if (_previewLbl != null)
        {
            string main = ORE_KOR[_selMain];
            string sub  = ORE_KOR[_selSub];
            string wpn  = WEAPON_KOR[_selWeapon];
            _previewLbl.text = $"{main} + {sub} {wpn}";
        }
    }

    private void HighlightAll()
    {
        ApplyHighlight(_weaponBtns, _selWeapon, WEAPON_COLORS);
        ApplyHighlight(_mainBtns,   _selMain,   ORE_COLORS);
        ApplyHighlight(_subBtns,    _selSub,    ORE_COLORS);
    }

    private static void ApplyHighlight(Button[] btns, int sel, Color[] colors)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i] == null) continue;
            var img = btns[i].GetComponent<Image>();
            if (img == null) continue;
            img.color = i == sel
                ? colors[i]
                : new Color(colors[i].r * 0.3f, colors[i].g * 0.3f, colors[i].b * 0.3f);
        }
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
        var txtRt = txtGO.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        if (_koreanFont != null) tmp.font = _koreanFont;
        tmp.text = label; tmp.fontSize = 9f; tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        return btn;
    }
}
