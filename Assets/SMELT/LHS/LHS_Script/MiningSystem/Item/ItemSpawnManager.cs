using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [HideInInspector] public Stack<GameObject>[] itemPools;
    
    [SerializeField]private int itemPoolCount =6;

    [SerializeField] private GameObject[] enemyItemPrefab;
    
    
    [SerializeField] private Transform inventoryUIPos;
    [SerializeField]private Camera uiCamera;
    private FruitMaterialLogic _fruitMaterialLogic;

    private Vector2 _screenPoint;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        _screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, inventoryUIPos.position);
        itemPools = new Stack<GameObject>[5];
          for(int i=0;i<itemPools.Length;i++)  itemPools[i] = new Stack<GameObject>();
        CreateEnemyItem();
    }
    
    public void SpawnItem(int itemIndex, Transform enemyDeadPos, int againValue)
    {
        for (int i = 0; i < againValue; i++)
        {
            GameObject item;
            if (itemPools[itemIndex].Count > 0)
            {
                item = itemPools[itemIndex].Pop();
                item.transform.position = enemyDeadPos.position;
                _fruitMaterialLogic = item.GetComponent<FruitMaterialLogic>();
                if (_fruitMaterialLogic != null) _fruitMaterialLogic.enemyMaterialEnable(_screenPoint);
                item.SetActive(true);
            }
            else item = Instantiate(enemyItemPrefab[itemIndex]);
        }
    }

    private void CreateEnemyItem()
    {
        for(int i=0;i<itemPools.Length;i++)
        {
             for (int j = 0; j < itemPoolCount; j++)
             {
                GameObject enemy  = Instantiate(enemyItemPrefab[i]);
                enemy.SetActive(false); 
                itemPools[i].Push(enemy);
             }
        }
    }
}
