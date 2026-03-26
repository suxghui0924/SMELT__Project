using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EricCloseShop : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene");//상점 씬 이름 넣으면 됨
    }
}