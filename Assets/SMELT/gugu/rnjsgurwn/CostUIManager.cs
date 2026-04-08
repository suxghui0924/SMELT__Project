using TMPro;
using UnityEngine;

public class CostUIManager : MonoBehaviour
{
    public TextMeshProUGUI costText;

    public int currentCost; 

    public void SetCost(int cost)
    {
        currentCost = cost;
        costText.text = $"Need Point: {cost}";
    }
}