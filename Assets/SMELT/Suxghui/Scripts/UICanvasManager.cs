using DG.Tweening;
using System;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager instance;

    [SerializeField] private Canvas _hub;
    [SerializeField] private Canvas _popup;
    [SerializeField] private Canvas _system;
    [SerializeField] TextMeshProUGUI _textLabelGameOver;
    [SerializeField] GameObject[] HubTopObject;
    [SerializeField] GameObject[] HubCenterObject;
    [SerializeField] GameObject[] HubBottomObject;
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
        foreach (GameObject obj in HubTopObject)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in HubCenterObject)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in HubBottomObject)
        {
            obj.SetActive(false);
        }
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
        Sequence fadeSequence = DOTween.Sequence();
//        fadeSequence.Append(DOTween.To(() => 0, color => 1) fadeCanvasGroup.DOFade(1, 1f).SetDelay(.5f).SetEase(Ease.InQuint));
    }
    private void Start()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }

    public void OpenCanvas(string canvasName)
    {
        if (canvasName == "Hub")
        {
            _hub.gameObject.SetActive(true);
        }
        else if (canvasName == "Popup")
        {
            _popup.gameObject.SetActive(true);
        }
        else if (canvasName == "System")
        {
            _system.gameObject.SetActive(true);
        }
    }

    public void CloseCanvas(string canvasName)
    {
        if (canvasName == "Hub")
        {
            _hub.gameObject.SetActive(false);
        }
        else if (canvasName == "Popup")
        {
            _popup.gameObject.SetActive(false);
        }
        else if (canvasName == "System")
        {
            _system.gameObject.SetActive(false);
        }
    }
    public void ControlObject(string canvasName, int index, bool value)
    {
        if (canvasName == "HubTop")
        {
            if (HubTopObject[index] != null)
                HubTopObject[index].SetActive(value);
        }
        else if (canvasName == "HubCenter")
        {
            if (HubCenterObject[index] != null)
                HubCenterObject[index].SetActive(value);
        }
        else if (canvasName == "HubBottom")
        {
            if (HubBottomObject[index] != null)
                HubBottomObject[index].SetActive(value);
        }
        else if (canvasName == "Popup")
        {
            if (PopupObject[index] != null)
                PopupObject[index].SetActive(value);
        }
        else if (canvasName == "System")
        {
            if (SystemObject[index] != null)
                SystemObject[index].SetActive(value);
        }
    }
    void UpdateUiTextLabel()
    {
        _textLabelGameOver.text = $"{Qty[0]} 일차\n{Qty[1]}\n{Qty[2]}\n{Qty[3]}\n{Qty[4]}\n{Qty[5]}";
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
        GameManager.instance.ChangeState(GameDataSO.GameState.Lobby);
    }
}
