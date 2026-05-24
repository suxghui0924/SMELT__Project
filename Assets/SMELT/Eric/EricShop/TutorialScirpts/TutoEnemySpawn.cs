using UnityEngine;

public class TutoEnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject[] spawnPoint;
    public void Spawn(bool right)
    {
        if (right)
        {
            GameObject go = Instantiate(enemyPrefab, spawnPoint[0].transform.position, Quaternion.identity);
            go.transform.localScale = new Vector3(go.transform.localScale.x*2, go.transform.localScale.y*2, go.transform.localScale.z*2);
            
        }   
        else 
        {
            GameObject go = Instantiate(enemyPrefab, spawnPoint[1].transform.position, Quaternion.identity);
            go.transform.localScale = new Vector3(-go.transform.localScale.x*2, go.transform.localScale.y*2, go.transform.localScale.z*2);
        }
    }
}
