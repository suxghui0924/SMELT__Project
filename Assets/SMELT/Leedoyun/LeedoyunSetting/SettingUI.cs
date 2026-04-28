using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// ESC 키로 설정 패널을 열고 닫는 UI 컨트롤러.
///
/// [인스펙터 연결 항목]
///   settingPanel  : 설정 창 루트 GameObject
///   volumeSlider  : 소리 조절 Slider
///   volumeLabel   : 볼륨 수치 표시 TextMeshProUGUI (0~100)
///   saveButton    : 저장하기 Button
///   loadButton    : 불러오기 Button
///   quitButton    : 게임 종료 Button
///   closeButton   : 설정 창 닫기 Button
///
/// ※ LeedoyunSetting 담당자 외 건드리지 마세요.
/// </summary>
public class SettingUI : MonoBehaviour, ISaveable
{
    [Header("UI 패널")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Image      dimBackground;     // 반투명 어두운 오버레이
    [SerializeField] private float      dimAlpha     = 0.6f;
    [SerializeField] private float      fadeDuration = 0.2f;

    private Coroutine _dimCoroutine;

    [Header("버튼 / 슬라이더")]
    [SerializeField] private Slider            volumeSlider;
    [SerializeField] private TextMeshProUGUI   volumeLabel;
    [SerializeField] private Button            saveButton;
    [SerializeField] private Button            loadButton;
    [SerializeField] private Button            quitButton;
    [SerializeField] private Button            closeButton;

    // -----------------------------------------
    // 초기화
    // -----------------------------------------
    private void Start()
    {
        // SettingUI가 속한 루트 Canvas를 찾아 카메라 연결
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas == null && settingPanel != null)
            rootCanvas = settingPanel.GetComponentInParent<Canvas>();

        if (rootCanvas != null)
        {
            Camera cam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
            rootCanvas.renderMode   = RenderMode.ScreenSpaceCamera;
            rootCanvas.worldCamera  = cam;
            rootCanvas.sortingOrder = 20;
        }

        // 시작 시 설정 창 및 dim 배경 숨김
        if (settingPanel != null)
            settingPanel.SetActive(false);

        if (dimBackground != null)
        {
            dimBackground.gameObject.SetActive(false);
            dimBackground.color = new Color(0f, 0f, 0f, 0f);
        }

        // 세이브 시스템 등록
        if (SaveManager.Instance != null)
            SaveManager.Instance.Register(this);

        // 슬라이더 초기값 = 현재 마스터 볼륨
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value    = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeLabel(AudioListener.volume);
        }

        // 버튼 이벤트 연결
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);

        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    // -----------------------------------------
    // 매 프레임: ESC 감지
    // -----------------------------------------
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePanel();
    }

    // -----------------------------------------
    // 패널 토글
    // -----------------------------------------
    public void TogglePanel()
    {
        if (settingPanel == null) return;
        bool opening = !settingPanel.activeSelf;
        settingPanel.SetActive(opening);
        FadeDim(opening);
    }

    public void ClosePanel()
    {
        if (settingPanel != null)
            settingPanel.SetActive(false);
        FadeDim(false);
    }

    private void FadeDim(bool fadeIn)
    {
        if (dimBackground == null) return;

        if (_dimCoroutine != null)
            StopCoroutine(_dimCoroutine);

        _dimCoroutine = StartCoroutine(FadeDimRoutine(fadeIn));
    }

    private IEnumerator FadeDimRoutine(bool fadeIn)
    {
        float target  = fadeIn ? dimAlpha : 0f;
        float start   = dimBackground.color.a;

        if (fadeIn)
            dimBackground.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            dimBackground.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        dimBackground.color = new Color(0f, 0f, 0f, target);

        if (!fadeIn)
            dimBackground.gameObject.SetActive(false);
    }

    // -----------------------------------------
    // 소리 조절
    // -----------------------------------------
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeLabel(value);
    }

    private void UpdateVolumeLabel(float value)
    {
        if (volumeLabel != null)
            volumeLabel.text = Mathf.RoundToInt(value * 100f).ToString();
    }

    // -----------------------------------------
    // ISaveable
    // -----------------------------------------
    public void OnSave(SaveData data)
    {
        if (volumeSlider != null)
            data.volumeLevel = volumeSlider.value;
    }

    public void OnLoad(SaveData data)
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = data.volumeLevel;
            AudioListener.volume = data.volumeLevel;
            UpdateVolumeLabel(data.volumeLevel);
        }
    }

    // -----------------------------------------
    // 저장하기
    // -----------------------------------------
    private void OnSaveClicked()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Save();
        else
            Debug.LogWarning("[SettingUI] SaveManager 인스턴스를 찾을 수 없습니다.");
    }

    // -----------------------------------------
    // 불러오기
    // -----------------------------------------
    private void OnLoadClicked()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.Load();
        else
            Debug.LogWarning("[SettingUI] SaveManager 인스턴스를 찾을 수 없습니다.");
    }

    // -----------------------------------------
    // 게임 종료
    // -----------------------------------------
    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
