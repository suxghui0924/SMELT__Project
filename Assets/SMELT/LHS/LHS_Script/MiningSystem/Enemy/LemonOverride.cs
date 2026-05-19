using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class LemonOverride : EnemyBase
{
    [SerializeField] private float lemonFlipTimer;
    [SerializeField] private float lemonFlipDuration;
    [SerializeField] private float lemonJumpPower;
    [SerializeField] private Sprite _lemonSprite2;
    private float timer = 0f;
    private SpriteRenderer _spriteRenderer;
    private Sequence _lemonSequence;
    private float _endPos;

    private void Awake()
    {
        base.Awake();
        _enemyCollider2D.enabled = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override IEnumerator KnockbackRoutine()
    {
        _canMove = false;
        float timer = 0;
        Vector3 knockbackDir = (transform.parent.position - _playerTransform.position);
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        float knockbackForce = knockbackPower * 4f;

        _melonRot = 0;
        while (timer <= knockbackTimer)
        {
            float progress = timer / knockbackTimer;
            float currentForce = Mathf.Lerp(knockbackForce, 0, progress);
            transform.parent.position += knockbackDir * (currentForce * Time.deltaTime);
            
            timer += Time.deltaTime;
            yield return null;
        }

        _canMove = true;
    }

    protected override void Update()
    {
        if (_canMove)
        {
            timer += Time.deltaTime;
            if (timer < lemonFlipTimer)
            {
                _enemyDirection = (transform.parent.position - _playerTransform.position).normalized;
                transform.parent.position -= _enemyDirection * (_enemySpeed * Time.deltaTime);
            }
            else
            {
                timer = 0f;
                _canMove = false;
                LemonFlip();
            }

        }
    }

    private void LemonFlip()
    {
        _spriteRenderer.sprite = _lemonSprite2;
        _lemonSequence = DOTween.Sequence();
        float CameraXaxis = Camera.main.transform.position.x;
        _endPos = 2 * Camera.main.transform.position.x - transform.parent.position.x;
        Vector3 targetPos = new Vector3(_endPos, _playerTransform.position.y, 0);
        _lemonSequence.Append(transform.parent.DOJump(targetPos, lemonJumpPower, 1, lemonFlipDuration));
        _lemonSequence.OnComplete(() => {
            transform.parent.Rotate(new Vector3(0, 180, 0));
            _enemyCollider2D.enabled = true;
            _canMove = true;
        });
    }
}