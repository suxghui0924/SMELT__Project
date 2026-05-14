using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AppleMaterial : MonoBehaviour
{
    private Sequence _appleSequence;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GameObject.Find("GettingParticle").GetComponent<ParticleSystem>();
    }

    public void AppleEnable(Vector2 screenPoint)
    {
        if(_appleSequence != null) _appleSequence.Kill();
        _appleSequence = DOTween.Sequence();
        
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10f));
        worldPoint.z = transform.position.z;
        Vector3 jumpPos = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 0f), 0);
        float RandomDelay = UnityEngine.Random.Range(0.6f, 1.2f);
        
        _appleSequence
            .Append(transform.DOJump(jumpPos, 0.5f,1,0.5f).SetEase(Ease.OutQuad))
            .AppendInterval(RandomDelay)
            .Append(transform.DOMoveX(worldPoint.x, 0.6f).SetEase(Ease.Linear))
            .Join(transform.DOMoveY(worldPoint.y, 0.6f).SetEase(Ease.InQuad))
            .OnComplete(() => {
                ParticlePlay();
                gameObject.SetActive(false);
            });
    }

    private void ParticlePlay()
    {
        particleSystem.transform.position = transform.position;
        particleSystem.Play();
    }


    private IEnumerator ParticleRoutine()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.startLifetime.constant);
        _spriteRenderer.enabled = true;
        gameObject.SetActive(false);
        ItemSpawnManager.instance.applePool.Push(gameObject);
    }
}
