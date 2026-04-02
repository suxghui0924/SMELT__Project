using UnityEngine;

public class AnvilPrompt : MonoBehaviour
{
    public GameObject promptUI;

    private void Start()
    {
        promptUI.SetActive(false);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptUI.SetActive(false);
        }
    }
}