using System;
using SMELT.LHS.LHS_Script.PlayerSkill;
using Unity.VisualScripting;
using UnityEngine;

public class BossPlayerAttack : MonoBehaviour
{
    [SerializeField] private SkillInputSO _skillInput;
     private PlayerAnimation _skillAnim;
   public float _skillCoolDown;
    
    public float _nextAttackTime = 0f;

    private void Start()
    {
        _skillAnim=GameObject.Find("PlayerVisual").GetComponent<PlayerAnimation>();
    }

    private void OnEnable()
    {
        _skillInput.OnLeftKey += AttackLeft;
        _skillInput.OnRightKey += AttackRight;
    }

    private void OnDisable()
    {
        _skillInput.OnLeftKey -= AttackLeft;
        _skillInput.OnRightKey -= AttackRight;
    }

    private void AttackLeft() { TryAttack(-1f); }
    private void AttackRight() { TryAttack(1f); }

    public void TryAttack(float dirX)
    {
        if (Time.time >= _nextAttackTime)
        {
            SoundManager.instance.PlaySFX("Attack");
            _nextAttackTime = Time.time + _skillCoolDown;
            
            _skillAnim.OnPlayerAttack(dirX);
        }
    }
}
