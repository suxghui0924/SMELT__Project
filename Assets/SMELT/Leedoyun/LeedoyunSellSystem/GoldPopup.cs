using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 무기 납품 / 주문 만료 시 화면에 골드 획득·손실 텍스트를 띄움.
/// 담당자: 이도윤
/// </summary>
public class GoldPopup : MonoBehaviour
{
    public static GoldPopup Instance { get; private set; }

    private Canvas        _canvas;
    private RectTransform _canvasRt;
    private TMP_FontAsset _font;
    private const string  FONT_PATH = "Assets/SMELT/Suxghui/Galmuri9 SDF.asset";

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildCanvas();
    }

    private void BuildCanvas()
    {
        var cGO = new GameObject("GoldPopupCanvas");
        cGO.transform.SetParent(transform);
        _canvas             = cGO.AddComponent<Canvas>();
        _canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 200;
        cGO.AddComponent<CanvasScaler>();
        _canvasRt = cGO.GetComponent<RectTransform>();

#if UNITY_EDITOR
        _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_PATH);
#endif
    }

    // ─────────────────────────────────────────
    // 퍼블릭 API
    // ─────────────────────────────────────────

    /// <summary>
    /// 월드 좌표 위에 골드 팝업을 표시합니다.
    /// amount 양수 → 노란색 "+X G", 음수 → 빨간색 "-X G"
    /// </summary>
    public static void Show(Vector3 worldPos, int amount)
    {
        if (Instance == null)
        {
            var go = new GameObject("[GoldPopup]");
            go.AddComponent<GoldPopup>();
        }
        if (Instance == null) return;
        Instance.StartCoroutine(Instance.PopupRoutine(worldPos, amount));
    }

    // ─────────────────────────────────────────
    // 팝업 코루틴
    // ─────────────────────────────────────────
    private IEnumerator PopupRoutine(Vector3 worldPos, int amount)
    {
        var go = new GameObject("GoldText");
        go.transform.SetParent(_canvas.transform, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200f, 40f);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        if (_font != null) tmp.font = _font;
        tmp.text          = amount >= 0 ? $"+{amount:N0} G" : $"{amount:N0} G";
        tmp.fontSize      = 22f;
        tmp.fontStyle     = FontStyles.Bold;
        tmp.alignment     = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        Color baseColor = amount >= 0
            ? new Color(1f, 0.85f, 0.20f)   // 노란색 — 획득
            : new Color(1f, 0.25f, 0.25f);   // 빨간색 — 패널티

        tmp.color = baseColor;

        const float duration  = 1.8f;
        const float riseUnits = 1.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 위로 떠오름
            Vector3 currentWorld = worldPos + Vector3.up * (t * riseUnits);

            if (Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(currentWorld);
                if (screenPos.z >= 0f)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _canvasRt,
                        new Vector2(screenPos.x, screenPos.y),
                        null,
                        out Vector2 local);
                    rt.anchoredPosition = local;
                }
            }

            // 후반 40%에서 페이드 아웃
            float alpha = t < 0.6f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
            Color c = baseColor;
            c.a = alpha;
            tmp.color = c;

            yield return null;
        }

        Destroy(go);
    }
}
