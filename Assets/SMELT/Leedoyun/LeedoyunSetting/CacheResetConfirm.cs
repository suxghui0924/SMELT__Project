using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 캐시 초기화 확인 팝업 전용 컨트롤러.
/// 확인 팝업 GameObject에 붙여서 사용합니다.
///
/// [버튼 OnClick 연결]
///   캐시 삭제 버튼  → CacheResetConfirm.ShowPanel
///   아니요 버튼     → CacheResetConfirm.HidePanel
///   초기화(예) 버튼 → CacheResetConfirm.ResetAndReturnToLobby
/// </summary>
public class CacheResetConfirm : MonoBehaviour
{
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ResetAndReturnToLobby()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.ResetAllData();

        gameObject.SetActive(false);

        if (UICanvasManager.instance != null)
            UICanvasManager.instance.SetCanvasActive(CanvasType.Title, false);

        GameManager.instance.ChangeState(new LobbyState());
    }
}
