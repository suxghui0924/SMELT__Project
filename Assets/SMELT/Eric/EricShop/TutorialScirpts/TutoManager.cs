using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    public static TutoManager Instance;

    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject[] lastObjects;

    public bool canLast;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        skillTreeUI = GameObject.Find("Group_ShopSkillTree");
        if (skillTreeUI == null) return;
        if (!skillTreeUI.activeSelf)
        {
            lastObjects[0].gameObject.SetActive(true);
            lastObjects[1].gameObject.SetActive(true);
            StartCoroutine(LastCoroutine());
        }
    }

    private IEnumerator LastCoroutine()
    {
        yield return null;
        StartCoroutine(TutoSaying.SayingCoroutine(TutoLine.store4));
        if (canLast)
        {
            StartCoroutine(TutoSaying.SayingCoroutine(TutoLine.changeBgm2));
            canLast = false;
        }

        if (canLast)
        {
            StartCoroutine(TutoSaying.SayingCoroutine(TutoLine.nextDay2));

        }

        if (canLast)
        {
            StartCoroutine(TutoSaying.SayingCoroutine(TutoLine.last));
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
