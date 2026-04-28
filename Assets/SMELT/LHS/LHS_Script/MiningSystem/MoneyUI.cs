using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        if (MoneyManager.Instance != null)
        {
            text.text = "Money: " + MoneyManager.Instance.money;
        }
    }
}