using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtn : MonoBehaviour
{
    public void LoadScene()
    {
        GameManager.instance.ChangeState(new HouseState());
    }
}
