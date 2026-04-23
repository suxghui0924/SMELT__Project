using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private float timer = 0;
    private float minTime = 1f;
    private float maxTime = 3f;

    [SerializeField] private GameObject enemyPrefab;



    private void Start()
    {
        timer = Random.Range(minTime, maxTime);

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnEnemy();
            timer = Random.Range(minTime, maxTime);
        }
    }
    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

}
