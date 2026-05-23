using System;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.NPC;
using _01_Scripts.NPC.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _01_Scripts.Player.Manager
{
    public class NPCSpawner : MonoBehaviour
    {
        public NPCListDataSO npcPrefab;
        
        public Transform StartPos;
        public Transform TurnPos;
        public Transform LastPos;

        private List<GameObject> npcList = new List<GameObject>();
        private int MAX_NPC_COUNT = 3;
        
        private void OnEnable()
        {
            OrderHUD.OnOrderCreated += HandleOnAcceptOrder;
            OrderHUD.OnOrderEnded += HandleOnRewardOrder;
        }

            private void OnDisable()
        {
            OrderHUD.OnOrderCreated -= HandleOnAcceptOrder;
            OrderHUD.OnOrderEnded -= HandleOnRewardOrder;
        }

        // ReSharper disable Unity.PerformanceAnalysis
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
}