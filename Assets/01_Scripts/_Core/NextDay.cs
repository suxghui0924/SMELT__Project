using System;
using _01_Scripts.Player.Portal;
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
            _textLabelData.text = $"현재 날짜(Day {InventoryManager.Instance.CurrentDay}) -> 다음 날짜(Day {InventoryManager.Instance.CurrentDay + 1})";
            _textLabelDuty.text = $"오늘의 유지비용 : { InventoryManager.Instance.MaintenanceCost.ToString("N0")} 골드";
            _textLabelResult.text = $"오늘의 수입 : {(InventoryManager.Instance.Gold - (ulong)_outData).ToString("N0")} 골드";
                
        }
        
        public void NextDayOnButton()
        {
            /*if (InventoryManager.Instance.CurrentDay == 7)
            {
                if (PortalController.Instance.bossIsComplete)
                {
                    if (InventoryManager.Instance.EndOfDay())
                    {
                        AchievementManager.Instance.AlarmPopUp("유지비 납부 성공", "가게 유지비를 납부하여 다음날로 넘어갑니다.");
                        TimerAndReward.Instance.canEnter = true;
                        TimerAndReward.Instance.FatigueReset();
                        UpdateOutData();
                        NextDayOnViusalButton();
                    }
                    else
                    {
                        AchievementManager.Instance.AlarmPopUp("유지비 송금 실패", "돈이 부족하여 다음날로 넘어가지 못합니다.");
                    }
                }
                else
                {
                    AchievementManager.Instance.AlarmPopUp("다음날로 넘어가지 못합니다.", "포탈을 들어간 후 보스를 잡고 오시기 바랍니다.");
                }
            }
            else
            {*/
                if (InventoryManager.Instance.EndOfDay())
                {
                    AchievementManager.Instance.AlarmPopUp("유지비 납부 성공", "가게 유지비를 납부하여 다음날로 넘어갑니다.");
                    ShopManager.Instance?.CloseShop();
                    TimerAndReward.Instance.canEnter = true;
                    TimerAndReward.Instance.isNextDay = true;
                    TimerAndReward.Instance.FatigueReset();
                    DayTimer.Instance?.ResetTimer();
                    UpdateOutData();
                    NextDayOnViusalButton();
                }
                else
                {
                    AchievementManager.Instance.AlarmPopUp("유지비 납부 실패.", "가게 유지비를 납부하지 못하여 다음날로 못 넘어갑니다.");
                }
            /*
            }*/
        }

        public void NextDayCancelButton()
        {
            this.gameObject.SetActive(false);
        }
    }
}