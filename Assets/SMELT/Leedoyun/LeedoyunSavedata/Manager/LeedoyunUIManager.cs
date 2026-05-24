using System;

/// <summary>
/// Leedoyun UI 상호 배타 관리자.
/// UI가 열릴 때 NotifyOpen을 호출하면 이전에 열린 UI를 자동으로 닫습니다.
/// OnAnyUIOpened 이벤트를 통해 PrototypeZone의 라디오/달력 UI도 함께 닫힙니다.
/// MonoBehaviour가 아니므로 씬에 별도 배치 불필요.
/// </summary>
public static class LeedoyunUIManager
{
    private static Action _currentCloseAction;

    /// <summary>Leedoyun UI가 열릴 때 발생. PrototypeZone이 구독해 라디오/달력을 닫습니다.</summary>
    public static event Action OnAnyUIOpened;

    /// <summary>UI가 열릴 때 호출. 기존 열린 UI를 닫고 closeAction을 등록합니다.</summary>
    public static void NotifyOpen(Action closeAction)
    {
        try { _currentCloseAction?.Invoke(); } catch { }
        _currentCloseAction = closeAction;
        OnAnyUIOpened?.Invoke();
    }

    /// <summary>현재 열린 UI를 닫고 등록을 초기화합니다.</summary>
    public static void CloseAll()
    {
        try { _currentCloseAction?.Invoke(); } catch { }
        _currentCloseAction = null;
    }

    /// <summary>씬 전환 시 상태 초기화 용도로 호출 가능.</summary>
    public static void Reset()
    {
        _currentCloseAction = null;
        OnAnyUIOpened = null;
    }
}
