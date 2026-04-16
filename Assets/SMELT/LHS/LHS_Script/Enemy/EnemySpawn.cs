using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
public class EnemySpawn : MonoBehaviour
{

    [SerializeField] private GameObject enemyPrefab;
    private float halfWidth;
    private float leftX;
    private float rightX;
    private int _enemySpawnPoint;
    private void Start()
    {
        halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        leftX = Camera.main.transform.position.x - halfWidth;
        rightX = Camera.main.transform.position.x + halfWidth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _enemySpawnPoint = Random.Range(0, 2);
            if (_enemySpawnPoint == 0)
            { 
                GameObject Enemy= Instantiate(enemyPrefab,new Vector3(leftX,0,0),Quaternion.identity);
            }
            else if (_enemySpawnPoint == 1)
            {
                GameObject Enemy= Instantiate(enemyPrefab,new Vector3(rightX,0,0),Quaternion.identity);
            }
        }
    }
}