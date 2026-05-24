using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
public class EnemySpawn : MonoBehaviour
{

    [SerializeField] private GameObject[] enemyPrefab;
    [Header("적 각각의 생성 확률")]
    [SerializeField] private float[] spawnChance;


    private bool _isGameObjectCurrentDay = false;
    private float _spawnTotalChance;
    private float _halfWidth;
    private float _leftX;
    private float _rightX;
    private int _enemySpawnPoint;
    private float _offset = -2.13f;

    private float _enemySpawnTimer;
    public float maxEnemySpawnTimer;
    public float minEnemySpawnTimer;
    private float _timer=0f;

    private void OnEnable()
    {
       string Currentday = Mathf.Clamp( InventoryManager.Instance.CurrentDay,1,7).ToString();
        if(gameObject.name==Currentday) _isGameObjectCurrentDay = true;
    }

    private void Start()
    {
        _halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        _leftX = Camera.main.transform.position.x - _halfWidth;
        _rightX = Camera.main.transform.position.x + _halfWidth;

        _enemySpawnTimer = Random.Range(minEnemySpawnTimer, maxEnemySpawnTimer);
        for (int i = 0; i < spawnChance.Length; i++)
        {
            _spawnTotalChance += spawnChance[i];
        }
    }

    private void Update()
    {
        if (_isGameObjectCurrentDay)
        {
            _timer += Time.deltaTime;

            if (_timer >= _enemySpawnTimer)
            {
                Spawn(_spawnTotalChance);
            }
        }
    }

    private void Spawn(float total)
    {
        float currentChance =  Random.Range(0.001f, total);
        float _totalChance = 0f;
        int enemyIndex = 0;
        for (int i = 0; i < spawnChance.Length; i++)
        {
            _totalChance += spawnChance[i];
            if (currentChance < _totalChance)
            {
                enemyIndex = i;
                break;
            }
        }
        _enemySpawnPoint = Random.Range(0, 2);

        if (_enemySpawnPoint == 0)
        {
           GameObject enemy= Instantiate(enemyPrefab[enemyIndex],new Vector3(_leftX,_offset,0),Quaternion.Euler(0,180,0));
           if (enemy.name != "Grape"&&enemy.name!="GrapeEnemy")
           {
               EnemyBase enemyBaseScript = enemy.GetComponentInChildren<EnemyBase>();
               enemyBaseScript.GetVolumeDir(0);
           }
           else
           {
               GrapeBase grapeBase = enemy.GetComponentInChildren<GrapeBase>();
               grapeBase.GetVolumeDir(0);
           }
        }   
        else if (_enemySpawnPoint == 1)
        {  
            
            GameObject enemy= Instantiate(enemyPrefab[enemyIndex],new Vector3(_rightX,_offset,0),Quaternion.identity);
            if (enemy.name !=null)
            {
                EnemyBase enemyBaseScript = enemy.GetComponentInChildren<EnemyBase>();
                enemyBaseScript.GetVolumeDir(1);
            }
            else
            {
                GrapeBase grapeBase = enemy.GetComponentInChildren<GrapeBase>();
                grapeBase.GetVolumeDir(1);
            }
        }
        _enemySpawnTimer = Random.Range(minEnemySpawnTimer, maxEnemySpawnTimer);
        _timer = 0;
    }
    
    
}