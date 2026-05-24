using _01_Scripts._Core._States;
using UnityEngine;

/// <summary>
/// 가게 포기 확인 패널.
/// 확인 버튼 → ConfirmAbandon(), 취소 버튼 → CancelAbandon()
/// 담당자: 이도윤
/// </summary>
public class StoreBrokenUI : MonoBehaviour
{
    public void ConfirmAbandon()
    {
        // 가게 닫기 (NPC 퇴장 + OnShopClosed 이벤트)
        ShopManager.Instance?.CloseShop();
        // 세이브 데이터 초기화 (일차·골드·인벤토리 리셋 + 등록된 모든 매니저 OnLoad 호출)
        SaveManager.Instance?.ResetAllData();
        // 타이머 초기화 (DDOL이라 직접 리셋 필요)
        DayTimer.Instance?.ResetTimer();
        // UI 닫기
        if (UICanvasManager.instance != null)
            UICanvasManager.instance.ControlObject(ObjectType.StoreBroken, false);
        // 로비로 이동
        GameManager.instance?.ChangeState(new LobbyState());
    }

    public void CancelAbandon()
    {
        UICanvasManager.instance?.ControlObject(ObjectType.StoreBroken, false);
    }
}
