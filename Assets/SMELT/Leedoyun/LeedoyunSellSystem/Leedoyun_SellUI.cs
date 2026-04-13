using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 판매 시스템 프로토타입 UI.
/// 코드로 UI를 자동 생성하므로 프리팹 없이 바로 실행 가능.
///
/// [씬 세팅 - 4개 오브젝트만 추가]
///   1. SaveManager     오브젝트 → SaveManager.cs
///   2. InventoryManager 오브젝트 → InventoryManager.cs
///   3. SellManager     오브젝트 → Leedoyun_SellManager.cs
///   4. SellUI          오브젝트 → Leedoyun_SellUI.cs  ← 이 파일
///
/// 담당자: 이도윤
/// </summary>
public class Leedoyun_SellUI : MonoBehaviour
{
    // ─────────────────────────────────────────
    // 폰트 설정 (Inspector에서 한국어 SDF 폰트 지정)
    // ─────────────────────────────────────────
    [Header("폰트")]
    [Tooltip("한국어 TMP 폰트 에셋. 비워두면 기본 폰트 사용.")]
    [SerializeField] private TMP_FontAsset _koreanFont;

    // ─────────────────────────────────────────
    // UI 참조 (코드로 생성됨)
    // ─────────────────────────────────────────
    private TextMeshProUGUI _todayGoldText;
    private TextMeshProUGUI _heldWeaponText;

    // 주문 슬롯 3개
    private const int SLOT_COUNT = 3;
    private OrderSlotUI[] _slots = new OrderSlotUI[SLOT_COUNT];

    // 테스트용 무기 선택 (← → 버튼으로 사이클)
    private int _selectedWeaponIndex = 0;
    private TextMeshProUGUI _selectedWeaponText;
    private static readonly string[] TEST_WEAPONS =
    {
        "weapon_sword_apple",
        "weapon_sword_melon",
        "weapon_axe_apple",
        "weapon_axe_melon",
        "weapon_spear_orange",
        "weapon_bat_lemon",
        "weapon_gauntlet_grape",
    };

    // ─────────────────────────────────────────
    // 슬롯 데이터 (내부 클래스)
    // ─────────────────────────────────────────
    private class OrderSlotUI
    {
        public GameObject  root;
        public TextMeshProUGUI weaponText;
        public TextMeshProUGUI goldText;
        public Image       timerBar;
        public Button      deliverButton;
        public GameObject  emptyLabel;
        public Leedoyun_CustomerOrder order;
    }

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Start()
    {
        BuildUI();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        var sm = Leedoyun_SellManager.Instance;
        if (sm == null) { Debug.LogError("[SellUI] Leedoyun_SellManager 없음"); return; }

        sm.OnOrderAdded      += HandleOrderAdded;
        sm.OnOrderFulfilled  += HandleOrderFulfilled;
        sm.OnOrderExpired    += HandleOrderExpired;
        sm.OnTodayGoldChanged += HandleGoldChanged;
    }

    private void OnDestroy()
    {
        var sm = Leedoyun_SellManager.Instance;
        if (sm == null) return;

        sm.OnOrderAdded      -= HandleOrderAdded;
        sm.OnOrderFulfilled  -= HandleOrderFulfilled;
        sm.OnOrderExpired    -= HandleOrderExpired;
        sm.OnTodayGoldChanged -= HandleGoldChanged;
    }

    // ─────────────────────────────────────────
    // 매 프레임: 타이머 바 갱신
    // ─────────────────────────────────────────
    private void Update()
    {
        foreach (var slot in _slots)
        {
            if (slot.order == null || !slot.order.IsActive) continue;
            if (slot.timerBar != null)
                slot.timerBar.fillAmount = slot.order.RemainingRatio;
        }
    }

