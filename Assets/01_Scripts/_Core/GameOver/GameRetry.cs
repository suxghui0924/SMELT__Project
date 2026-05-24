    using _01_Scripts._Core._States;
    using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class GameRetry : MonoBehaviour
{
     
    [SerializeField] TextMeshProUGUI _textLabel;
    public static GameRetry instance;
    int[] Qty = new int[6];
    string[] itemIds = { "fruitstone_apple", "fruitstone_melon", "fruitstone_orange", "fruitstone_lemon", "fruitstone_grape" };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void UpdateUiTextLabel()
    {
        _textLabel.text = $"{Qty[0]} 일차\n{Qty[1]}\n{Qty[2]}\n{Qty[3]}\n{Qty[4]}\n{Qty[5]}";
    }
    private void Start()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }
    public void GetQty()
    {
        Qty[0] = InventoryManager.Instance.CurrentDay;
        Qty[1] = (int)InventoryManager.Instance.Gold;
        /*       Qty[3] = InventoryManager.Instance.Gold;
               Qty[4] = InventoryManager.Instance.Gold;
               Qty[5] = InventoryManager.Instance.Gold;*/
        for (int i = 0; i < itemIds.Length; i++)
        {
            Qty[2] += InventoryManager.Instance.GetQuantity(itemIds[i]);
        }

        UpdateUiTextLabel();
    }
    public void RestartButton()
    {
        Debug.Log("버튼 클릭됨!");
        Qty = new int[6];

        // 가게·주문 초기화 → NPC 퇴장
        ShopManager.Instance?.CloseShop();
        // 게임 오버 시 세이브 데이터 초기화 (일차·골드·인벤토리 리셋)
        SaveManager.Instance?.ResetAllData();
        // 타이머 초기화 (DDOL이라 직접 리셋 필요)
        DayTimer.Instance?.ResetTimer();

        UICanvasManager.instance.ControlObject(ObjectType.GameOver, false);
        UICanvasManager.instance.ControlObject(ObjectType.StoreBroken, false);
        GameManager.instance.ChangeState(new LobbyState());
    }
}
