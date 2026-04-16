using TMPro;
using UnityEngine;

public class MainMatter : MonoBehaviour
{
    [SerializeField] private int Mmatter = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int cost = 10;

    private void Start()
    {
        UpdateScore();
    }

    public void AddScore(int value)
    {
        Mmatter += value;
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = $"Last Main Matter: {Mmatter}";
    }
    public bool UseScore(int cost)
    {
        if (Mmatter >= cost)
        {
            Mmatter -= cost;
            scoreText.text = $"Last Main Matter: {Mmatter}";
            return true; // 성공
        }
        else
        {
            Debug.Log("점수 부족!");
            return false; // 실패
        }
    }

}
