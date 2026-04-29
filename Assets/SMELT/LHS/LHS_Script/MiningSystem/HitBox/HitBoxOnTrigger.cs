using System;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxOnTrigger : MonoBehaviour
{
    private PlayerHitBox _playerHitBox;
    private List<GameObject> _enemies = new List<GameObject>(); //한번 히트박스에서 데미지를 입으면 계속 데미지가 들어오는걸 방지하기 위한 리스트
    public PickaxeSO _pickaxeStat;
    private void Start()
    {
        _playerHitBox = GetComponentInParent<PlayerHitBox>();
    }

    private void OnEnable()
    {
        _enemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")&&_playerHitBox._triggerOn)
        {
            if (!_enemies.Contains(other.gameObject))
            {
                if (other.TryGetComponent<EnemyBase>(out EnemyBase _enemyBase))
                {
                    _enemyBase.OnEnemyDamaged(_pickaxeStat.PickaxeDamage);
                }
                _enemies.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (_enemies.Contains(other.gameObject))
            {
                _enemies.Remove(other.gameObject);
            }
        }
    }
}
