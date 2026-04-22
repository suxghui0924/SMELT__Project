using UnityEngine;
using UnityEngine.SceneManagement;

public class scenetrigger : MonoBehaviour
{
    public string nextSceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
