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
    private void Start()
    {
        UpdateTreesUI();
    }

    public void UpdateTreesUI()
    {
        bool isCompleted = PlayerStatManager.Instance.IsUpgradePurchased(upso.upName);

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
    public void OnClickUpgradeButton()
    {
        bool isSuccess = PlayerStatManager.Instance.BuyUpgrade(upso.upName, upso.needMoney);

        if (isSuccess)
        {
            Debug.Log("업그레이드 성공!");
            UpdateTreesUI();
        }
        else
        {
            Debug.Log("업그레이드 실패! 돈이 부족합니다.");
        }
    }
}