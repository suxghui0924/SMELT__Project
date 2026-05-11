using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
public class EnemySpawn : MonoBehaviour
{

    [SerializeField] private GameObject enemyPrefab;
    private float _halfWidth;
    private float _leftX;
    private float _rightX;
    private int _enemySpawnPoint;
    private float _offset = -2.13f;

    private float _enemySpawnTimer;
    public float maxEnemySpawnTimer;
    public float minEnemySpawnTimer;
    private float _timer=0f;
    private void Start()
    {
        _halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        _leftX = Camera.main.transform.position.x - _halfWidth;
        _rightX = Camera.main.transform.position.x + _halfWidth;

        _enemySpawnTimer = Random.Range(minEnemySpawnTimer, maxEnemySpawnTimer);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
           
        if (_timer>=_enemySpawnTimer)
        {
            _enemySpawnPoint = Random.Range(0, 2);
            if (_enemySpawnPoint == 0)
            {
                GameObject enemy= Instantiate(enemyPrefab,new Vector3(_leftX,_offset,0),Quaternion.Euler(0,180,0));
            }
            else if (_enemySpawnPoint == 1)
            {
                GameObject enemy= Instantiate(enemyPrefab,new Vector3(_rightX,_offset,0),Quaternion.identity);
            }
            _enemySpawnTimer = Random.Range(minEnemySpawnTimer, maxEnemySpawnTimer);
            _timer = 0;
        }
    }
}