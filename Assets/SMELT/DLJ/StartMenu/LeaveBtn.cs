using UnityEngine;

public class LeaveBtn : MonoBehaviour
{
    public void Leave()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
