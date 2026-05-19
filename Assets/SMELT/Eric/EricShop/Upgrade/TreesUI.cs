using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreesUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI needMoney;
    public TextMeshProUGUI upName;
    public TextMeshProUGUI detail;
    
    [SerializeField]Upgrading upgrade;
    

    private void OnEnable()
    {
        upgrade = GetComponentInParent<Upgrading>();
        icon = transform.Find("Icon").GetComponent<Image>();
        needMoney = transform.Find("NeedMoney").GetComponent<TextMeshProUGUI>();
        upName = transform.Find("UpName").GetComponent<TextMeshProUGUI>();
        detail = transform.Find("Detail").GetComponent<TextMeshProUGUI>();
        upgrade.StatText();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
