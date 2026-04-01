using UnityEngine;
using UnityEngine.UI;

public class UI_HpChangingScript : MonoBehaviour
{
    [SerializeField] private GameObject hp;

   [SerializeField]private Player_HpScript _playerHpScript;
    private Image[] _hpImageArr; //풀피만큼 피 이미지 넣기
    [Header("차있는 HP 이미지")]
    [SerializeField] private Sprite _hpEnabledImage;
    [Header("비어있는 HP 이미지")]
    [SerializeField] private Sprite _hpDisabledImage;
    private int _maxHpCount; //Hp스크립트에서 가져올 풀피 칸
    private int _currentHp;

   
    private void Start()
    {
        //_playerHpScript = GetComponent<Player_HpScript>();
        _maxHpCount =_playerHpScript._playerMaxHp;
        _hpImageArr = new Image[_maxHpCount];
        for (int i = 0; i < _maxHpCount; i++)
        {
            GameObject _HpPrefabToGameObject = Instantiate(hp, transform);
            _hpImageArr[i] = _HpPrefabToGameObject.GetComponent<Image>();
        }
    }

    public void HealthViewUpdate(int hp)
    {
        for (int i = 0; i < _hpImageArr.Length; i++)
        {
            if (i >= hp)
            {
                _hpImageArr[i].sprite = _hpDisabledImage;
            }
            else
            {
                _hpImageArr[i].sprite = _hpEnabledImage;
            }
        }
    }
}
