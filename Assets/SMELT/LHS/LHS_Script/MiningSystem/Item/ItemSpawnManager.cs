using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;
    
    public Stack<GameObject> applePool = new Stack<GameObject>();
    [SerializeField]private int appleCount = 10;
    [SerializeField]private GameObject appleItemPrefab;
    [SerializeField] private Transform inventoryUIPos;
    [SerializeField]private Camera uiCamera;
    private AppleMaterial _appleMaterial;

    private Vector2 _screenPoint;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        CreateAppleItem();
        _screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, inventoryUIPos.position);
    }
    
    public void AppleItemSpawn(Transform enemyDeadPos,int againValue)
    {
        for (int i = 0; i < againValue; i++)
        {
            GameObject apple;
            if (applePool.Count > 0)
            {
                apple = applePool.Pop();
                apple.transform.position = enemyDeadPos.position;
                _appleMaterial = apple.GetComponent<AppleMaterial>();
                if (_appleMaterial != null) _appleMaterial.AppleEnable(_screenPoint);
                apple.SetActive(true);
            }
            else apple = Instantiate(appleItemPrefab);
        }
    }

    private void CreateAppleItem()
    {
        for (int i = 0; i < appleCount; i++)
        {
            GameObject apple  = Instantiate(appleItemPrefab);
            apple.SetActive(false);
            applePool.Push(apple);
        }
    }
}
