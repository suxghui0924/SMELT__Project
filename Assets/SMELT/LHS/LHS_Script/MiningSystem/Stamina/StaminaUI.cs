using System;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
  [SerializeField] private Image _staminaUI;

  private void OnEnable()
  {
      TimerAndReward.Instance.OnFatigueChange.AddListener(HandleFatigueChanged);
  }

  private void HandleFatigueChanged(float amount)
  {
    _staminaUI.fillAmount = (amount+0.0f)/100f;
      
  }

  private void OnDisable()
  {
      TimerAndReward.Instance.OnFatigueChange.RemoveListener(HandleFatigueChanged);
  }
}