    // ─────────────────────────────────────────
    // 이벤트 핸들러
    // ─────────────────────────────────────────
    private void HandleOrderAdded(Leedoyun_CustomerOrder order)
    {
        // 빈 슬롯 찾아서 할당
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order == null)
            {
                AssignSlot(i, order);
                return;
            }
        }
        Debug.LogWarning("[SellUI] 빈 슬롯 없음 - 슬롯 수 부족");
    }

    private void HandleOrderFulfilled(Leedoyun_CustomerOrder order, int gold)
    {
        ClearSlot(order);
    }

    private void HandleOrderExpired(Leedoyun_CustomerOrder order)
    {
        ClearSlot(order);
    }

    private void HandleGoldChanged(int prev, int next)
    {
        if (_todayGoldText != null)
            _todayGoldText.text = $"오늘 수익: {next:N0} G";
    }

    // ─────────────────────────────────────────
    // 슬롯 조작
    // ─────────────────────────────────────────
    private void AssignSlot(int index, Leedoyun_CustomerOrder order)
    {
        var slot = _slots[index];
        slot.order = order;

        slot.emptyLabel.SetActive(false);
        slot.weaponText.gameObject.SetActive(true);
        slot.goldText.gameObject.SetActive(true);
        slot.timerBar.gameObject.SetActive(true);
        slot.deliverButton.gameObject.SetActive(true);

        slot.weaponText.text   = WeaponDisplayName(order.requestedWeaponId);
        slot.goldText.text     = $"{order.rewardGold:N0} G";
        slot.timerBar.fillAmount = 1f;
    }

    private void ClearSlot(Leedoyun_CustomerOrder order)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (_slots[i].order == order)
            {
                _slots[i].order = null;
                RefreshEmptySlot(i);
                return;
            }
        }
    }

    private void RefreshEmptySlot(int index)
    {
        var slot = _slots[index];
        slot.emptyLabel.SetActive(true);
        slot.weaponText.gameObject.SetActive(false);
        slot.goldText.gameObject.SetActive(false);
        slot.timerBar.gameObject.SetActive(false);
        slot.deliverButton.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // 버튼 콜백
    // ─────────────────────────────────────────

    // [납품] 버튼 클릭 (슬롯 인덱스 캡처)
    private void OnDeliverClicked(int slotIndex)
    {
        var slot = _slots[slotIndex];
        if (slot.order == null) return;

        // 테스트: 들고 있는 무기가 없으면 자동으로 드롭다운 무기를 인벤토리에 추가 후 납품
        string weaponId = slot.order.requestedWeaponId;
        if (!InventoryManager.Instance.HasItem(weaponId))
            InventoryManager.Instance.AddItem(weaponId, 1);

        bool ok = Leedoyun_SellManager.Instance.TryFulfillByWeapon(weaponId);
        Debug.Log(ok ? $"[SellUI] 납품 성공: {weaponId}" : $"[SellUI] 납품 실패");
    }

    // [인벤에 추가] 버튼 - 선택된 무기를 인벤토리에 추가 (테스트용)
    private void OnAddWeaponClicked()
    {
        string weaponId = TEST_WEAPONS[_selectedWeaponIndex];
        InventoryManager.Instance.AddItem(weaponId, 1);
        Debug.Log($"[SellUI] 테스트 무기 추가: {weaponId}");
        UpdateHeldText();
    }

    // [◀] 버튼 - 이전 무기
    private void OnPrevWeapon()
    {
        _selectedWeaponIndex = (_selectedWeaponIndex - 1 + TEST_WEAPONS.Length) % TEST_WEAPONS.Length;
        RefreshSelectedWeaponText();
    }

    // [▶] 버튼 - 다음 무기
    private void OnNextWeapon()
    {
        _selectedWeaponIndex = (_selectedWeaponIndex + 1) % TEST_WEAPONS.Length;
        RefreshSelectedWeaponText();
    }

    private void RefreshSelectedWeaponText()
    {
        if (_selectedWeaponText != null)
            _selectedWeaponText.text = WeaponDisplayName(TEST_WEAPONS[_selectedWeaponIndex]);
    }

    // [주문 강제 생성] 버튼 (테스트용)
    private void OnForceOrderClicked()
    {
        // SendMessage로 private 메서드 호출
        Leedoyun_SellManager.Instance.SendMessage("TrySpawnOrder",
            SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateHeldText()
    {
        if (_heldWeaponText == null) return;
        // 실제 플레이에서는 WeaponHolder 연동, 프로토타입은 인벤 현황 표시
        _heldWeaponText.text = "인벤토리 확인: 로그 참고";
    }

    // ─────────────────────────────────────────
    // UI 자동 생성
    // ─────────────────────────────────────────
    private void BuildUI()
    {
        // 카메라 없으면 자동 생성
        if (Camera.main == null)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags       = CameraClearFlags.SolidColor;
            cam.backgroundColor  = new Color(0.1f, 0.1f, 0.15f);
            cam.orthographic     = true;
        }

        // EventSystem 없으면 자동 생성 (버튼 클릭에 필요)
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }

        // Canvas
        var canvasGo = new GameObject("SellUI_Canvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10; // 다른 UI 위에 표시
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // 배경 패널 (클릭 흡수 방지: raycastTarget = false)
        var bg = MakePanel(canvasGo.transform, "BG",
            new Vector2(0, 0), new Vector2(1, 1),
            new Color(0.1f, 0.1f, 0.15f, 0.92f));
        bg.GetComponent<Image>().raycastTarget = false;

        // ── 오늘 수익 텍스트 ──
        _todayGoldText = MakeText(bg.transform, "GoldText", "오늘 수익: 0 G",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -60), 56, Color.yellow);

        // ── 주문 슬롯 3개 ──
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            float xCenter = 0.18f + i * 0.32f;
            _slots[i] = BuildOrderSlot(bg.transform, i, xCenter);
        }

        // ── 하단 테스트 패널 ──
        BuildTestPanel(bg.transform);
    }

    private OrderSlotUI BuildOrderSlot(Transform parent, int index, float xAnchor)
    {
        // 슬롯 패널
        var panel = MakePanel(parent, $"Slot_{index}",
            new Vector2(xAnchor - 0.13f, 0.25f),
            new Vector2(xAnchor + 0.13f, 0.85f),
            new Color(0.2f, 0.22f, 0.28f, 1f));

        // 슬롯 배경 클릭 흡수 방지
        panel.GetComponent<Image>().raycastTarget = false;

        // 테두리 (외곽선 느낌)
        var outline = panel.AddComponent<Outline>();
        outline.effectColor    = new Color(0.5f, 0.6f, 1f, 0.8f);
        outline.effectDistance = new Vector2(2, -2);

        var slot = new OrderSlotUI { root = panel };

        // 빈 슬롯 안내
        slot.emptyLabel = MakeText(panel.transform, "Empty", "대기 중...",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, 36, new Color(0.5f, 0.5f, 0.5f)).gameObject;

        // 무기 이름
        slot.weaponText = MakeText(panel.transform, "WeaponName", "",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -60), 40, Color.white);

        // 골드
        slot.goldText = MakeText(panel.transform, "Gold", "",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -120), 36, Color.yellow);

        // 타이머 바
        var barBg = MakePanel(panel.transform, "TimerBg",
            new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.45f),
            new Color(0.15f, 0.15f, 0.15f));
        barBg.GetComponent<Image>().raycastTarget = false;
        var barFill = MakePanel(barBg.transform, "TimerFill",
            new Vector2(0, 0), new Vector2(1, 1),
            new Color(0.3f, 0.8f, 0.3f));
        // MakePanel이 이미 Image를 추가하므로 GetComponent로 가져옴
        slot.timerBar             = barFill.GetComponent<Image>();
        slot.timerBar.type        = Image.Type.Filled;
        slot.timerBar.fillMethod  = Image.FillMethod.Horizontal;
        slot.timerBar.fillAmount  = 1f;
        slot.timerBar.color       = new Color(0.3f, 0.8f, 0.3f);

        // 납품 버튼
        int captured = index;
        slot.deliverButton = MakeButton(panel.transform, "DeliverBtn", "납품",
            new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.25f),
            new Color(0.2f, 0.6f, 0.3f),
            () => OnDeliverClicked(captured));

        // 초기 숨김
        slot.weaponText.gameObject.SetActive(false);
        slot.goldText.gameObject.SetActive(false);
        barBg.SetActive(false);
        slot.timerBar.gameObject.SetActive(false);
        slot.deliverButton.gameObject.SetActive(false);

        return slot;
    }

    private void BuildTestPanel(Transform parent)
    {
        var panel = MakePanel(parent, "TestPanel",
            new Vector2(0f, 0f), new Vector2(1f, 0.2f),
            new Color(0.08f, 0.08f, 0.12f, 1f));
        panel.GetComponent<Image>().raycastTarget = false;

        // 드롭다운 (무기 선택)
        // [◀] 이전 무기 버튼
        MakeButton(panel.transform, "PrevBtn", "<",
            new Vector2(0.01f, 0.3f), new Vector2(0.08f, 0.85f),
            new Color(0.3f, 0.3f, 0.35f),
            OnPrevWeapon);

        // 선택된 무기 이름 표시
        _selectedWeaponText = MakeText(panel.transform, "SelectedWeapon",
            WeaponDisplayName(TEST_WEAPONS[0]),
            new Vector2(0.09f, 0.3f), new Vector2(0.38f, 0.85f),
            Vector2.zero, 32, Color.white);

        // [▶] 다음 무기 버튼
        MakeButton(panel.transform, "NextBtn", ">",
            new Vector2(0.39f, 0.3f), new Vector2(0.46f, 0.85f),
            new Color(0.3f, 0.3f, 0.35f),
            OnNextWeapon);

        // [인벤에 추가] 버튼
        MakeButton(panel.transform, "AddBtn", "인벤에 추가",
            new Vector2(0.48f, 0.3f), new Vector2(0.72f, 0.85f),
            new Color(0.2f, 0.4f, 0.7f),
            OnAddWeaponClicked);

        // [주문 강제 생성] 버튼 (테스트)
        MakeButton(panel.transform, "SpawnBtn", "주문 강제 생성",
            new Vector2(0.74f, 0.3f), new Vector2(0.99f, 0.85f),
            new Color(0.6f, 0.3f, 0.1f),
            OnForceOrderClicked);

        _heldWeaponText = MakeText(panel.transform, "HeldText",
            "[납품] 버튼을 누르면 해당 무기가 자동 추가 후 납품됩니다.",
            new Vector2(0f, 0f), new Vector2(1f, 0.28f),
            Vector2.zero, 26, new Color(0.7f, 0.7f, 0.7f));
    }

    // ─────────────────────────────────────────
    // UI 생성 헬퍼
    // ─────────────────────────────────────────
    private static GameObject MakePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var img  = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    private TextMeshProUGUI MakeText(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos,
        float fontSize, Color color)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin        = anchorMin;
        rect.anchorMax        = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta        = new Vector2(200, 40);
        var tmp  = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.alignment = TextAlignmentOptions.Center;
        // 한국어 폰트가 Inspector에 지정된 경우 적용
        if (_koreanFont != null) tmp.font = _koreanFont;
        // 텍스트는 클릭 필요 없음 → 뒤에 있는 버튼 클릭을 막지 않도록
        tmp.raycastTarget = false;
        return tmp;
    }

    private Button MakeButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor,
        System.Action onClick)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => onClick());

        // 버튼 텍스트
        var textGo   = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var textRect = textGo.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text                  = label;
        tmp.fontSize              = 32;
        tmp.enableAutoSizing      = true;  // 버튼 크기에 맞게 자동 조절
        tmp.fontSizeMin           = 18;
        tmp.fontSizeMax           = 40;
        tmp.color                 = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        // 한국어 폰트 적용
        if (_koreanFont != null) tmp.font = _koreanFont;

        return btn;
    }

    // ─────────────────────────────────────────
    // 표시용 이름
    // ─────────────────────────────────────────
    private static string WeaponDisplayName(string weaponItemId)
    {
        string[] p = weaponItemId.Split('_');
        if (p.Length < 3) return weaponItemId;
        return $"{OreKor(p[2])} {WeaponKor(p[1])}";
    }

    private static string WeaponKor(string t)
    {
        switch (t)
        {
            case "sword":    return "검";
            case "axe":      return "도끼";
            case "spear":    return "창";
            case "bat":      return "방망이";
            case "gauntlet": return "건틀릿";
            default:         return t;
        }
    }

    private static string OreKor(string o)
    {
        switch (o)
        {
            case "apple":  return "사과";
            case "melon":  return "멜론";
            case "orange": return "귤";
            case "lemon":  return "레몬";
            case "grape":  return "포도";
            default:       return o;
        }
    }
}
