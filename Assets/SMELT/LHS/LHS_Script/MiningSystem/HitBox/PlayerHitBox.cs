using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class PlayerHitBox : MonoBehaviour
{
    [SerializeField] PickaxeSO _pickaxeSO;
    private GameObject[]  _hitBoxes;
    private PlayerAnimation _anim;
    public Collider2D _leftColl { get; private set; }
    public Collider2D _rightColl { get; private set; }

[HideInInspector]
    public bool _triggerOn = false;
     
    private PlayerAttack _att;

    private void Start()
    {
        
        _att = GameObject.Find("PlayerVisual").GetComponent<PlayerAttack>();
        _anim = GameObject.Find("PlayerVisual").GetComponent<PlayerAnimation>();
        _leftColl = transform.GetChild(0).GetComponent<Collider2D>();
        _rightColl = transform.GetChild(1).GetComponent<Collider2D>();
        
        
        int arrayLength = transform.childCount;
        _hitBoxes = new GameObject[arrayLength];
        for (int i = 0; i < arrayLength; i++)
            _hitBoxes[i] = transform.GetChild(i).gameObject;
        _leftColl.enabled = false;
        _rightColl.enabled = false;

      
    }
    

    public void CheckHit(int direction)
    {
        if (direction == 0) 
        {
            _leftColl.enabled = true;
            _triggerOn = true;
        }
        else if (direction == 1)
        {
            _rightColl.enabled = true;
            _triggerOn = true;
        }

        
       
        /*_alreadyHit = new List<Collider2D>();
        
        GameObject targetBox = hitBoxes[direction];
        Vector2 size = targetBox.transform.lossyScale / 2;

        Collider2D[] results = Physics2D.OverlapBoxAll(targetBox.transform.position, size, 0, _enemyLayer);
        
        foreach (var col in results)
        {
            if (!_alreadyHit.Contains(col))
            {
                if (col.TryGetComponent(out EnemyBase enemy))
                {
                    enemy.OnEnemyDamaged(_pickaxeSO.PickaxeDamage);
                    _alreadyHit.Add(col);
                }
            }
        }*/
    }
    public void RightHitBoxEnd()
             {
                 _rightColl.enabled = false;
                 _triggerOn = false;
             }
    public void LeftHitBoxEnd()
    {
        _leftColl.enabled = false;
        _triggerOn = false;
    }
}
