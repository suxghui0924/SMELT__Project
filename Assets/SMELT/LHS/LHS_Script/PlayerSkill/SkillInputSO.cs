using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SMELT.LHS.LHS_Script.PlayerSkill
{
    [CreateAssetMenu(fileName = "SkillInputSO", menuName = "Scriptable Objects/SkillInputSO", order = 0)]
    public class SkillInputSO : ScriptableObject, ParrySystem.IPlayerSkillActions
    {
        [HideInInspector]
        public Vector2 moveDir;
        public event Action OnLeftKey; 
        public event Action OnRightKey; 
        private ParrySystem _playerSkillActions;


        private void OnEnable()
        {
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

        public void OnLeftSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("OnLeftSkill");
                OnLeftKey?.Invoke();
            }
        }

        public void OnRightSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("OnRightSkill");
                OnRightKey?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {   
            if(context.performed)
                moveDir = context.ReadValue<Vector2>();
        }
    }
}