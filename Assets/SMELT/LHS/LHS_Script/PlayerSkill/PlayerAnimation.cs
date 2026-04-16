using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _anim;
    private readonly int skillPlayHash = Animator.StringToHash("SkillTrigger");
    [SerializeField] float _skillCoolDown;
    private bool canInput = true;
    private Vector2 _vector2;
    private Transform _playerTransform;
    public bool _leftHitBoxOn{ get; private set;}
    public bool _rightHitBoxOn { get; private set; }
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _playerTransform = GetComponentInParent<Transform>();
    }

    private void Update()
    {
        if (_vector2 != Vector2.zero)
        {
          
            if (canInput == true)
            {
                StartCoroutine(CoolTime());
                if (_vector2.x == 1f)
                {
                    _playerTransform.rotation = Quaternion.Euler(0, 0, 0); //Right
                    _anim.SetFloat(skillPlayHash, 1f);
                    _rightHitBoxOn = true; 
                }
                else if (_vector2.x == -1f)
                {
                    _playerTransform.rotation = Quaternion.Euler(0, 180, 0); //Left
                    _anim.SetFloat(skillPlayHash, -1f);
                    _leftHitBoxOn = true;
                }
            }
        }
    }
    private IEnumerator CoolTime()
    {
        canInput = false;
        yield return new WaitForSeconds(_skillCoolDown);
        canInput = true;
        _anim.SetFloat(skillPlayHash, 0f);
        _rightHitBoxOn = false;
        _leftHitBoxOn = false;
    }
    private void OnMove(InputValue value)
    {
        _vector2 = value.Get<Vector2>();
    }
}