using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Upgrading : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //private PlayerStatManager plStatData;
    [SerializeField] public SOUpgrading upso;

    public Button upgradeButton;
    public GameObject checkMark;
    
    private bool _first;
    private GameObject _treesUI;
    
    private TreesUI _treesUIScripts;
    
    private Image _image;
    private Sprite _sprite;

    private void Awake()
    {
        _treesUI = transform.Find("TreeUI").gameObject;
        _treesUIScripts =  _treesUI.GetComponent<TreesUI>();
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        UpdateTreesUI();
        if (upso.needName == "First") 
            _first = true;
        _sprite = _image.sprite;
    }
    
    private void Update()
    {
        if (!_first)
            CanUp();
    }

    public void UpdateTreesUI()
    {
        bool isCompleted = PlayerStatManager.Instance.IsUpgradePurchased(upso.upName);

        if (isCompleted)
        {
            upgradeButton.interactable = false; 
            checkMark.SetActive(true);
        }
        else
        {
            upgradeButton.interactable = true;
            checkMark.SetActive(false);
        }
    }
    public void OnClickUpgradeButton()
    {
        bool isSuccess = PlayerStatManager.Instance.BuyUpgrade(upso.upName, upso.needMoney, upso.upTime);

        if (isSuccess)
        {
            Debug.Log("업그레이드 성공!");
            UpdateTreesUI();
        }
        else
        {
            Debug.Log("업그레이드 실패! 돈이 부족합니다.");
        }
    }
    private void CanUp()
    {
        bool isCompleted1 = PlayerStatManager.Instance.IsUpgradePurchased(upso.upName);
        bool isCompleted2 = PlayerStatManager.Instance.IsUpgradePurchased(upso.needName);

        if (!isCompleted1 && isCompleted2)
        {
            upgradeButton.interactable = true;
            checkMark.SetActive(false);
        }
        else
        {
            upgradeButton.interactable = false;
            checkMark.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _treesUI.SetActive(true);
        _treesUIScripts.icon.sprite = _sprite;
        _treesUIScripts.needMoney.text = upso.needMoney.ToString();
        _treesUIScripts.upName.text = upso.upName;
        _treesUIScripts.detail.text = $"{upso.upTime} times";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _treesUI.SetActive(false);
    }
}