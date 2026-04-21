using System.Collections;
using SMELT.LHS.LHS_Script.PlayerSkill;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private readonly int skillPlayHash = Animator.StringToHash("SkillTrigger");
    [SerializeField] float _skillCoolDown;
    private bool canInput = true;
    private Transform _playerTransform;
    public bool _leftHitBoxOn{ get; private set;}
    public bool _rightHitBoxOn { get; private set; }
    [SerializeField] private SkillInputSO _skillInput;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _playerTransform = GetComponentInParent<Transform>();

        _skillInput.OnLeftKey += AttackLeft;
        _skillInput.OnRightKey += AttackRight;
        
    }   

    private void OnDestroy()
    {
        _skillInput.OnLeftKey -= AttackLeft;
        _skillInput.OnRightKey -= AttackRight;
    }
    private void AttackLeft() { OnPlayerAttack(-1f); }
    private void AttackRight() { OnPlayerAttack(1f); }
    private IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(_skillCoolDown);
        canInput = true;
    }

    public void OnAttackEnd()
    {
        _anim.SetFloat(skillPlayHash, 0f);
        _rightHitBoxOn = false;
        _leftHitBoxOn = false;
    }
    
    private void OnMove()
    {
       OnPlayerAttack(_skillInput.moveDir.x);
    }

    private void OnPlayerAttack(float vector)
    {
        
            if (canInput == true)
            {
               canInput = false;
                if (vector>0)
                {
                    _playerTransform.localScale = new Vector3(1, 1, 1); //Right
                    
                    _rightHitBoxOn = true; 
                }
                else if (vector<0)
                {
                    _playerTransform.localScale = new Vector3(-1, 1, 1); //Left
                    _leftHitBoxOn = true;
                }
                _anim.SetFloat(skillPlayHash, vector);
                _anim.SetTrigger("Attack");
                StartCoroutine(CoolTime());
            }
            
    }
    
}