using UnityEngine;

public class CanvasDebugger : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.LogWarning($"[CanvasDebugger] '{gameObject.name}' 비활성화됨!\n{new System.Diagnostics.StackTrace()}");
    }

    private void OnDestroy()
    {
        Debug.LogWarning($"[CanvasDebugger] '{gameObject.name}' 파괴됨!\n{new System.Diagnostics.StackTrace()}");
    }
}
