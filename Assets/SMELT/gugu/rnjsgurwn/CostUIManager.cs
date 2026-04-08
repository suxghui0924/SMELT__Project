using TMPro;
using UnityEngine;

public class CostUIManager : MonoBehaviour
{
    public TextMeshProUGUI costText;

    public void SetCost(int cost)
    {
        costText.text = $"Need Point: {cost}";
    }
}