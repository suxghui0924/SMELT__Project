using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 세이브/로드 버튼 UI 컨트롤러. 팝업 없이 동작합니다.
///
/// [Inspector 연결 필요]
///   saveButton → 세이브 버튼
///   loadButton → 로드 버튼
/// </summary>
public class SaveUIController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button saveButton;
    //[SerializeField] private Button loadButton;

    private void Start()
    {
        saveButton.onClick.AddListener(OnSaveClicked);
       // loadButton.onClick.AddListener(OnLoadClicked);

     //   loadButton.interactable = SaveManager.Instance.HasSaveData();
    }

    private void OnDestroy()
    {
        saveButton.onClick.RemoveListener(OnSaveClicked);
   //     loadButton.onClick.RemoveListener(OnLoadClicked);
    }

    private void OnSaveClicked()
    {
        saveButton.interactable = false;
        SaveManager.Instance.Save();
        saveButton.interactable = true;
      // loadButton.interactable = true;
    }

    private void OnLoadClicked()
    {
      //  loadButton.interactable = false;
        SaveManager.Instance.Load();
   //     loadButton.interactable = true;
    }
}
