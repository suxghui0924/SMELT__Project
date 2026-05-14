using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyDataSo;

    [SerializeField] protected float knockbackTimer;
    [SerializeField] protected float knockbackPower;
    protected Transform _playerTransform;
    public int CurrentHp{get; private set;}
    protected float _enemySpeed;
    protected int _enemyDamage;
    protected string _enemyName;
    protected Vector3 _enemyDirection;
    protected Collider2D _enemyCollider2D;
    protected bool _canMove = true;
    Player_HpScript _playerHpScript;


    private float _melonRot=0;
    
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
        {
            
            transform.position -= _enemyDirection * (_enemySpeed * Time.deltaTime);
            if (_enemyName == "Melon") transform.Rotate(0,0,_melonRot++*Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
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
    protected virtual IEnumerator KnockbackRoutine()
    {
        _canMove = false;
        float timer = 0;
        Vector3 knockbackDir = (transform.position - _playerTransform.position);
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        float knockbackForce = knockbackPower * 4f; 

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
        //TimerAndReward.Instance.ReduceFatigue(1);
    }
}




