using UnityEngine;
using UnityEngine.InputSystem;

public class Creafting : MonoBehaviour
{
    public GameObject CraftingUI;
    public GameObject button;
    public GameObject Creaft;

    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CraftingUI.SetActive(true);
            button.SetActive(true);
            Creaft.SetActive(true);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}