using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CanvasType { Hud, Popup, System };
public enum ObjectType { Top, Center, Bottom, ShopASkill, DayNext, Setting , Fade, GameOver, Loading };

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager instance;

    [SerializeField] private Canvas _hud;
    [SerializeField] private Canvas _popup;
    [SerializeField] private Canvas _system;
    [SerializeField] TextMeshProUGUI _textLabelGameOver;
    [SerializeField] GameObject HudTopObject;
    [SerializeField] GameObject HudCenterObject;
    [SerializeField] GameObject HudBottomObject;
    [SerializeField] GameObject[] PopupObject;
    [SerializeField] GameObject[] SystemObject;
    [SerializeField] CanvasGroup fadeCanvasGroup;
    int[] Qty = new int[6];
    string[] itemIds = { "fruitstone_apple", "fruitstone_melon", "fruitstone_orange", "fruitstone_lemon", "fruitstone_grape" };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        foreach (GameObject obj in PopupObject)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in SystemObject)
        {
            obj.SetActive(false);
        }
    }

    public void FadeStart()
    {
        DG.Tweening.Sequence fadeSequence = DOTween.Sequence();
//        fadeSequence.Append(DOTween.To(() => 0, color => 1) fadeCanvasGroup.DOFade(1, 1f).SetDelay(.5f).SetEase(Ease.InQuint));
    }
    private void Start()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }

    public void SetCanvasActive(CanvasType canvasName, bool isActive)
    {
        switch(canvasName)
        {
            case CanvasType.Hud: _hud.gameObject.SetActive(isActive); break;
            case CanvasType.Popup: _popup.gameObject.SetActive(isActive); break;
            case CanvasType.System: _system.gameObject.SetActive(isActive); break;
        }
    }
    public void ControlObject(ObjectType canvasName, bool isActive)
    {
        switch(canvasName)
        {
            case ObjectType.Top: if (HudTopObject != null) HudTopObject.SetActive(isActive); break;
            case ObjectType.Center: if (HudCenterObject != null) HudCenterObject.SetActive(isActive); break;
            case ObjectType.Bottom: if (HudBottomObject != null) HudBottomObject.SetActive(isActive); break;
            case ObjectType.ShopASkill: if (PopupObject[0] != null) PopupObject[0].SetActive(isActive); break;
            case ObjectType.DayNext: if (PopupObject[1] != null) PopupObject[1].SetActive(isActive); break;
            case ObjectType.Setting: if (PopupObject[2] != null) PopupObject[2].SetActive(isActive); break;
            case ObjectType.Fade: if (SystemObject[0] != null) SystemObject[0].SetActive(isActive); break;
            case ObjectType.GameOver: if (SystemObject[1] != null) SystemObject[1].SetActive(isActive); break;
            case ObjectType.Loading: if (SystemObject[2] != null) SystemObject[2].SetActive(isActive); break;
        }
    }
    void UpdateUiTextLabel()
    {
        _textLabelGameOver.text = $"{Qty[0]} 일차\n{Qty[1].ToString("N0")}\n{Qty[2].ToString("N0")}\n{Qty[3].ToString("N0")}\n{Qty[4].ToString("N0")}\n{Qty[5].ToString("N0")}";
    }
    public void GetQty()
    {
        Qty[0] = InventoryManager.Instance.CurrentDay;
        Qty[1] = InventoryManager.Instance.Gold;
        Qty[3] = InventoryManager.Instance.Gold;
        Qty[4] = InventoryManager.Instance.Gold;
        Qty[5] = InventoryManager.Instance.Gold;
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
        //GameManager.instance.ChangeState(GameDataSO.GameState.Lobby);
    }
}
