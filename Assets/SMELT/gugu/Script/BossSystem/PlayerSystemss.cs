using UnityEngine;

public class PlayerSystemss : MonoBehaviour
{
    [SerializeField]private bool isAlive = true;
    private float Damage = 10f;
    [SerializeField]private GameObject boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("gg");
        if (isAlive == false) return;
        if (collision.gameObject.CompareTag("juice"))
        {

            BossSystem.Instance._Hp.fillAmount -= Damage / 200f;
            Debug.Log("플레이어와 충돌함");

        }
        else if (collision.gameObject.CompareTag("ore"))
        {
            BossSystem.Instance._Hp.fillAmount -= Damage / 500f;
            Debug.Log("플레이어와 충돌함");

        }

        if (BossSystem.Instance._Hp.fillAmount <= 0 )
        {
            Debug.Log("보스가 죽었습니다!");
            isAlive = false;
            PlayDeathAnimation();
        }
    }
   
    private void PlayDeathAnimation()
    {
        Debug.Log("애니메이션 실행");
        OnAnimationEnd();
    }
    public void OnAnimationEnd()
    {
        Destroy(boss);
    }
    
}
