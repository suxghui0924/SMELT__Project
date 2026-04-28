using UnityEngine;
using UnityEngine.UI;

public class BossSystem : MonoBehaviour
{
    public Image _Hp;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _Hp.fillAmount -= 0.1f;
            Debug.Log("플레이어와 충돌함");
           

        }
    }
}
