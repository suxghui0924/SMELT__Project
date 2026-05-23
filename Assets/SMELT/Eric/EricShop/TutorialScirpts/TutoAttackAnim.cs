using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoAttackAnim : MonoBehaviour
{
    private void OnMove(InputValue value)
    {
        OnPlayerMove(value.Get<Vector2>().x);
    }
    private Animator _anim;
    private readonly int skillPlayHash = Animator.StringToHash("SkillTrigger");
    private Transform _playerTransform;
    private PlayerHitBox _playerHitBox;
    
    private void Start()
    {
        _playerHitBox=GameObject.Find("HitBox").GetComponent<PlayerHitBox>();
        _anim = GetComponent<Animator>();
        _playerTransform = GetComponentInParent<Transform>();
    }   
    private void OnPlayerMove(float x)
    {
        if (x>0)
        {
            _playerTransform.localScale = new Vector3(1, 1, 1); //Right
            _playerHitBox.CheckHit(1);
        }
        else if (x<0)
        {
            _playerTransform.localScale = new Vector3(-1, 1, 1); //Left
            _playerHitBox.CheckHit(0);
        }
        _anim.SetFloat(skillPlayHash, x);
        _anim.SetTrigger("Attack");
    }
}
