using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveUIController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("토스트 설정")]
    [SerializeField] private Font toastFont;
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup _toastGroup;
    private Text _toastText;
    private Coroutine _toastCoroutine;
    private static readonly WaitForSeconds WaitLoadCooldown = new WaitForSeconds(1.5f);
    private GameObject _toastGO;

    private void Awake()
    {
        CreateToastUI();
    }

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveClicked);

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnLoadClicked);
            loadButton.interactable = SaveManager.Instance.HasSaveData();
        }
    }

    private void OnDestroy()
    {
        saveButton.onClick.RemoveListener(OnSaveClicked);
        if (loadButton != null)
            loadButton.onClick.RemoveListener(OnLoadClicked);
        if (_toastGO != null)
            Destroy(_toastGO);
    }

    private void OnSaveClicked()
    {
        saveButton.interactable = false;
        SaveManager.Instance.Save();
        saveButton.interactable = true;
        if (loadButton != null)
            loadButton.interactable = true;
        ShowToast("세이브 되었습니다");
    }

    private void OnLoadClicked()
    {
        if (loadButton != null) loadButton.interactable = false;
        SaveManager.Instance.Load();
        ShowToast("로드 되었습니다");
        StartCoroutine(ReenableLoadButton());
    }

    private IEnumerator ReenableLoadButton()
    {
        yield return WaitLoadCooldown;
        if (loadButton != null)
            loadButton.interactable = SaveManager.Instance != null && SaveManager.Instance.HasSaveData();
    }

    private void ShowToast(string message)
    {
        _toastText.text = message;

        if (_toastCoroutine != null)
            StopCoroutine(_toastCoroutine);

        _toastCoroutine = StartCoroutine(ToastRoutine());
    }

    private IEnumerator ToastRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _toastGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        _toastGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _toastGroup.alpha = Mathf.Clamp01(1f - elapsed / fadeDuration);
            yield return null;
        }
        _toastGroup.alpha = 0f;
    }

    private void CreateToastUI()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        // 토스트 패널
        _toastGO = new("Toast");
        GameObject toastGO = _toastGO;
        toastGO.transform.SetParent(canvas.transform, false);

        RectTransform toastRect = toastGO.AddComponent<RectTransform>();
        toastRect.anchorMin = new Vector2(0.5f, 0f);
        toastRect.anchorMax = new Vector2(0.5f, 0f);
        toastRect.pivot = new Vector2(0.5f, 0f);
        toastRect.anchoredPosition = new Vector2(0f, 80f);
        toastRect.sizeDelta = new Vector2(320f, 56f);

        Image bg = toastGO.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        _toastGroup = toastGO.AddComponent<CanvasGroup>();
        _toastGroup.alpha = 0f;
        _toastGroup.blocksRaycasts = false;
        _toastGroup.interactable = false;

        // 텍스트
        GameObject textGO = new("ToastText");
        textGO.transform.SetParent(toastGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 4f);
        textRect.offsetMax = new Vector2(-12f, -4f);

        _toastText = textGO.AddComponent<Text>();
        _toastText.alignment = TextAnchor.MiddleCenter;
        _toastText.color = Color.white;
        _toastText.fontSize = 20;
        _toastText.font = toastFont != null
            ? toastFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 가장 위에 렌더링되도록 마지막 자식으로 이동
        toastGO.transform.SetAsLastSibling();
    }
}
