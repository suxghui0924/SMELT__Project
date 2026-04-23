using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 프로토타입 전체 HUD.
///
/// [패널 구성]
///   하단 중앙: 구역 안내 텍스트
///   화면 중앙: 제작 패널 (E키 - 대장간), 판매 패널 (E키 - 상점) [하나만 표시]
///   우하단  : 상태 메시지 텍스트
/// (재화/광석 표시는 EconomyUI가 담당)
/// </summary>
public class PrototypeHUD : MonoBehaviour
{
    // ─────────────────────────────────────────
    // 싱글톤
    // ─────────────────────────────────────────
    public static PrototypeHUD Instance { get; private set; }

    // 한국어 폰트 (빌드 시 캐시)
    private static TMP_FontAsset _korFont;

    private const string KOR_FONT_PATH = "Assets/SMELT/Suxghui/GmarketSansTTFMedium SDF.asset";

    // ─────────────────────────────────────────
    // 레시피 정의
    // ─────────────────────────────────────────
    private class Recipe
    {
        public string WeaponId;
        public string WeaponName;
        public Color  WeaponColor;
        public string Symbol;
        public (string id, int amount)[] Ingredients;
        public int SellPrice;
    }

    private static readonly Recipe[] RECIPES =
    {
        new Recipe {
            WeaponId    = "weapon_fruitsword",
            WeaponName  = "과일 단검",
            WeaponColor = new Color(0.90f, 0.30f, 0.30f),
            Symbol      = "단검",
            Ingredients = new[] { ("fruitstone_apple",  2) },
            SellPrice   = 400
        },
        new Recipe {
            WeaponId    = "weapon_melonaxe",
            WeaponName  = "멜론 도끼",
            WeaponColor = new Color(0.25f, 0.78f, 0.35f),
            Symbol      = "도끼",
            Ingredients = new[] { ("fruitstone_melon",  2) },
            SellPrice   = 500
        },
        new Recipe {
            WeaponId    = "weapon_orangelance",
            WeaponName  = "오렌지 창",
            WeaponColor = new Color(1.00f, 0.58f, 0.10f),
            Symbol      = "창",
            Ingredients = new[] { ("fruitstone_orange", 2) },
            SellPrice   = 600
        },
        new Recipe {
            WeaponId    = "weapon_lemonbow",
            WeaponName  = "레몬 활",
            WeaponColor = new Color(0.95f, 0.92f, 0.20f),
            Symbol      = "활",
            Ingredients = new[] { ("fruitstone_lemon",  2) },
            SellPrice   = 700
        },
        new Recipe {
            WeaponId    = "weapon_grapespear",
            WeaponName  = "포도 창",
            WeaponColor = new Color(0.60f, 0.20f, 0.90f),
            Symbol      = "창",
            Ingredients = new[] { ("fruitstone_grape",  2) },
            SellPrice   = 800
        },
        new Recipe {
            WeaponId    = "weapon_mixsword",
            WeaponName  = "혼합 검",
            WeaponColor = new Color(0.20f, 0.62f, 1.00f),
            Symbol      = "대검",
            Ingredients = new[] { ("fruitstone_apple", 1), ("fruitstone_melon", 1) },
            SellPrice   = 1200
        },
    };

    // ─────────────────────────────────────────
    // 광석 이름 / ID 목록
    // ─────────────────────────────────────────
    private static readonly string[] ORE_IDS =
    {
        "fruitstone_apple", "fruitstone_melon", "fruitstone_orange",
        "fruitstone_lemon",  "fruitstone_grape"
    };

    private static readonly Dictionary<string, string> ORE_NAMES = new Dictionary<string, string>
    {
        { "fruitstone_apple",  "사과석"   },
        { "fruitstone_melon",  "멜론석"   },
        { "fruitstone_orange", "귤석" },
        { "fruitstone_lemon",  "레몬석"   },
        { "fruitstone_grape",  "포도석"   },
    };

    // ─────────────────────────────────────────
    // UI 레퍼런스 캐시
    // ─────────────────────────────────────────
    // 구역 힌트
    private GameObject _zoneHintGO;
    private TMP_Text   _zoneHintText;

