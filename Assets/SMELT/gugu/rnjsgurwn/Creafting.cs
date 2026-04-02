using UnityEngine;
using UnityEngine.InputSystem;

public class Creafting : MonoBehaviour
{
    public GameObject CraftingUI;

    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CraftingUI.SetActive(true);
            Debug.Log("E 키 눌림 → UI 활성화"); // 근데 이건 UI가 있어야 하는데 없어서 일단 임시로 만듬
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