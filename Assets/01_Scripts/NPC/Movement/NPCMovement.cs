    using System;
using _01_Scripts.Player.Manager;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace _01_Scripts.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private float _timer;
        [SerializeField] private float _offset;
        
        public int Index { get; private set; }

        private Transform _startPos;
        private Transform _turnPos;
        private Transform _lastPos;

        private bool _isMoving;
        private bool _isTurn;
        private bool _isTurned;
        private bool _checkLastPos;

        private Tween _moveTweener;
        private Sequence _turnSequence;
        private Sequence _exitSequence;
        private NPCSpawner _spawner;

        private void OnEnable()
        {
            _spawner = GetComponentInParent<NPCSpawner>();
            if (_spawner != null)
            {
                _startPos = _spawner.StartPos;
                _turnPos = _spawner.TurnPos;
                _lastPos = _spawner.LastPos;  
            }
            IndexChange(Index);
        }
        
        #region test

        private int f = 3;
        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                IndexChange(f--);
            }
        }*/

        #endregion

        public void IndexChange(int newIndex)
        {
            Index = newIndex;
        
            KillAllTweens();

            if (Index == 0)
            {
                ExitQuene();
            }
            else
            {
                Vector3 targetPosition = _lastPos.position + (Vector3.left * ((Index - 1) * _offset));
                _moveTweener = transform.DOMove(targetPosition, _timer).SetEase(Ease.OutQuad);
            }
        }

        private void ExitQuene()
        {
            _exitSequence = DOTween.Sequence();
            
            if (!_isTurn)
            {
                _exitSequence.Append(transform.DOMove(_turnPos.position, _timer * 0.5f).SetEase(Ease.Linear))
                    .OnComplete(() => _isTurn = true);  
            }

            _exitSequence.Append(transform.DOMoveX(_startPos.position.x, _timer).SetEase(Ease.Linear))
                .OnComplete(() => Destroy(gameObject));
        }

        private void KillAllTweens()
        {
            if(_moveTweener != null && _moveTweener.IsActive()) _moveTweener.Kill();
            if(_exitSequence !=null && _exitSequence.IsActive()) _exitSequence.Kill();
        }
        
        private void OnDisable()
        {
            KillAllTweens();
        }
    }
}