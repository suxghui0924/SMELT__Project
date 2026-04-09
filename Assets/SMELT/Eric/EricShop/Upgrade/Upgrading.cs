using System.Collections.Generic;
using System.Data;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PlayerStatManager;
using static SaveManager;


public class Upgrading : MonoBehaviour
{
    //private PlayerStatManager plStatData;
    [SerializeField] private SOUpgrading upso;

    public Button upgradeButton;
    public GameObject checkMark;
    public SOUpgrading myTrees;
    private void Start()
    {
        UpdateTreesUI();
    }

    public void UpdateTreesUI()
    {
        bool isCompleted = PlayerStatManager.Instance.IsUpgradePurchased(myTrees.upName);

        if (isCompleted)
        {
            upgradeButton.interactable = false; 
            checkMark.SetActive(true);
        }
        else
        {
            upgradeButton.interactable = true;
            checkMark.SetActive(false);
        }
    }

}