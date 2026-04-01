using UnityEngine;
using UnityEngine.InputSystem;

public class Creafting : MonoBehaviour
{

    public GameObject craftingUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어와 충돌했을 때
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) // E 키가 눌렸을 때
            {
                craftingUI.SetActive(true); // 크래프팅 UI 활성화
                Debug.Log("E 키가 눌렸습니다. 크래프팅 UI가 활성화되었습니다.");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어가 충돌에서 벗어났을 때
        {
            craftingUI.SetActive(false); // 크래프팅 UI 비활성화
            Debug.Log("플레이어가 충돌에서 벗어났습니다. 크래프팅 UI가 비활성화되었습니다.");
        }
    }

}
