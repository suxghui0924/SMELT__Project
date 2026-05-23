using System;
using System.Collections;
using UnityEngine;

public class GrapeBase : MonoBehaviour
{
    private Transform _playerTransform;
    private Vector3 _enemyDirection;
    private float _timer = 0f;
    private float _spawntimer = 0f;
    private bool _canMove = true;
    private bool _grapeSpawnStart = false;
    
    private int _volumeDirect;
    
    [SerializeField] private GameObject grapePrefab;
    [SerializeField] private int grapeCount;
    [SerializeField] private float grapeSpeed;
    [SerializeField] private float spawnDuration;
    [SerializeField] private float timerWhenStop;
    
    
    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (_canMove)
        {
            _timer += Time.deltaTime;
            if (_timer < timerWhenStop)
            {
                _enemyDirection = (transform.parent.position - _playerTransform.position).normalized;
                transform.parent.position -= _enemyDirection * (grapeSpeed * Time.deltaTime);
            }
            else
            {
                _canMove = false;
                _grapeSpawnStart = true;
            }
        }
        else if (_grapeSpawnStart&&grapeCount>0)
        {
            _spawntimer += Time.deltaTime;
            if (_spawntimer >= spawnDuration)
            {
               GameObject grapeEnemy = Instantiate(grapePrefab, transform.parent);
               EnemyBase grapeEnemyBase = GetComponentInChildren<EnemyBase>();
               grapeEnemyBase.GetVolumeDir(_volumeDirect);
                _spawntimer = 0;
                grapeCount--;
            }
            else if(grapeCount<=0) _grapeSpawnStart = false;
        }
        else if (!_grapeSpawnStart && !_canMove)
        {
            Destroy(gameObject);      
        }
    }
    
    public void GetVolumeDir(int dir)
    {
        _volumeDirect = dir;
    }
}
