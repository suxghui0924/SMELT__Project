using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class BossSystem : MonoBehaviour
{
    public Image _Hp;
    public static BossSystem Instance;

    private bool isAlive = true;
    private float Damage = 10f; //근데 이건 곡괭이에따라서 데미지 달라져서 수정헤야함

    private void Awake()
    {
        Instance = this;
    }
    public void Init(float damage)
    {
        this.Damage = damage;
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive == false) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌함");
            // 보스 max hp에서 damage만큼 감소시키는 로직 추가
            _Hp.fillAmount -= Damage / 100f;
            if (_Hp.fillAmount <= 0)
            {
                Debug.Log("보스가 죽었습니다!");
                isAlive = false;
                PlayDeathAnimation();
            }
        }
    }
    private void PlayDeathAnimation()
    {
        Debug.Log("애니메이션 실행");
    }
    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}