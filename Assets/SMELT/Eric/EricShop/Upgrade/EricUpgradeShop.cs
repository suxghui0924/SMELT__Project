using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EricUpgradeShop : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("EricUpgradeShop");//상점 씬 이름 넣으면 됨
    }
}