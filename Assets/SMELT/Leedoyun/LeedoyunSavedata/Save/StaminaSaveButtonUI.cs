using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 팝업 없이 버튼 클릭만으로 세이브하는 UI 컨트롤러.
///
/// [Inspector 연결 필요]
///   saveButton → 세이브 버튼
///
/// SaveUIController 대신 이 스크립트를 사용하세요.
/// </summary>
public class StaminaSaveButtonUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button saveButton;

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveClicked);
    }

    private void OnDestroy()
    {
        saveButton.onClick.RemoveListener(OnSaveClicked);
    }

    private void OnSaveClicked()
    {
        SaveManager.Instance.Save();
    }
}
