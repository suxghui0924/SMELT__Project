/// <summary>
/// 저장 시스템에 연동할 매니저들이 구현해야 하는 인터페이스.
/// 사용법: public class YourManager : MonoBehaviour, ISaveable
/// </summary>
public interface ISaveable
{
    void OnSave(SaveData data);   // SaveManager.Save() 호출 시 자동으로 불림
    void OnLoad(SaveData data);   // SaveManager.Load() 호출 시 자동으로 불림
}
