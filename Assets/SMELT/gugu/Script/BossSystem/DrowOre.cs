using UnityEngine;
using System.Collections;


public class DrowOre : MonoBehaviour
{
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private GameObject ore;

    [SerializeField] private float spawnDelay = 5f; // 생성 쿨타임 (변경 가능+ 나중에 스킬 추가 되면 지우고 랜덤으로 바꿀예정)
    [SerializeField] private int spawnCount = 3;// 생성 개수 (변경 가능)
    [SerializeField] private float Delays = 3f;

    private float timer = 0f;

    private void Start()
    {
        timer = spawnDelay;
    }

    
    public IEnumerator SpawnOre()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f),Random.Range(-0.5f, 0.5f));
            Instantiate(ore, spawnPoints.position + randomOffset, Quaternion.identity);
            BossSkillManager.Instance.timer = 5f;
            yield return new WaitForSeconds(Delays);

        }
    }
}