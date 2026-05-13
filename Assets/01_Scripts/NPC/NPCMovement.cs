using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace _01_Scripts.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private Transform _startPos;
        [SerializeField] private Transform _turnPos;
        [SerializeField] private Transform _lastPos;
        [SerializeField] private float _timer;
        [SerializeField] private float _offset;
        [SerializeField] private int _index;
        private bool _isMoving;
        private bool _isTurn;
        private bool _isTurned;
        private Sequence mySequence;
     
        private void OnEnable()
        {
            transform.DOKill();
            Move();
        }
        
        #region test

        private int f = 1;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                IndexChange(f++);
            }
        }

        #endregion

        public void IndexChange(int index)
        {
            _index = index;
            Move();
        }
        #region NPCMovement
        private void Move()
        {
            if (!_isMoving)
            {
                this.gameObject.transform.position = _startPos.position;
                _isMoving = true;
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOMove(_lastPos.position + (( Vector3.down * _index * _offset)), _timer));
                mySequence.OnComplete(Turn);
            }
            else if (_isTurn || _isTurned)
            {
                if (_isTurned)
                    Turned();
                if(_isTurn)
                    Turn();
            }
            else
            {
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOMove(_lastPos.position + (( Vector3.down * _index * _offset)), _timer));
                mySequence.OnComplete(Turn);
            }
        }

        private void Turn()
        {
            _isTurn = true;
            if (_isTurn && _index == 1)
            {
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOMove(_turnPos.position, _timer / 2));
                mySequence.OnComplete(Turned);
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
        }
    }
}