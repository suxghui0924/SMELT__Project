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

        private Stack<GameObject> npcStack = new Stack<GameObject>();
        private Stack<GameObject> npcReverseStack;

        private NPCMovement NpcMovement;
        
        private void Awake()
        {
        }

        private void OnEnable()
        {
            // Event
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
                HandleOnAcceptOrder();
            if(Input.GetKeyDown(KeyCode.H))
                HandleOnRewardOrder();
            }

        private void HandleOnAcceptOrder()
        {
            GameObject npcObject = Instantiate(npcPrefab.CharacterSprites[UnityEngine.Random.Range(0, npcPrefab.CharacterSprites.Length)]);
            npcStack.Push(npcObject);
        }
        private void HandleOnRewardOrder()
        {
          npcReverseStack = new Stack<GameObject>(npcStack.Reverse());
          for (int i = 0; i < npcReverseStack.Count; i++            )
          {
              NpcMovement = npcReverseStack.Pop().GetComponent<NPCMovement>();
              NpcMovement.IndexChange(NpcMovement._index);
          }
        }
        
        
    }
}
