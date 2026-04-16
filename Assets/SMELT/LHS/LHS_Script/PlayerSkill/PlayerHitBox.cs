using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField] PickaxeSO _pickaxeSO;
    private GameObject[]  hitBoxes;
    private PlayerAnimation _anim;
    private List<Collider2D> _alreadyHit;
    private bool _prevRightHitBoxOn;
    private bool _prevLeftHitBoxOn;
    
    [SerializeField] private LayerMask _enemyLayer;
    private void Start()
    {
        _anim = GameObject.Find("PlayerVisual").GetComponent<PlayerAnimation>();
//        _enemyLayer = LayerMask.GetMask("Enemy");
        int arrayLength = transform.childCount;
        hitBoxes = new GameObject[arrayLength];
        for (int i = 0; i < arrayLength; i++)
            hitBoxes[i] = transform.GetChild(i).gameObject;
    }

    private void Update()
    {
        if (_anim._rightHitBoxOn)
        {
            if(!_prevRightHitBoxOn) _alreadyHit = new List<Collider2D>();
            
            Vector2 size = new Vector2(hitBoxes[1].transform.lossyScale.x, hitBoxes[1].transform.lossyScale.y) / 2;
            Collider2D[] collider2D = Physics2D.OverlapBoxAll(new Vector2(hitBoxes[1].transform.position.x, hitBoxes[1].transform.position.y),
                size, 0, _enemyLayer);

            if (collider2D != null)
            {
                 for (int i = 0; i < collider2D.Length; i++)
                    { 
                        if (!_alreadyHit.Contains(collider2D[i]) )
                        {
                            EnemyBase _enemyBase = collider2D[i].GetComponent<EnemyBase>();
                            _enemyBase.OnEnemyDamaged(_pickaxeSO.PickaxeDamage);
                            _alreadyHit.Add(collider2D[i]);
                            
                        }
                        
                    }   
            }
            _prevRightHitBoxOn = true;
        }
        else
        {
            _prevRightHitBoxOn = false;
        }
        if (_anim._leftHitBoxOn)
        {
            if(!_prevLeftHitBoxOn) _alreadyHit = new List<Collider2D>();
            Vector2 size = new Vector2(hitBoxes[0].transform.lossyScale.x, hitBoxes[0].transform.lossyScale.y) / 2;
            Collider2D[] collider2D = Physics2D.OverlapBoxAll(new Vector2(hitBoxes[0].transform.position.x, hitBoxes[0].transform.position.y),
                size, 0, _enemyLayer);

            if (collider2D != null)
            {
                for (int i = 0; i < collider2D.Length; i++)
                { 
                    if (!_alreadyHit.Contains(collider2D[i]) )
                    {
                        EnemyBase _enemyBase = collider2D[i].GetComponent<EnemyBase>();
                        _enemyBase.OnEnemyDamaged(_pickaxeSO.PickaxeDamage);
                        _alreadyHit.Add(collider2D[i]);
                            
                    }
                        
                }   
            }
            _prevLeftHitBoxOn = true;
        }
        else
        {
            _prevRightHitBoxOn = false;
        }
        
        
        
    }
}
