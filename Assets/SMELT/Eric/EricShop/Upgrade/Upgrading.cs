using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Upgrading : MonoBehaviour
{
    //private PlayerStatManager plStatData;
    [SerializeField] private SOUpgrading upso;
    private string upNamed;
    private float upTimed;
    private int needMoneyd;
    private bool buyUp;
    public Button but;

    private void Awake()
    {
        upNamed = upso.upName;
        upTimed = upso.upTime;
        needMoneyd = upso.needMoney;
        //plStatData = GetComponent<PlayerStatManager>(b);
        but.onClick.AddListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        //buyUp = plStatData.BuyUpgrade(upNamed, needMoneyd);
        if (buyUp == false)
        {
            Debug.Log("금지");
        }
        else
        {
            Debug.Log("금지아님");
        }
    }

}