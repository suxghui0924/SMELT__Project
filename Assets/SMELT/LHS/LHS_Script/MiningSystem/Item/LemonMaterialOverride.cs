using System.Collections;
using UnityEngine;

public class LemonMaterialOverride : FruitMaterialLogic
{
    protected override void Awake()
    {
        particleSystem = GameObject.Find("LemonParticle").GetComponent<ParticleSystem>();
    }
    
    protected override IEnumerator ParticleRoutine()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.startLifetime.constant);
        _spriteRenderer.enabled = true;
        gameObject.SetActive(false);
        ItemSpawnManager.instance.itemPools[2].Push(gameObject);
    }
}
