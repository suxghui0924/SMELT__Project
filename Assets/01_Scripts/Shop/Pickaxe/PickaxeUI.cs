using UnityEngine;

namespace _01_Scripts.Shop.Pickaxe
{
    public class PickaxeUI : MonoBehaviour
    {
        [SerializeField] PickaxeDataListSO _pickaxeDataListSO;
        #region pickaxe Buy
        public void PickaxeBuy(int index)
        {
            int count = 0;
            int result = 0;
            Debug.Log(_pickaxeDataListSO.pickaxeDataSO[index].name + " pickaxe buying");
            #region fruit Price More Check
            for(int i = 0; i <= _pickaxeDataListSO.pickaxeDataSO[index].fruitPrice.Length; i++)
            {
                if (_pickaxeDataListSO.pickaxeDataSO[index].fruitPrice[index] > 0)
                {
                    count++;
                    Debug.Log("Sus Fruit Price");
                    #region fruit Price Check
                    if (InventoryManager.Instance.HasItem(_pickaxeDataListSO.fruitTypes[index],_pickaxeDataListSO.pickaxeDataSO[index].fruitPrice[index]))
                    {
                        result++;
                        Debug.Log("full Fruit");
                    }
                    else
                    {
                        Debug.Log("need more Fruit");
                    }
                    #endregion
              
                }
                else
                {
                    Debug.Log("과일석이 전부가 0개임.");
                }
            }

            if (result >= count)
            {
                #region gold Price Check
                if (InventoryManager.Instance.SpendGold(_pickaxeDataListSO.pickaxeDataSO[index].goldPrice))
                    Debug.Log("Bought Pickaxe");
                else
                    Debug.Log("No Money");
                #endregion
            }
            else
            {
                Debug.Log("과일석이 부족합니다");
            }
            #endregion
        
        }
        #endregion
        #region pickaxe Eqiuqed

        public void PickaxeEqiuqed(int index)
        {
        
        }
        #endregion

    }
}