using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EricPickaxeShop : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("EricPickaxeShop");//상점 씬 이름 넣으면 됨
    }
}