using UnityEngine;

/// <summary>
/// 스킬 트리 열기/닫기 컨트롤러.
/// PrototypeZone(SkillTree) 이 Toggle() 을 호출합니다.
///
/// [Inspector 설정]
///   Skill Tree Panel : UpgradeTrees Canvas 오브젝트
///   Background       : 스킬 트리 전용 배경 오브젝트
///
/// 담당자: 이도윤
/// </summary>
public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController Instance { get; private set; }

    [Tooltip("스킬 트리 패널 오브젝트 (UpgradeTrees)")]
    [SerializeField] private GameObject _skillTreePanel;

    [Tooltip("스킬 트리 전용 배경 오브젝트")]
    [SerializeField] private GameObject _background;

    private Canvas _panelCanvas;
    private Canvas _bgCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_skillTreePanel != null) _panelCanvas = GetRootCanvas(_skillTreePanel);
        if (_background     != null) _bgCanvas    = _background.GetComponent<Canvas>();
    }

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show()
    {
        if (_background     != null) _background.SetActive(true);
        if (_skillTreePanel != null) _skillTreePanel.SetActive(true);

        // 스킬트리 패널을 최상위로
        if (_panelCanvas != null) _panelCanvas.sortingOrder = 500;

        // 배경이 자체 Canvas를 가지면 패널 바로 아래로, 아니면 형제 순서 맨 뒤로
        if (_background != null)
        {
            if (_bgCanvas != null)
                _bgCanvas.sortingOrder = 499;
            else
                _background.transform.SetAsFirstSibling();
        }

        RefreshAll();
    }

    public void Hide()
    {
        if (_skillTreePanel != null) _skillTreePanel.SetActive(false);
        if (_background     != null) _background.SetActive(false);
    }

    public void Toggle()
    {
        bool isOpen = _skillTreePanel != null && _skillTreePanel.activeSelf;
        if (isOpen) Hide(); else Show();
    }

    // 디버그용 — T키로 직접 토글
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) Toggle();
    }

    private void RefreshAll()
    {
        if (_skillTreePanel == null) return;
        foreach (var up in _skillTreePanel.GetComponentsInChildren<Upgrading>(true))
            up.UpdateTreesUI();
    }

    private static Canvas GetRootCanvas(GameObject go)
    {
        var canvases = go.GetComponentsInParent<Canvas>(true);
        if (canvases == null || canvases.Length == 0)
            return go.GetComponent<Canvas>();
        return canvases[^1]; // 가장 상위 Canvas
    }
}
