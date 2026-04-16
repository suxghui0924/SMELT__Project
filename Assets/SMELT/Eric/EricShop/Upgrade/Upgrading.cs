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

    private bool first;
    private void Start()
    {
        UpdateTreesUI();
        if (upso.needName == "First") 
            first = true;

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
    private void Update()
    {
        if (!first)
            CanUp();
    }
    public void OnClickUpgradeButton()
    {
        bool isSuccess = PlayerStatManager.Instance.BuyUpgrade(upso.upName, upso.needMoney, upso.upTime);

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
    private void CanUp()
    {
        bool isCompleted1 = PlayerStatManager.Instance.IsUpgradePurchased(upso.upName);
        bool isCompleted2 = PlayerStatManager.Instance.IsUpgradePurchased(upso.needName);

        if (!isCompleted1 && isCompleted2)
        {
            upgradeButton.interactable = true;
            checkMark.SetActive(false);
        }
        else
        {
            upgradeButton.interactable = false;
            checkMark.SetActive(true);
        }
    }
}