using System.Collections;
using SMELT.LHS.LHS_Script.PlayerSkill;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private readonly int skillPlayHash = Animator.StringToHash("SkillTrigger");
    private bool canInput = true; //스킬 입력 가능 판별
    private Transform _playerTransform;
    public bool _leftHitBoxOn{ get; private set;}
    public bool _rightHitBoxOn { get; private set; }
    [SerializeField] private SkillInputSO _skillInput;
     private PlayerHitBox _playerHitBox;

    private void Start()
    {
        _playerHitBox=GameObject.Find("HitBox").GetComponent<PlayerHitBox>();
        _anim = GetComponent<Animator>();
        _playerTransform = GetComponentInParent<Transform>();
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

    public void LeftEnd()
    {
        _playerHitBox.LeftHitBoxEnd();
    }

    public void RightEnd()
    {
        _playerHitBox.RightHitBoxEnd();
    }

    public void OnPlayerAttack(float vector)
    {
        
                if (vector>0)
                {
                    _playerTransform.localScale = new Vector3(1, 1, 1); //Right
                    
                    _rightHitBoxOn = true; 
                    _playerHitBox.CheckHit(1);
                }
                else if (vector<0)
                {
                    _playerTransform.localScale = new Vector3(-1, 1, 1); //Left
                    _leftHitBoxOn = true;
                    _playerHitBox.CheckHit(0);
                }
                _anim.SetFloat(skillPlayHash, vector);
                _anim.SetTrigger("Attack");
    }
    
}