using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 가게 폐업 확인 팝업 컨트롤러.
/// Group_StoreBroken 오브젝트에 붙어있음.
/// But_Y → ResetAndReturnToLobby, But_X → HidePanel, But_StoreBroken → ShowPanel
/// </summary>
public class CacheResetConfirm : MonoBehaviour
{
    public static CacheResetConfirm Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPanel()
    {
        // Radio UI 닫기
        if (UICanvasManager.instance != null)
            UICanvasManager.instance.ControlObject(ObjectType.Radio, false);
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ResetAndReturnToLobby()
    {
        ShopManager.Instance?.CloseShop();
        if (SaveManager.Instance != null)
            SaveManager.Instance.ResetAllData();
        DayTimer.Instance?.ResetTimer();

        gameObject.SetActive(false);

        if (UICanvasManager.instance != null)
            UICanvasManager.instance.SetCanvasActive(CanvasType.Title, false);

        GameManager.instance.ChangeState(new LobbyState());
    }
}
