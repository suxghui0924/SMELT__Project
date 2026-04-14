using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtn : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
