using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public Scoremanager scoreManager;
    public int cost = 10;

    public void Craft()
    {
        if (scoreManager.UseScore(cost))
        {
            Debug.Log("제작 성공!");
        }
        else
        {
            Debug.Log("점수 부족!");
        }
    }
}