    // 제작 패널
    private GameObject _craftPanel;
    private Button[]   _craftButtons = new Button[RECIPES.Length];

    // 판매 패널
    private GameObject  _sellPanel;
    private TMP_Text[]  _sellQtyTexts  = new TMP_Text[RECIPES.Length];
    private Button[]    _sellButtons   = new Button[RECIPES.Length];
    private TMP_Text[]  _sellBtnTexts  = new TMP_Text[RECIPES.Length];

    // 상태 텍스트
    private TMP_Text _statusText;

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
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ─────────────────────────────────────────
    // 외부 API (Zone / Player → HUD)
    // ─────────────────────────────────────────
    public void OnZoneEnter(ZoneType type)
    {
        if (_zoneHintGO == null) return;
        _zoneHintText.text = type switch
        {
            ZoneType.Crafting => "대장간  —  [ E ] 무기 제작",
            _                 => ""
        };
        _zoneHintGO.SetActive(true);
    }

    public void OnZoneExit(ZoneType type)
    {
        if (type == ZoneType.Crafting && _craftPanel != null) _craftPanel.SetActive(false);
        if (type == ZoneType.Selling  && _sellPanel  != null) _sellPanel.SetActive(false);
        if (_zoneHintGO != null) _zoneHintGO.SetActive(false);
    }

    public void OnOreGathered(string oreId) { }

    public void ToggleCraftPanel()
    {
        if (_craftPanel == null) return;
        bool open = !_craftPanel.activeSelf;
        _craftPanel.SetActive(open);
        _sellPanel?.SetActive(false);
        if (open) RefreshCraftButtons();
    }

    public void ToggleSellPanel()
    {
        if (_sellPanel == null) return;
        bool open = !_sellPanel.activeSelf;
        _sellPanel.SetActive(open);
        _craftPanel?.SetActive(false);
        if (open) RefreshSellPanel();
    }

