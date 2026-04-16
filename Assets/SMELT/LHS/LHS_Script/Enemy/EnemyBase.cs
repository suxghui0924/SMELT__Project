using System;
using System.Collections;
using UnityEngine;
public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyDataSo;
    private Transform _playerTransform;
    public int CurrentHp{get; private set;}
    private float _enemySpeed;
    private int _enemyDamage;
    private string _enemyName;
    private Vector3 _enemyDirection;
    private Collider2D _enemyCollider2D;
    Player_HpScript _playerHpScript;
    private void Start()
    {
       _playerHpScript= GameObject.Find("PlayerHP").GetComponent<Player_HpScript>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        CurrentHp = enemyDataSo.enemyMaxHp;
        _enemySpeed = enemyDataSo.enemySpeed;
        _enemyDamage = enemyDataSo.enemyDamage;
        _enemyName = enemyDataSo.enemyName;
        _enemyCollider2D=GetComponent<Collider2D>();
            _enemyDirection=(transform.position-_playerTransform.position).normalized;
    }

    private void Update()
    {
       transform.position-=_enemyDirection*_enemySpeed*Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            _playerHpScript.TakeDamage(_enemyDamage);
        }
    }

    public void OnEnemyDamaged(int damage)
    {
        CurrentHp -= damage;
        if(CurrentHp<=0)Destroy(gameObject);
    }
}
