using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftButton : MonoBehaviour
{
    public CostUIManager uiManager;
    public Scoremanager scoreManager;

    public void OnCraft()
    {
        int cost = uiManager.currentCost;

        // 선택 안 했을 때 방지
        if (cost == 0)
        {
            Debug.Log("아이템 먼저 선택!");
            return;
        }

        if (scoreManager.UseScore(cost))
        {
            Debug.Log("제작 성공!");
        }
        else
        {
            Debug.Log("포인트 부족!");
        }
    }
}