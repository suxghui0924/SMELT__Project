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

        private void UpdateOutData()
        {
            _outData = InventoryManager.Instance.Gold;
        }

        public void NextDayOnViusalButton()
        {
            this.gameObject.SetActive(true);
            _textLabelData.text = $"현재 날짜(Day {InventoryManager.Instance.CurrentDay}) -> 다음 날짜(Day {InventoryManager.Instance.CurrentDay +1})";
            _textLabelDuty.text = $"오늘의 유지비용 : { InventoryManager.Instance.MaintenanceCost.ToString("N0")} 골드";
            _textLabelResult.text = $"오늘의 수입 : {(InventoryManager.Instance.Gold - _outData).ToString("N0")} 골드";
                
        }
        
        public void NextDayOnButton()
        {
            //this.gameObject.SetActive(false);
            NextDayOnViusalButton();
            if (InventoryManager.Instance.EndOfDay())
            {
                Debug.Log("넘어갑니다");
                
                UpdateOutData();
            }
            else
            {
                Debug.Log("못넘어가용");
            }
        }

        public void NextDayCancelButton()
        {
            this.gameObject.SetActive(false);
        }
    }
}