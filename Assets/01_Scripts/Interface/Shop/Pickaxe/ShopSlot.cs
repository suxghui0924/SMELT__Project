using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopSlot : MonoBehaviour
{
    [SerializeField] PickaxeDataSO _PickaxeDataSO;
    [SerializeField] Image _imgIcon;
    [SerializeField] TextMeshProUGUI _txtTitle;
    [SerializeField] TextMeshProUGUI _txtDesc;
    void Start()
    {
        _imgIcon.sprite = _PickaxeDataSO.icon;
        _txtTitle.text = _PickaxeDataSO.name;
        _txtDesc.text = _PickaxeDataSO.desc;
    }
}