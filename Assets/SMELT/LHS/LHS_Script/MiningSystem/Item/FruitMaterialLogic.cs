using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FruitMaterialLogic : MonoBehaviour
{
    public int amount;
    private Sequence _enemeySequence;
    protected SpriteRenderer _spriteRenderer;
    protected ParticleSystem particleSystem;
    
    protected virtual void Awake()
    {
        particleSystem = GameObject.Find("AppleParticle").GetComponent<ParticleSystem>();
    }

    public void enemyMaterialEnable(Vector2 screenPoint)
    {
        AddEconomy(amount);
        if(_enemeySequence != null) _enemeySequence.Kill();
        _enemeySequence = DOTween.Sequence();
        
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10f));
        worldPoint.z = transform.position.z;
        Vector3 jumpPos = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 0f), 0);
        float RandomDelay = UnityEngine.Random.Range(0.6f, 1.2f);
        
        _enemeySequence
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

    protected virtual void AddEconomy(int amount)
    {
        Debug.Log("테스트테스트 "+ PlayerStatManager.Instance.UpGetFruits);
        SaveData data = SaveManager.Instance.CurrentData;
        float bonus = data.getApple + data.getFriuts;
        Debug.Log("테스트테스트2 "+ bonus);
        int finalAmount = Mathf.RoundToInt(amount * (1 + bonus));
        InventoryManager.Instance.AddItem("fruitstone_apple", finalAmount);
        Debug.Log("AddEconomy" + InventoryManager.Instance.GetQuantity("fruitstone_apple"));
    }
    protected virtual IEnumerator ParticleRoutine()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.startLifetime.constant);
        _spriteRenderer.enabled = true;
        gameObject.SetActive(false);
        ItemSpawnManager.instance.itemPools[0].Push(gameObject);
    }
     
}
