using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class Upgrading : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //private PlayerStatManager plStatData;
    [SerializeField] public SOUpgrading upso;

    public Button upgradeButton;
    public GameObject checkMark;
    private Image _thisSprite;
    
    private bool _first;
    private GameObject _treesUI;
    
    private TreesUI _treesUIScripts;
    
    private Image _image;
    private Sprite _sprite;

    [SerializeField] private bool isTutorial;

    [SerializeField]private TMP_FontAsset myFontAsset;
    [SerializeField]private TMP_SpriteAsset mySpriteAsset;
    
    [Header("이미지 세팅")]
    [Range(10f, 300f)] 
    [SerializeField]private float imageSize = 200f ;      // 이미지 크기 

    [Range(-2f, 2f)] 
    [SerializeField] private float verticalOffset = 0f; // 위 아래 오프셋

    [Range(-5f, 5f)]
    [SerializeField]  private float horizontalSpace;  // 좌우 띄어쓰기

    private void Setting()
    {
        _treesUI = transform.Find("TreeUI").gameObject;
        _treesUIScripts =  _treesUI.GetComponent<TreesUI>();
        _image = GetComponent<Image>();
        _thisSprite = transform.Find("CheckMark").GetComponent<Image>();
    }
    
    

    private void OnEnable()
    {
        Setting();
        UpdateTreesUI();
        if (upso.needName == "First")
        {
            _first = true;
            isTutorial = true;
        }
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

        if (isSuccess && isTutorial)
        {
            StartCoroutine(TutoCoroutine());
            Debug.Log("업그레이드 성공!");
            UpdateTreesUI();
        }
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
            
            if (!isCompleted2)
            {
                _thisSprite.color = Color.red;
            }
            else
            {
                _thisSprite.color = Color.green;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _treesUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _treesUI.SetActive(false);
    }

    public void StatText()
    {
        ResetText();
        
        _treesUIScripts.icon.sprite = _sprite;
        
        _treesUIScripts.needMoney.text = upso.needMoney switch
        {
            >= 1000000000 => $"<space={horizontalSpace}em><size={imageSize}%><voffset={verticalOffset}em><sprite=0><voffset=0></size>{(float)upso.needMoney/1000000000:f1} B",
            >= 1000000 => $"<space={horizontalSpace}em><size={imageSize}%><voffset={verticalOffset}em><sprite=0><voffset=0></size>{(float)upso.needMoney/1000000:f1} M",
            >= 1000 => $"<space={horizontalSpace}em><size={imageSize}%><voffset={verticalOffset}em><sprite=0></voffset></size> {(float)upso.needMoney/1000:f1} K",
            >= 0 => $"<space={horizontalSpace}em><size={imageSize}%><voffset={verticalOffset}em><sprite=0><voffset=0></size>{(float)upso.needMoney:f0}",
            _ => $"Error"
        };
        _treesUIScripts.upName.text = upso.upName;
        _treesUIScripts.detail.text = $"{upso.upTime*100}% plus";
    }

    private void ResetText()
    {
        imageSize = 200f ;
        verticalOffset = 0f;
        
        _treesUIScripts.needMoney.font = myFontAsset;
        _treesUIScripts.upName.font = myFontAsset;
        _treesUIScripts.detail.font = myFontAsset;
        
        _treesUIScripts.needMoney.spriteAsset = mySpriteAsset;
        _treesUIScripts.upName.spriteAsset = mySpriteAsset;
        _treesUIScripts.detail.spriteAsset = mySpriteAsset;

        _treesUIScripts.needMoney.alignment = TextAlignmentOptions.Bottom;
        
        _treesUIScripts.needMoney.SetAllDirty();
        _treesUIScripts.upName.SetAllDirty();
        _treesUIScripts.detail.SetAllDirty();
    }

    private IEnumerator TutoCoroutine()
    {
        isTutorial = false;
        yield return null;
        StartCoroutine(TutoManager.Instance.TutoSaying.SayingCoroutine(TutoManager.Instance.TutoLine.store3));
    }
}