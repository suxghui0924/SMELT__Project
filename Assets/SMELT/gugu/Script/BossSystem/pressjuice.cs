using UnityEngine;
using System.Collections;
public class pressjuice : MonoBehaviour
{
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private GameObject juice;

    [SerializeField] private float spawnDelay = 7f; // 생성 쿨타임 (변경 가능+ 나중에 스킬 추가 되면 지우고 랜덤으로 바꿀예정)
    [SerializeField] private int spawnCount = 1;// 생성 개수 (변경 가능)

    private float timer = 0f;


   public void SpawnJuice()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Instantiate(juice, spawnPoints.position + randomOffset, Quaternion.identity);
            BossSkillManager.Instance.timer = 3f;
        }
    }
}
