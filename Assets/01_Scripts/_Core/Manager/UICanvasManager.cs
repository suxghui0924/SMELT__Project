using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CanvasType { Title , Hud, Popup, System };
public enum ObjectType { Top, Center, Bottom, ShopASkill, DayNext, Setting , Fade, GameOver, Loading, GameDie, Radio, Tutorial, Hackboom, StoreBroken };

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager instance;

    [SerializeField] private Canvas _title;
    [SerializeField] private Canvas _hud;
    [SerializeField] private Canvas _popup;
    [SerializeField] private Canvas _system;
    [SerializeField] TextMeshProUGUI _textLabelGameOver;
    [SerializeField] private TextMeshProUGUI _timerText;
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
            //DontDestroyOnLoad(gameObject);
            /*// 캔버스들도 씬 전환 시 파괴되지 않도록 영속화
            TryPersist(_title?.gameObject);
            TryPersist(_hud?.gameObject);
            TryPersist(_popup?.gameObject);
            TryPersist(_system?.gameObject);*/
            Init();
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            /*// 씬 재로드: 이전 캔버스 교체 + 새 캔버스 영속화 + 참조 갱신
            SwapCanvas(ref instance._title,  _title);
            SwapCanvas(ref instance._hud,    _hud);
            SwapCanvas(ref instance._popup,  _popup);
            SwapCanvas(ref instance._system, _system);
            instance._textLabelGameOver = _textLabelGameOver;
            instance.HudTopObject       = HudTopObject;
            instance.HudCenterObject    = HudCenterObject;
            instance.HudBottomObject    = HudBottomObject;
            instance.PopupObject        = PopupObject;
            instance.SystemObject       = SystemObject;
            instance.fadeCanvasGroup    = fadeCanvasGroup;*/
            /*instance.Init();
            Destroy(this);*/
        }
    }
    
    
    public void Init()
    {
        if (SystemObject == null) return;
        foreach (GameObject obj in SystemObject)
            if (obj != null) obj.SetActive(false);
    }

    public void FadeStart()
    {
        DG.Tweening.Sequence fadeSequence = DOTween.Sequence();
//        fadeSequence.Append(DOTween.To(() => 0, color => 1) fadeCanvasGroup.DOFade(1, 1f).SetDelay(.5f).SetEase(Ease.InQuint));
    }

    public void SetCanvasActive(CanvasType canvasName, bool isActive)
    {
        switch(canvasName)
        {
            case CanvasType.Title:  if (_title  != null) _title.gameObject.SetActive(isActive);  break;
            case CanvasType.Hud:    if (_hud    != null) _hud.gameObject.SetActive(isActive);    break;
            case CanvasType.Popup:  if (_popup  != null) _popup.gameObject.SetActive(isActive);  break;
            case CanvasType.System: if (_system != null) _system.gameObject.SetActive(isActive); break;
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
            case ObjectType.Radio: if (PopupObject[3] != null) PopupObject[3].SetActive(isActive); break;
            case ObjectType.GameDie: if (SystemObject[3] != null)  SystemObject[3].SetActive(isActive); break;
            case ObjectType.Tutorial: if (PopupObject[4] != null)  PopupObject[4].SetActive(isActive); break;
            case ObjectType.Hackboom: if (SystemObject[4] != null)  SystemObject[4].SetActive(isActive); break;
            case ObjectType.StoreBroken: if (PopupObject[5] != null)  PopupObject[5].SetActive(isActive); break;
            
        }
    }
    void UpdateUiTextLabel()
    {
        _textLabelGameOver.text = $"{Qty[0]} ����\n{Qty[1].ToString("N0")}\n{Qty[2].ToString("N0")}\n{Qty[3].ToString("N0")}\n{Qty[4].ToString("N0")}\n{Qty[5].ToString("N0")}";
    }
    public void GetQty()
    {
        Qty[2] = 0;
        Qty[0] = InventoryManager.Instance.CurrentDay;
        Qty[1] = (int)InventoryManager.Instance.Gold;
        Qty[3] = (int)InventoryManager.Instance.Gold;
        Qty[4] = (int)InventoryManager.Instance.Gold;
        Qty[5] = (int)InventoryManager.Instance.Gold;
        for (int i = 0; i < itemIds.Length; i++)
        {
            Qty[2] += InventoryManager.Instance.GetQuantity(itemIds[i]);
        }

        UpdateUiTextLabel();
    }
    public void UpdateTimerDisplay(string text, bool isWarning)
    {
        if (_timerText == null) return;
        _timerText.text = text;
        _timerText.color = isWarning ? Color.red : Color.white;
    }

    public void RestartButton()
    {
        Debug.Log("��ư Ŭ����!");
        Qty = new int[6];
        //GameManager.instance.ChangeState(GameDataSO.GameState.Lobby);
    }
}