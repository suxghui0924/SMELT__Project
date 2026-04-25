using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 플레이어의 스킬들을 담당하는 인풋시스템
///
/// [사용법]
/// 다른 스크립트에서 인풋이 들어오면 실행하게 하고 싶은것들을 아래처럼 +=로 더하면 자동으로 가능함
/// _skillInput.OnLeftKey += 실행하고 싶은 메서드명;
/// _skillInput.OnRightKey += 실행하고 싶은 메서드명;
///
/// 담당자:<이호승>
/// </summary>
namespace SMELT.LHS.LHS_Script.PlayerSkill
{
    [CreateAssetMenu(fileName = "SkillInputSO", menuName = "Scriptable Objects/SkillInputSO", order = 0)]
    public class SkillInputSO : ScriptableObject, ParrySystem.IPlayerSkillActions
    {
        public static SkillInputSO Instance;
        [HideInInspector]
        public Vector2 moveDir;
        
        public event Action OnLeftKey; 
        public event Action OnRightKey; 
        private ParrySystem _playerSkillActions;


        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (_playerSkillActions == null)
            {
                _playerSkillActions = new ParrySystem();
                _playerSkillActions.PlayerSkill.SetCallbacks(this);
            }
            _playerSkillActions.PlayerSkill.Enable();
        }

        private void OnDisable()
        {
            _playerSkillActions.PlayerSkill.RemoveCallbacks(this);
            _playerSkillActions.PlayerSkill.Disable();
        }

        public void OnLeftSkill(InputAction.CallbackContext context) //A나 왼쪽 화살표를 누르면 활성화
        {
            if (context.started)
            {
                Debug.Log("OnLeftSkill");
                OnLeftKey?.Invoke();
            }
        }

        public void OnRightSkill(InputAction.CallbackContext context) //D나 오른쪽 화살표를 누르면 활성화
        {
            if (context.started)
            {
                Debug.Log("OnRightSkill");
                OnRightKey?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext context) // wasd 방향 감지용 
        {   
            if(context.performed)
                moveDir = context.ReadValue<Vector2>();
        }
    }
}