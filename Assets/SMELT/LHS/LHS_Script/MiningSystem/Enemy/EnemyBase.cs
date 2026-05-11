using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyDataSo;

    [SerializeField] private float knockbackTimer;
    private Transform _playerTransform;
    public int CurrentHp{get; private set;}
    private float _enemySpeed;
    private int _enemyDamage;
    private string _enemyName;
    private Vector3 _enemyDirection;
    private Collider2D _enemyCollider2D;
    Player_HpScript _playerHpScript;
    
    private bool _canMove = true;
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
        if (_canMove)
            transform.position -= _enemyDirection * (_enemySpeed * Time.deltaTime);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            _playerHpScript.TakeDamage(_enemyDamage);
        }
    }

    public void OnEnemyDamaged(int damage)
    {
        CurrentHp -= damage;
        if(CurrentHp<=0)
        {
            int randomItemCount = Random.Range(1, 3);
            ItemSpawnManager.instance.AppleItemSpawn(transform,randomItemCount);
            StartCoroutine(ParticleRoutine());
        }
        else
        {
            StartCoroutine(KnockbackRoutine());
        }
    }
    private IEnumerator KnockbackRoutine()
    {
        _canMove = false;
        float timer = 0;
        Vector3 knockbackDir = (transform.position - _playerTransform.position).normalized;
        float knockbackForce = _enemySpeed * 4f; 

        while (timer <= knockbackTimer)
        {
            float progress = timer / knockbackTimer;
            float currentForce = Mathf.Lerp(knockbackForce, 0, progress);
            transform.position += knockbackDir * (currentForce * Time.deltaTime);
            
            timer += Time.deltaTime;
            yield return null;
        }
    
        _canMove = true;
    }

    private IEnumerator ParticleRoutine()
    {
        _canMove = false;
        Collider2D collider2D= GetComponent<Collider2D>();
        SpriteRenderer spriteRenderer = collider2D.GetComponent<SpriteRenderer>();
        collider2D.enabled = false;
        spriteRenderer.enabled = false;
        
        ParticleSystem particleSystem= GetComponentInChildren<ParticleSystem>();
             particleSystem.Play();
             yield return new WaitForSeconds(particleSystem.main.startLifetime.constant);
             Destroy(gameObject);
    }
    private void OnDestroy()
    {
        TimerAndReward.Instance.ReduceFatigue(1);
        Debug.Log("니거"+ ++ItemSpawnManager.instance.nigger);
    }
}




