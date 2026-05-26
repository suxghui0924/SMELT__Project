using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    public static TutoManager Instance;

   public GameObject skillTreeUI;
    [SerializeField] private GameObject[] lastObjects;

    public bool canLast;
    public bool canLastCoroutine = true;
    public bool isCanUpgrading = true;

    public Vector3[] targets;
    public GameObject[] targetObjects;

    public GameObject tree;

    public void TargetPos(int index)
    {
        targetObjects[0].transform.position = targets[index];
        targetObjects[1].SetActive(true);
    }

    public void Triggered()
    {
        targetObjects[1].SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        canLastCoroutine = true;
    }
    

    private void Update()
    {
        if (skillTreeUI == null )
        {
            skillTreeUI = GameObject.Find("Group_ShopSkillTree");
            return;
        }
        if(Upgrading != null)
            if (!Upgrading.isTutorial&&isCanUpgrading)
            {
                isCanUpgrading = false;
                Upgrading.isTutorial = true;
            }
            else
            {
                Upgrading = GameObject.Find("UpEveryOre1").GetComponent<Upgrading>();
            }
        if (!skillTreeUI.activeSelf&&canLastCoroutine)
        {
            canLastCoroutine = false;
            lastObjects[0].gameObject.SetActive(true);
            lastObjects[1].gameObject.SetActive(true);
            StartCoroutine(LastCoroutine());
        }
    }

    private IEnumerator LastCoroutine()
    {
        yield return null;
        TutoManager.Instance.Triggered();
        StartCoroutine(TutoSaying.SayingCoroutine(TutoLine.store4));
        tree.SetActive(false);
    }

    [field:SerializeField]public TutorialLine TutoLine{ get;private set; }
    [field:SerializeField]public TutorialSaying TutoSaying{ get;private set; }
    [field:SerializeField]public Dialogue Dialogue{ get;private set; }
    [field:SerializeField]public Upgrading Upgrading{ get;private set; }
    [field:SerializeField]public TutoExitDungeon TutoExitDungeon{ get;private set; }


}
