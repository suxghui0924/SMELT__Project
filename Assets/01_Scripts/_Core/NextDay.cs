using System;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Player
{
    public class NextDay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textLabelData;
        [SerializeField] private TextMeshProUGUI _textLabelDuty;
        [SerializeField] private TextMeshProUGUI _textLabelResult;
        private int _outData;

        void Start()
        {
            UpdateOutData();
        }

        private void OnEnable()
        {
            NextDayOnViusalButton();
        }

        private void UpdateOutData()
        {
            _outData = (int)InventoryManager.Instance.Gold;
        }

        public void NextDayOnViusalButton()
        {
            this.gameObject.SetActive(true);
            _textLabelData.text = $"현재 날짜(Day {InventoryManager.Instance.CurrentDay}) -> 다음 날짜(Day {InventoryManager.Instance.CurrentDay +1})";
            _textLabelDuty.text = $"오늘의 유지비용 : { InventoryManager.Instance.MaintenanceCost.ToString("N0")} 골드";
            _textLabelResult.text = $"오늘의 수입 : {(InventoryManager.Instance.Gold - (ulong)_outData).ToString("N0")} 골드";
                
        }
        
        public void NextDayOnButton()
        {
            //this.gameObject.SetActive(false);
            NextDayOnViusalButton();
            if (InventoryManager.Instance.EndOfDay())
            {
                AchievementManager.Instance.AlarmPopUp("유지비 송금 성공", "집 주인이게 송금을 하였기에 다음날로 넘어갑니다.");
                TimerAndReward.Instance.canEnter = true;
                TimerAndReward.Instance.FatigueReset();
                UpdateOutData();
            }
            else
            {
                AchievementManager.Instance.AlarmPopUp("유지비 송금 실패", "돈이 부족하여 다음날로 넘어가지 못합니다.");
            }
        }

        public void NextDayCancelButton()
        {
            this.gameObject.SetActive(false);
        }
    }
}