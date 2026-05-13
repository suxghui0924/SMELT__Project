using System;
using _01_Scripts.NPC.Data;
using UnityEngine;

namespace _01_Scripts.Player.Manager
{
    public class NPCSpawner : MonoBehaviour
    {
        public NPCListDataSO npcPrefab;
        public Transform StartPos;
        public Transform TurnPos;
        public Transform LastPos;
        private void Awake()
        {
            
        }
    }
}