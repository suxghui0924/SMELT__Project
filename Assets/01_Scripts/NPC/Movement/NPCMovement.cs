<<<<<<< HEAD
﻿    using System;
=======
﻿using System;
>>>>>>> parent of 33da136 (Revert "Reapply "Merge branch 'base' into Eric/Upgrade"")
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
<<<<<<< HEAD
        
        public int Index { get; private set; }
=======
        public int _index { get; private set; }
>>>>>>> parent of 33da136 (Revert "Reapply "Merge branch 'base' into Eric/Upgrade"")

        private Transform _startPos;
        private Transform _turnPos;
        private Transform _lastPos;

        private bool _isMoving;
        private bool _isTurn;
        private bool _isTurned;
        private bool _checkLastPos;
<<<<<<< HEAD

        private Tween _moveTweener;
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
=======
        
        private Sequence mySequence;

        private NPCSpawner NPCSpawner;
        
        private void Awake()
        {
             
        }

        private void OnEnable()
        {
            NPCSpawner = gameObject.GetComponentInParent<NPCSpawner>();
         
            _startPos = NPCSpawner.StartPos;
            _turnPos = NPCSpawner.TurnPos;
            _lastPos = NPCSpawner.LastPos;  
            _index = 3;
            transform.DOKill();
            Move();
>>>>>>> parent of 33da136 (Revert "Reapply "Merge branch 'base' into Eric/Upgrade"")
        }
        
        #region test

        private int f = 3;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                IndexChange(f--);
            }
        }

        #endregion

<<<<<<< HEAD
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
                Vector3 targetPosition = _lastPos.position + (Vector3.down * ((Index - 1) * _offset));
                _moveTweener = transform.DOMove(targetPosition, _timer).SetEase(Ease.OutQuad);
            }
        }

        private void ExitQuene()
        {
            _exitSequence = DOTween.Sequence();

            _exitSequence.Append(transform.DOMove(_turnPos.position, _timer * 0.5f).SetEase(Ease.Linear))
                .Append(transform.DOMoveY(_startPos.position.y, _timer).SetEase(Ease.Linear))
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
=======
        public void IndexChange(int index)
        {
            _index = --index;
            Move();
        }
        #region NPCMovement
        private void Move()
        {
            if ( _index == 0 || _checkLastPos)
            {
                Turn();
            }
            else
            {
                if (!_isMoving)
                {
                    this.gameObject.transform.position = _startPos.position;
                    _isMoving = true;
                    mySequence = DOTween.Sequence();
                    mySequence.Append(transform.DOMove(_lastPos.position + ((Vector3.down * _index * _offset)),
                        _timer));
                    if(_index == 1)
                        mySequence.OnComplete(() => _checkLastPos = true);
                }
                else if (_isTurn || _isTurned)
                {
                    if (_isTurned)
                        Turned();
                    if (_isTurn)
                        Turn();
                }
                else
                {
                    mySequence = DOTween.Sequence();
                    mySequence.Append(transform.DOMove(_lastPos.position + ((Vector3.down * _index * _offset)),
                        _timer));
                    if(_index == 1)
                        mySequence.OnComplete(() => _checkLastPos = true);
                }
            }
        }

        private void Turn()
        {
            _isTurn = true;
            if (_isTurn && !_isTurned)
            {
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOMove(_turnPos.position, _timer / 2));
                mySequence.OnComplete(Turned);
            }
            else
            {
                Turned();
            }
        }

        private void Turned()
        {
            _isTurned = true;
            if (_isTurned)
            {
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOMoveY(_startPos.position.y, _timer / 2));
            }
        }
        #endregion
        private void OnDisable()
        {
            if (mySequence == null) return;
            mySequence.Kill();
            mySequence = null;
>>>>>>> parent of 33da136 (Revert "Reapply "Merge branch 'base' into Eric/Upgrade"")
        }
    }
}