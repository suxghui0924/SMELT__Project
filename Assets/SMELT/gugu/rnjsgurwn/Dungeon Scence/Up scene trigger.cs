/*using UnityEngine;
using UnityEngine.SceneManagement;

public class UpSceneTrigger : MonoBehaviour
{
    public string nextSceneName;
    public MoneyManager moneyManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (MoneyManager.Instance.UseMoney(100))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("돈 부족!");
        }
    }
}*/