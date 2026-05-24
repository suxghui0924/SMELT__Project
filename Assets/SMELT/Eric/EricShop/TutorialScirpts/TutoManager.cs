using System;
using System.Linq;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    public static TutoManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (InventoryManager.Instance._inventory != null)
        {
            string temp = InventoryManager.Instance._inventory.First().Key;
            InventoryManager.Instance._inventory.Remove(temp);
        }
    }

    [field:SerializeField]public TutoAttack TutoRAttack { get;private set; }
    [field:SerializeField]public TutoAttack TutoLAttack { get;private set; }
    [field:SerializeField]public TutoDungeonAndUpgrade TutoDungeon { get;private set; }
    [field:SerializeField]public TutoDungeonAndUpgrade TutoUpgrade { get;private set; }
    [field:SerializeField]public TutoEnemySpawn TutoEnemySpawn{ get;private set; }
    [field:SerializeField]public TutoOnTrigger TutoOnTrigger{ get;private set; }
    [field:SerializeField]public TutorialLine TutoLine{ get;private set; }
    [field:SerializeField]public TutorialSaying TutoSaying{ get;private set; }
    [field:SerializeField]public TutoHit TutoHit{ get;private set; }
    [field:SerializeField]public TutoOpenTrigger TutoOpenTrigger{ get;private set; }
    [field:SerializeField]public Dialogue Dialogue{ get;private set; }


}
