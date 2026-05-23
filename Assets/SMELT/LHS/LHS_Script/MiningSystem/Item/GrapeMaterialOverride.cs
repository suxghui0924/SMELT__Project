using System.Collections;
using UnityEngine;

public class GrapeMaterialOverride : FruitMaterialLogic
{
    protected override void Awake()
    {
        particleSystem = GameObject.Find("GrapeParticle").GetComponent<ParticleSystem>();
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

    protected override void AddEconomy(int amount)
    { 
        SaveData data = SaveManager.Instance.CurrentData;
        float bonus = data.getGrape + data.getFriuts;
        int finalAmount = Mathf.RoundToInt(amount * (1 + bonus));
        InventoryManager.Instance.AddItem("fruitstone_grape", finalAmount);
    }
}
