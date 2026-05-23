using UnityEngine;

public class TutoEnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject[] spawnPoint;
    public void Spawn(bool right)
    {
        if (right)
        {
            Instantiate(enemyPrefab, spawnPoint[0].transform.position, Quaternion.identity);
        }   
        else 
        {
            Instantiate(enemyPrefab, spawnPoint[1].transform.position, Quaternion.identity);
        }
    }
}
