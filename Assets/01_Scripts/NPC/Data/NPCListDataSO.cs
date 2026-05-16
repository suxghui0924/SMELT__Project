using UnityEngine;

namespace _01_Scripts.NPC.Data
{
    [CreateAssetMenu(fileName = "NPC List Data", menuName = "NPC List Data", order = 0)]
    public class NPCListDataSO : ScriptableObject
    {
        public GameObject[] CharacterSprites;
        
    }
}