using System;
using UnityEngine;
using UnityEngine.UI;
public class BossSceneSceneHpUI : MonoBehaviour
{
    [SerializeField] private Image _staminaUI;
    private float hp = 100;


    public void HpChanged(float amount)
    {
        _staminaUI.fillAmount -= (amount+0.0f)/100f;
      
    }
}
