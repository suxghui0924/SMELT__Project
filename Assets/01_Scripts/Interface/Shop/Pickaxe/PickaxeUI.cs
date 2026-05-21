using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Shop.Pickaxe
{
    public class PickaxeUI : MonoBehaviour
    {
        [SerializeField] PickaxeDataListSO _pickaxeDataListSO;
        [SerializeField] private GameObject objectGroup;
        [SerializeField] private TextMeshProUGUI tmpTextLabelName;
        [SerializeField] private TextMeshProUGUI tmpTextLabelGold;
        [SerializeField] private TextMeshProUGUI tmpTextLabelFruit;
        [SerializeField] private TextMeshProUGUI tmpTextLabelResult;
        [SerializeField] private GameObject[] pickaxeBuyItems;
        [SerializeField] private GameObject[] pickaxeEquiedItems;
        [SerializeField] private TextMeshProUGUI[] pickaxeEquiedTexts;
        private int _index = 0;
        PickaxeDataSO curPickaxeDataSO;

        void OnEnable()
        {
            InitPickaxeState();
        }
        #region pickaxe Init
        private void InitPickaxeState()
        {
            for (int i = 0; i < _pickaxeDataListSO.pickaxeDataSO.Length-1; i++)
            { Debug.Log(i);
                if ( _pickaxeDataListSO != null && PickaxeManager.Instance.IsPickaxePurchased(_pickaxeDataListSO.pickaxeDataSO[i].pickaxeId))
                {
                    Debug.Log("구매됨 " + _pickaxeDataListSO.pickaxeDataSO[i].name);

                    _pickaxeDataListSO.pickaxeDataSO[i].bought = true;
                    pickaxeBuyItems[i].SetActive(false);
                    pickaxeEquiedItems[i].SetActive(true);
                }
                else
                {
                    Debug.Log(_pickaxeDataListSO.pickaxeDataSO[i].name);
                    pickaxeBuyItems[i].SetActive(true);
                    pickaxeEquiedItems[i].SetActive(false);
                }
            }           
            /*foreach (PickaxeDataSO pickaxeData in  _pickaxeDataListSO.pickaxeDataSO)
            {
                if (PickaxeManager.Instance.IsPickaxePurchased(pickaxeData.name))
                {
                    Debug.Log("Work");
                    pickaxeData.bought = true;
                    pickaxeBuyItems[index].SetActive(false);
                    pickaxeEquiedItems[index].SetActive(true);
                }
                else
                {
                    pickaxeBuyItems[index].SetActive(true);
                    pickaxeEquiedItems[index].SetActive(false);
                }
                Debug.Log(index);
                if (index != 4) index++;
            }
            index = 0;*/
        }
        #endregion
        #region pickaxe Buy
        public void PickaxeBuy(int index)
        {
            objectGroup.SetActive(true);
            _index = index;
            InitPickaxe(_pickaxeDataListSO.pickaxeDataSO[index]);
        }

        private void InitPickaxe(PickaxeDataSO pickaxeDataSO)
        {
            curPickaxeDataSO =  pickaxeDataSO;
            tmpTextLabelName.text = curPickaxeDataSO.name;
            tmpTextLabelGold.text = $"소모할 골드 : {curPickaxeDataSO.goldPrice.ToString("N0")}";
            tmpTextLabelFruit.text = "소모할 자원 : ";
            for (int i = 0; i < curPickaxeDataSO.fruitPrice.Length; i++)
            {
                if(curPickaxeDataSO.fruitPrice[i]>0)
                        tmpTextLabelFruit.text = $"{tmpTextLabelFruit.text} {_pickaxeDataListSO.fruitTypesKr[i]} {curPickaxeDataSO.fruitPrice[i]}x ";
            }
           
        }
        #endregion
        #region group Buy && group Buy Cancel
        
        public void GroupBuy()
        {
            if (PickaxeManager.Instance.BuyPickaxe(curPickaxeDataSO))
            {
                SoundManager.instance.PlaySFX("UIClick");
                getCurPickaxeDataSO();
                GroupBuyCancel();
                AchievementManager.Instance.AlarmPopUp("상점 알리미",$"성공적으로 {curPickaxeDataSO.name} 구매를 하였습니다.");
            }
            else
            {
                SoundManager.instance.PlaySFX("failed");
                AchievementManager.Instance.AlarmPopUp("상점 알리미",$"성공적으로 {curPickaxeDataSO.name} 구매를 하지 못했습니다.");
                //N
            }
            
        }

        private void getCurPickaxeDataSO()
        {
            
            if (curPickaxeDataSO == null) return;
                    Debug.Log("Work2");
            curPickaxeDataSO.bought = true;
            InitPickaxeState();
          }

        public void GroupBuyCancel()
        {
            objectGroup.SetActive(false);
            //StartCoroutine(GroupClose(1f));
        }
        
        /*IEnumerator GroupClose(float timer)
        {
            yield return new WaitForSeconds(timer);

        }*/
        #endregion
        #region pickaxe Eqiuqed
        public void PickaxeEqiuqed(int index)
        {
            if (PickaxeManager.Instance.EquipPickaxe(_pickaxeDataListSO.pickaxeDataSO[index]))
            {
                foreach (TextMeshProUGUI textlabel in pickaxeEquiedTexts)
                {
                    textlabel.text = "장착하기";
                }
                pickaxeEquiedTexts[index].text = "장착완료";
                Debug.Log("장착");
            }
        }
        #endregion
        
    }
}