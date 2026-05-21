using UnityEngine;
using System.Collections;


public class DrowOre : MonoBehaviour
{
    
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private GameObject ore;
    private Animator _bossSKillAnim;

     private float spawnDelay = 5f; // 생성 쿨타임 (변경 가능+ 나중에 스킬 추가 되면 지우고 랜덤으로 바꿀예정)
     private int spawnCount = 3;// 생성 개수 (변경 가능)
     private float Delays = 1f;

    private float timer = 0f;

    private void Awake()
    {
        _bossSKillAnim = GetComponentInParent<Animator>();
    }
    
    private void Start()
    {
        timer = spawnDelay;
    }

    

    
    public IEnumerator SpawnOre()
    {
        
        
        for (int i = 0; i < spawnCount; i++)
        {
            _bossSKillAnim.SetTrigger("BossSkill");
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f),Random.Range(-0.5f, 0.5f));
            Instantiate(ore, spawnPoints.position + randomOffset, Quaternion.identity);
            BossSkillManager.Instance.timer = 5f;
            yield return new WaitForSeconds(Delays);

        }
    }
}