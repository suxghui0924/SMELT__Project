using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreesUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SOUpgrading _soup;
    [SerializeField] private GameObject ui;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI needMoney;
    [SerializeField] private TextMeshProUGUI upName;
    [SerializeField] private TextMeshProUGUI detail;

    private void Start()
    {
        ui.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
           ui.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.SetActive(false);
    }
}
