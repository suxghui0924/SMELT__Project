using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _01_Scripts.NPC;
using _01_Scripts.NPC.Data;

public class TutoExitDungeon : MonoBehaviour
{
    [SerializeField] private GameObject[] dungeon;
    [SerializeField] private GameObject[] house;
    public Transform StartPos;
    public Transform TurnPos;
    public Transform LastPos;
    public void ExitDungeon()
    {
        if (!TutoManager.Instance.TutoSaying.canExitDungeon) return;
        TutoManager.Instance.TutoSaying.ui1 = true;
        dungeon[0].SetActive(false);
        dungeon[1].SetActive(false);
        house[0].SetActive(true);
        house[1].SetActive(true);
        
        StartPos = GameObject.Find("StartPos").transform;
        TurnPos = GameObject.Find("TurnPos").transform;
        LastPos = GameObject.Find("LastPos").transform;
        
        StartCoroutine(ShopOpenCoroutine());
    }
    
    public NPCListDataSO npcPrefab;
        


    private List<GameObject> npcList = new List<GameObject>();
    private int MAX_NPC_COUNT = 3;

    private IEnumerator ShopOpenCoroutine()
    {
        OrderHUD.OnOrderCreated += HandleOnAcceptOrder;
        OrderHUD.OnOrderEnded += HandleOnRewardOrder;
        yield return new WaitForSeconds(1f);
        OrderHUD.OnOrderCreated -= HandleOnAcceptOrder;
        OrderHUD.OnOrderEnded -= HandleOnRewardOrder;
    }
    
    private void HandleOnAcceptOrder(Leedoyun_CustomerOrder order)
    {
        if (npcList.Count >= MAX_NPC_COUNT)
        {
            Debug.LogWarning("Quene Order Over");
            return;
        }
        GameObject prefab =
            npcPrefab.CharacterSprites[UnityEngine.Random.Range(0, npcPrefab.CharacterSprites.Length)];
        GameObject npcObject = Instantiate(prefab, transform);

        npcObject.transform.position = StartPos.position;
            
        npcList.Add(npcObject);
    
        int grantIndex = npcList.Count;
        npcObject.GetComponent<NPCMovement>().IndexChange(grantIndex);
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleOnRewardOrder(Leedoyun_CustomerOrder order)
    {
        if (npcList.Count == 0) return;

        GameObject finishedNPC = npcList[0];
        npcList.RemoveAt(0);
            
        if(finishedNPC != null)
            finishedNPC.GetComponent<NPCMovement>().IndexChange(0);

        for (int _ = 0; _ < npcList.Count; _++)
        {
            if (npcList[_] == null) continue;
            npcList[_].GetComponent<NPCMovement>().IndexChange(_ + 1);
        }
    }
}
