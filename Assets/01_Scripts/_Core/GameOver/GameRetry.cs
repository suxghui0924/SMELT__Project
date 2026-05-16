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
        Qty[1] = InventoryManager.Instance.Gold;
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
        UICanvasManager.instance.ControlObject(ObjectType.GameOver, false);
        GameManager.instance.ChangeState(new LobbyState());
    }
}
