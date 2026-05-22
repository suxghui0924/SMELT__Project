using UnityEngine;
using System.Collections;


public class DrowOre : MonoBehaviour
{
    
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private GameObject ore;
    private Animator _bossSKillAnim;
    [SerializeField] skillSystem _skillSystem;
     private int spawnCount = 3;// 생성 개수 (변경 가능)
     private float Delays = 1f;

    

    private void Awake()
    {
        _bossSKillAnim = GetComponentInParent<Animator>();
    }

    

    
    public IEnumerator SpawnOre()
    {
        
        
        for (int i = 0; i < spawnCount; i++)
        {
            if (_skillSystem.IsAlive == true)
            {
                _bossSKillAnim.SetTrigger("BossSkill");
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f),Random.Range(-0.5f, 0.5f));
                Instantiate(ore, spawnPoints.position + randomOffset, Quaternion.identity);
                yield return new WaitForSeconds(Delays);
            }
            

        }
        BossSkillManager.Instance.timer = 1.5f;
        BossSkillManager.Instance.stack = true;

    }
}