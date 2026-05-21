using TMPro;
using UnityEngine;

/// <summary>
/// 가게 열기/닫기 토글 버튼 컴포넌트.
/// 버튼 게임오브젝트에 붙이고 _label에 TextMeshProUGUI를 연결하세요.
/// 버튼 OnClick → ShopManager.ToggleShop()
/// </summary>
public class ShopToggleButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;

    private void OnEnable()
    {
        ShopManager.OnShopToggled += UpdateLabel;
        if (ShopManager.Instance != null)
            UpdateLabel(ShopManager.Instance.IsShopOpen);
    }

    private void OnDisable()
    {
        ShopManager.OnShopToggled -= UpdateLabel;
    }

    private void UpdateLabel(bool isOpen)
    {
        if (_label != null)
            _label.text = isOpen ? "가게 닫기" : "가게 열기";
    }
}
