using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 세이브/로드 팝업 UI 컨트롤러.
///
/// [Inspector 연결 필요]
///   saveButton   → 세이브 버튼
///   loadButton   → 로드 버튼
///   popupPanel   → 결과 팝업 패널 (기본 비활성화, CanvasGroup 컴포넌트 추가 필수!)
///   popupMessage → 팝업 안의 TextMeshPro
///   popupIcon    → 성공/실패 아이콘 Image (선택)
///
/// [팝업 패널 구조 예시]
///   Canvas
///   └─ SaveUI (이 스크립트 부착)
///      ├─ SaveButton  ──→ saveButton
///      ├─ LoadButton  ──→ loadButton
///      └─ PopupPanel  ──→ popupPanel (CanvasGroup 추가!)
///         ├─ IconImage   ──→ popupIcon
///         └─ MessageText ──→ popupMessage
/// </summary>
public class SaveUIController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("팝업")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text popupMessage;
    [SerializeField] private Image popupIcon;       // 선택 사항
    [SerializeField] private Image background;

    [Header("아이콘 (선택)")]
    [SerializeField] private Sprite successSprite;       // 성공 아이콘
    [SerializeField] private Sprite failSprite;          // 실패 아이콘

    [Header("팝업 표시 시간 (초)")]
    [SerializeField] private float popupDuration = 2.0f;

    private Coroutine _popupCoroutine;

    // ─────────────────────────────────────────
    // 초기화
    // ─────────────────────────────────────────
    private void Start()
    {


        saveButton.onClick.AddListener(OnSaveClicked);
        loadButton.onClick.AddListener(OnLoadClicked);

        SaveManager.Instance.OnSaveResult += ShowPopup;
        SaveManager.Instance.OnLoadResult += ShowPopup;

        popupPanel.SetActive(false);

        // 세이브 파일 없으면 로드 버튼 비활성화
        loadButton.interactable = SaveManager.Instance.HasSaveData();
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.OnSaveResult -= ShowPopup;
        SaveManager.Instance.OnLoadResult -= ShowPopup;
    }

    // ─────────────────────────────────────────
    // 버튼 핸들러
    // ─────────────────────────────────────────
    private void OnSaveClicked()
    {
        saveButton.interactable = false;
        SaveManager.Instance.Save();
        saveButton.interactable = true;
        loadButton.interactable = true;  // 저장 후 로드 버튼 활성화
    }

    private void OnLoadClicked()
    {
        loadButton.interactable = false;
        SaveManager.Instance.Load();
        loadButton.interactable = true;
    }

    // ─────────────────────────────────────────
    // 팝업 표시
    // ─────────────────────────────────────────
    private void ShowPopup(bool success, string message)
    {
        if (_popupCoroutine != null)
            StopCoroutine(_popupCoroutine);

        popupMessage.text  = message;
        popupMessage.color = success ? Color.white : new Color(1f, 0.4f, 0.4f);

        if (popupIcon != null)
            popupIcon.sprite = success ? successSprite : failSprite;

        _popupCoroutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        popupPanel.SetActive(true);
        yield return FadePanel(0f, 1f, 0.15f);
        yield return new WaitForSeconds(popupDuration);
        yield return FadePanel(1f, 0f, 0.3f);
        popupPanel.SetActive(false);
    }

    private IEnumerator FadePanel(float from, float to, float duration)
    {
        var group = popupPanel.GetComponent<CanvasGroup>();
        if (group == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed    += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}
