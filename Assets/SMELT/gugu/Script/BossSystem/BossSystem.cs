using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class BossSystem : MonoBehaviour
{
    public Image _Hp;
    public static BossSystem Instance;

   
    private bool isStart = true;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _Hp.fillAmount = 0;        
    }
    //시작할때 보스 체력바가 차오르는 연출
    private void Update()
    {
        if(isStart) OnStarting();
    }
   

    private void OnStarting()
    {
        if (_Hp.fillAmount == 1||!isStart) isStart = false;
        _Hp.fillAmount += 1f * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TryGetComponent(out BSkillAttack attack))
        {
            _Hp.fillAmount -= attack.damage / 100;
        }
    }
}