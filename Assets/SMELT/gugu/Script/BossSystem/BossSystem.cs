using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class BossSystem : MonoBehaviour
{
    public Image _Hp;
    public static BossSystem Instance;

    private bool isAlive = true;
    private float Damage = 10f; //근데 이건 곡괭이에따라서 데미지 달라져서 수정헤야함

    private bool isStart = true;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _Hp.fillAmount = 0;        
    }
    //시작할때 보스 체력바가 차오르는 연출
    private void Update()
    {
        if(isStart) OnStarting();
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
            // 보스 max hp에서 damage만큼 감소시키는 로직 추가
            _Hp.fillAmount -= Damage / 100f;
            Debug.Log("플레이어와 충돌함");
            if (_Hp.fillAmount <= 0)
            {
                Debug.Log("보스가 죽었습니다!");
                isAlive = false;
                PlayDeathAnimation();
            }
        }
    }

    private void OnStarting()
    {
        if (_Hp.fillAmount == 1||!isStart) isStart = false;
        _Hp.fillAmount += 1f * Time.deltaTime;
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