    // ─────────────────────────────────────────
    // UI 빌드
    // ─────────────────────────────────────────
    private void BuildUI()
    {
        // 한국어 폰트 로드 (에디터 전용 - 프로토타입이므로 AssetDatabase 사용)
#if UNITY_EDITOR
        _korFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(KOR_FONT_PATH);
        if (_korFont == null)
            Debug.LogWarning($"[PrototypeHUD] 한국어 폰트를 찾을 수 없습니다: {KOR_FONT_PATH}");
#endif

        var canvasGO = new GameObject("ProtoCanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        var root = canvasGO.transform;
        BuildZoneHint(root);
        BuildCraftPanel(root);
        BuildSellPanel(root);
        BuildStatusText(root);
    }

    // ── 구역 힌트 (하단 중앙) ─────────────────
    private void BuildZoneHint(Transform root)
    {
        // 배경 패널
        _zoneHintGO = MakeRT(root, "ZoneHint", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(620f, 38f));
        _zoneHintGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        // 텍스트는 자식 오브젝트에 (Image와 TMP는 같은 GO에 붙일 수 없음)
        _zoneHintText = MakeText(_zoneHintGO.transform, "", 14, FontStyles.Normal,
            Color.white, Vector2.zero, new Vector2(620f, 38f));
        _zoneHintText.alignment = TextAlignmentOptions.Center;
        _zoneHintGO.SetActive(false);
    }

    // ── 제작 패널 (화면 중앙) ─────────────────
    private void BuildCraftPanel(Transform root)
    {
        var bg = MakePanel(root, "CraftPanel", new Color(0.06f, 0.06f, 0.15f, 0.97f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(840f, 490f));
        _craftPanel = bg.gameObject;
        _craftPanel.SetActive(false);

        var p = _craftPanel.transform;

        // 타이틀
        var title = MakeText(p, "무기 제작", 22, FontStyles.Bold, Color.white, new Vector2(0f, 210f), new Vector2(800f, 36f));
        title.alignment = TextAlignmentOptions.Center;

        // 레시피 카드 (3열 × 2행)
        for (int i = 0; i < RECIPES.Length; i++)
        {
            int col = i % 3;
            int row = i / 3;
            float x = -270f + col * 270f;
            float y = 100f  - row * 185f;
            BuildRecipeCard(p, i, new Vector2(x, y));
        }

        // 닫기 버튼
        MakeBtn(p, "닫기", new Vector2(0f, -220f), new Vector2(120f, 36f),
            new Color(0.40f, 0.14f, 0.14f), () => _craftPanel.SetActive(false));
    }

    private void BuildRecipeCard(Transform parent, int idx, Vector2 pos)
    {
        var recipe = RECIPES[idx];
        var card   = MakePanel(parent, $"Card_{idx}", new Color(0.11f, 0.11f, 0.22f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(240f, 160f));
        var p = card.transform;

        // 무기 이미지 (컬러 박스)
        var imgBox = MakePanel(p, "WeaponImg", recipe.WeaponColor,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-70f, 20f), new Vector2(80f, 80f));
        var sym = MakeText(imgBox.transform, recipe.Symbol, 16, FontStyles.Bold,
            Color.white, Vector2.zero, new Vector2(80f, 80f));
        sym.alignment = TextAlignmentOptions.Center;

        // 무기 이름
        var wname = MakeText(p, recipe.WeaponName, 14, FontStyles.Bold,
            Color.white, new Vector2(48f, 40f), new Vector2(130f, 22f));

        // 재료 목록
        var sb = new System.Text.StringBuilder();
        foreach (var (id, amount) in recipe.Ingredients)
            sb.AppendLine($"{ORE_NAMES[id]}  x{amount}");
        var ing = MakeText(p, sb.ToString().TrimEnd(), 11, FontStyles.Normal,
            new Color(0.7f, 0.95f, 0.7f), new Vector2(48f, 5f), new Vector2(130f, 34f));

        // 제작 버튼
        _craftButtons[idx] = MakeBtn(p, $"제작  {recipe.SellPrice}G",
            new Vector2(0f, -58f), new Vector2(210f, 32f),
            new Color(0.18f, 0.34f, 0.74f), () => TryCraft(idx));
    }

    // ── 판매 패널 (화면 중앙) ─────────────────
    private void BuildSellPanel(Transform root)
    {
        var bg = MakePanel(root, "SellPanel", new Color(0.06f, 0.12f, 0.06f, 0.97f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(530f, 500f));
        _sellPanel = bg.gameObject;
        _sellPanel.SetActive(false);

        var p = _sellPanel.transform;

        var title = MakeText(p, "무기 판매", 22, FontStyles.Bold, Color.white, new Vector2(0f, 220f), new Vector2(490f, 36f));
        title.alignment = TextAlignmentOptions.Center;

        // 판매 행 (고정 6개)
        for (int i = 0; i < RECIPES.Length; i++)
        {
            float y = 158f - i * 62f;
            BuildSellRow(p, i, y);
        }

        MakeBtn(p, "닫기", new Vector2(0f, -222f), new Vector2(120f, 36f),
            new Color(0.40f, 0.14f, 0.14f), () => _sellPanel.SetActive(false));
    }

    private void BuildSellRow(Transform parent, int idx, float y)
    {
        var recipe = RECIPES[idx];
        var row    = MakePanel(parent, $"SellRow_{idx}", new Color(0.10f, 0.16f, 0.10f, 0.90f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(495f, 55f));
        var p = row.transform;

        // 무기 컬러 박스
        var box = MakePanel(p, "Box", recipe.WeaponColor,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-215f, 0f), new Vector2(42f, 42f));
        var sym = MakeText(box.transform, recipe.Symbol, 11, FontStyles.Bold, Color.white, Vector2.zero, new Vector2(42f, 42f));
        sym.alignment = TextAlignmentOptions.Center;

        // 무기 이름
        MakeText(p, recipe.WeaponName, 14, FontStyles.Bold, Color.white, new Vector2(-85f, 8f), new Vector2(170f, 22f));

        // 보유 수량 (동적)
        _sellQtyTexts[idx] = MakeText(p, "보유: 0", 12, FontStyles.Normal,
            new Color(0.6f, 0.85f, 0.6f), new Vector2(-85f, -12f), new Vector2(170f, 18f));

        // 판매 가격
        var price = MakeText(p, $"{recipe.SellPrice} G", 15, FontStyles.Bold,
            new Color(1f, 0.85f, 0.2f), new Vector2(60f, 0f), new Vector2(110f, 36f));
        price.alignment = TextAlignmentOptions.Center;

        // 판매 버튼
        _sellBtnTexts[idx] = null;
        var btnGO = MakeRT(p, $"SellBtn_{idx}", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(190f, 0f), new Vector2(92f, 38f));
        btnGO.AddComponent<Image>().color = new Color(0.15f, 0.58f, 0.38f);
        _sellButtons[idx] = btnGO.AddComponent<Button>();
        int capturedIdx = idx;
        _sellButtons[idx].onClick.AddListener(() => TrySell(capturedIdx));
        var btnTxt = new GameObject("Text");
        btnTxt.transform.SetParent(btnGO.transform, false);
        var bRT = btnTxt.AddComponent<RectTransform>();
        bRT.anchorMin = Vector2.zero; bRT.anchorMax = Vector2.one;
        bRT.offsetMin = bRT.offsetMax = Vector2.zero;
        _sellBtnTexts[idx] = btnTxt.AddComponent<TextMeshProUGUI>();
        if (_korFont != null) _sellBtnTexts[idx].font = _korFont;
        _sellBtnTexts[idx].text      = "판매";
        _sellBtnTexts[idx].fontSize  = 14f;
        _sellBtnTexts[idx].fontStyle = FontStyles.Bold;
        _sellBtnTexts[idx].color     = Color.white;
        _sellBtnTexts[idx].alignment = TextAlignmentOptions.Center;
    }

    // ── 상태 메시지 텍스트 (우하단) ──────────
    private void BuildStatusText(Transform root)
    {
        var sGO = MakeRT(root, "StatusText", new Vector2(1f, 0f), new Vector2(1f, 0f),
            new Vector2(1f, 0f), new Vector2(-10f, 18f), new Vector2(280f, 34f));
        _statusText           = sGO.AddComponent<TextMeshProUGUI>();
        if (_korFont != null) _statusText.font = _korFont;
        _statusText.fontSize  = 12f;
        _statusText.color     = new Color(0.58f, 0.58f, 0.58f);
        _statusText.alignment = TextAlignmentOptions.Right;
    }

    // ─────────────────────────────────────────
    // 제작 / 판매 로직
    // ─────────────────────────────────────────
    private void TryCraft(int idx)
    {
        var recipe = RECIPES[idx];
        var inv    = InventoryManager.Instance;
        if (inv == null) return;

        foreach (var (id, amount) in recipe.Ingredients)
        {
            if (inv.HasItem(id, amount)) continue;
            SetStatus($"재료 부족: {ORE_NAMES[id]} x{amount} 필요", new Color(1f, 0.4f, 0.4f));
            return;
        }

        foreach (var (id, amount) in recipe.Ingredients)
            inv.RemoveItem(id, amount);

        inv.AddItem(recipe.WeaponId, 1);
        SetStatus($"{recipe.WeaponName} 제작 완료!", new Color(0.5f, 0.9f, 1f));
        RefreshCraftButtons();
    }

    private void TrySell(int idx)
    {
        var recipe = RECIPES[idx];
        var inv    = InventoryManager.Instance;
        if (inv == null || !inv.HasItem(recipe.WeaponId)) return;

        inv.RemoveItem(recipe.WeaponId, 1);
        inv.AddGold(recipe.SellPrice);

        SetStatus($"{recipe.WeaponName} 판매 완료!  +{recipe.SellPrice} G", new Color(1f, 0.85f, 0.2f));
        RefreshSellPanel();
    }

    // ─────────────────────────────────────────
    // 패널 갱신
    // ─────────────────────────────────────────
    private void RefreshCraftButtons()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        for (int i = 0; i < RECIPES.Length; i++)
        {
            if (_craftButtons[i] == null) continue;

            bool canCraft = true;
            foreach (var (id, amount) in RECIPES[i].Ingredients)
                if (!inv.HasItem(id, amount)) { canCraft = false; break; }

            var img = _craftButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = canCraft ? new Color(0.18f, 0.34f, 0.74f) : new Color(0.25f, 0.25f, 0.30f);
        }
    }

    private void RefreshSellPanel()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        for (int i = 0; i < RECIPES.Length; i++)
        {
            int qty = inv.GetQuantity(RECIPES[i].WeaponId);

            if (_sellQtyTexts[i] != null)
                _sellQtyTexts[i].text = $"보유: {qty}개";

            if (_sellButtons[i] != null)
            {
                _sellButtons[i].interactable = qty > 0;
                var img = _sellButtons[i].GetComponent<Image>();
                if (img != null)
                    img.color = qty > 0 ? new Color(0.15f, 0.58f, 0.38f) : new Color(0.28f, 0.28f, 0.28f);
            }

            if (_sellBtnTexts[i] != null)
                _sellBtnTexts[i].text = qty > 0 ? "판매" : "없음";
        }
    }

    // ─────────────────────────────────────────
    // 상태 텍스트
    // ─────────────────────────────────────────
    private void SetStatus(string msg, Color color)
    {
        if (_statusText == null) return;
        _statusText.text  = msg;
        _statusText.color = color;
    }

    // ─────────────────────────────────────────
    // UI 헬퍼
    // ─────────────────────────────────────────

    /// <summary>패널 생성. pivot 미지정 시 anchor 방향에 맞게 자동 설정.</summary>
    private static Image MakePanel(Transform parent, string name, Color color,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size,
        Vector2 pivot = default)
    {
        // pivot 미지정(0,0)이면 anchor 방향으로 자동 결정
        Vector2 p = (pivot == default) ? anchorMin : pivot;
        // 완전 중앙 앵커(0.5,0.5)는 center pivot 유지
        if (anchorMin == new Vector2(0.5f, 0.5f)) p = new Vector2(0.5f, 0.5f);
        var go  = MakeRT(parent, name, anchorMin, anchorMax, p, anchoredPos, size);
        var img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    /// <summary>패널 내부 텍스트. anchor/pivot = (0.5,0.5), pos는 패널 중심 기준.</summary>
    private static TMP_Text MakeText(Transform parent, string text, float size,
        FontStyles style, Color color, Vector2 pos, Vector2 sizeDelta)
    {
        var go  = MakeRT(parent, "Txt", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), pos, sizeDelta);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (_korFont != null) tmp.font = _korFont;
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.fontStyle = style;
        tmp.color     = color;
        return tmp;
    }

    /// <summary>버튼 생성.</summary>
    private static Button MakeBtn(Transform parent, string label, Vector2 pos, Vector2 size,
        Color bgColor, System.Action onClick,
        Vector2 anchorMin = default, Vector2 anchorMax = default)
    {
        bool useCenter = (anchorMin == default && anchorMax == default);
        var amin = useCenter ? new Vector2(0.5f, 0.5f) : anchorMin;
        var amax = useCenter ? new Vector2(0.5f, 0.5f) : anchorMax;

        // pivot follows anchor (same logic as MakePanel)
        Vector2 pvt = amin == new Vector2(0.5f, 0.5f) ? new Vector2(0.5f, 0.5f) : amin;

        var go  = MakeRT(parent, "Btn", amin, amax, pvt, pos, size);
        go.AddComponent<Image>().color = bgColor;
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick());

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(go.transform, false);
        var rt = txtGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        if (_korFont != null) tmp.font = _korFont;
        tmp.text      = label;
        tmp.fontSize  = 13f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return btn;
    }

    /// <summary>RectTransform GameObject 생성.</summary>
    private static GameObject MakeRT(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = size;
        return go;
    }
}
