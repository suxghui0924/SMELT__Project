using System;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxOnTrigger : MonoBehaviour
{
    private PickaxeDataSO _pickaxeSO;
    private PlayerHitBox _playerHitBox;
    private List<GameObject> _enemies = new List<GameObject>(); //한번 히트박스에서 데미지를 입으면 계속 데미지가 들어오는걸 방지하기 위한 리스트
    public int currentDamage;
    private void Start()
    {
        _playerHitBox = GetComponentInParent<PlayerHitBox>();
    }
    
    public void GetPickaxeData(PickaxeDataSO pickaxeSO)
    {
        _pickaxeSO = pickaxeSO;
        Debug.Log("현재 곡괭이 업데이트됨 : "+ _pickaxeSO.name);
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
                    if (_pickaxeSO != null)
                    { 
                        SoundManager.instance.PlaySFX("Parry");
                    _enemyBase.OnEnemyDamaged(_pickaxeSO.damage);
                        
                    }
                    else
                    {
                        _enemyBase.OnEnemyDamaged(1);
                        SoundManager.instance.PlaySFX("Parry");
                    }
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
