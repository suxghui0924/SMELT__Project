using System;
using SMELT.LHS.LHS_Script.PlayerSkill;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private SkillInputSO _skillInput;
     private PlayerAnimation _skillAnim;
   public float _skillCoolDown;
    
    public float _nextAttackTime = 0f;
    private PickaxeDataSO _pickaxeSO;

    private void Start()
    {
        _skillAnim=GameObject.Find("PlayerVisual").GetComponent<PlayerAnimation>();
    }

    public void SkillCooldownUpdate(float bonus)
    {
        if (_pickaxeSO != null)
        {
            _skillCoolDown *= (1f - bonus - _pickaxeSO.speed);
        }
        else
        {
            _skillCoolDown *= (1f - bonus);
        }

        _skillCoolDown=Mathf.Clamp(_skillCoolDown, 0.05f, 1f); 
            Debug.Log("현재 평쿨 업데이트됨 " + _skillCoolDown);
       }
    public void GetPickaxeData(PickaxeDataSO pickaxeSO)
    {
        _pickaxeSO = pickaxeSO;
        Debug.Log("현재 곡괭이 업데이트됨 : "+ _pickaxeSO.name);
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
            _nextAttackTime = Time.time + _skillCoolDown;
            
            _skillAnim.OnPlayerAttack(dirX);
            SoundManager.instance.PlaySFX("Attack");
        }
    }
}
