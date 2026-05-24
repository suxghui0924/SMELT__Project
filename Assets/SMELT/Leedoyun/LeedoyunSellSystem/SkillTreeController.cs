using UnityEngine;

/// <summary>
/// 스킬 트리 열기/닫기 컨트롤러.
/// 담당자: 이도윤
/// </summary>
public class SkillTreeController : MonoBehaviour
{
    public static SkillTreeController Instance { get; private set; }

    [Tooltip("RefreshAll용 — Upgrading 컴포넌트 탐색 기준")]
    [SerializeField] private GameObject _skillTreePanel;

    private bool _isOpen;
    public bool IsOpen => _isOpen;

    public static event System.Action<bool> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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
        _isOpen = true;
        LeedoyunUIManager.NotifyOpen(Hide);
        if (UICanvasManager.instance != null)
            UICanvasManager.instance.ControlObject(ObjectType.ShopASkill, true);
        RefreshAll();
        OnStateChanged?.Invoke(true);
    }

    public void Hide()
    {
        _isOpen = false;
        if (UICanvasManager.instance != null)
            UICanvasManager.instance.ControlObject(ObjectType.ShopASkill, false);
        OnStateChanged?.Invoke(false);
    }

    public void Toggle()
    {
        if (_isOpen) Hide(); else Show();
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
}
