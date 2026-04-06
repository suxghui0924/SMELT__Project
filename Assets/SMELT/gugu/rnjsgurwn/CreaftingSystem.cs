using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public int playerScore = 100; // 현재 점수
    public int cost = 10;         // 제작 비용

    public void Craft()
    {
        if (playerScore >= cost)
        {
            playerScore -= cost;
            Debug.Log("제작 성공! 남은 점수: " + playerScore);
        }
        else
        {
            Debug.Log("점수가 부족해서 제작 불가!");
        }
    }
}